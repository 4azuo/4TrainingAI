"""
Orchestrator — reads TODO.md, routes each pending task to the correct sub-agent.

Routing rules (based on task Type field):
  research  → Researcher
  write     → Writer  (then Verifier reviews the output)
  verify    → Verifier
  report    → Reporter
  unknown   → Claude decides via LLM routing call
"""

import json
import re
from pathlib import Path

import anthropic

from agents.researcher import ResearcherAgent
from agents.writer import WriterAgent
from agents.verifier import VerifierAgent
from agents.reporter import ReporterAgent

TODO_PATH = Path(__file__).parent / "TODO.md"
OUTPUT_DIR = Path(__file__).parent / "output"

AGENT_MAP = {
    "research": "researcher",
    "write":    "writer",
    "verify":   "verifier",
    "report":   "reporter",
}


class Orchestrator:
    def __init__(self):
        self.client = anthropic.Anthropic()
        self.agents = {
            "researcher": ResearcherAgent(),
            "writer":     WriterAgent(),
            "verifier":   VerifierAgent(),
            "reporter":   ReporterAgent(),
        }
        OUTPUT_DIR.mkdir(exist_ok=True)

    # ── TODO parsing ─────────────────────────────────────────────────────────

    def _parse_pending_tasks(self) -> list[dict]:
        """Extract all unchecked tasks from TODO.md."""
        content = TODO_PATH.read_text(encoding="utf-8")
        tasks = []

        # Match task blocks:  - [ ] **TASK-NNN**: Title\n  - **Key**: Value ...
        task_pattern = re.compile(
            r"- \[ \] \*\*(TASK-\S+)\*\*: (.+?)\n((?:  - \*\*.+?\*\*: .+\n?)*)",
            re.MULTILINE,
        )
        for m in task_pattern.finditer(content):
            task_id = m.group(1)
            title = m.group(2).strip()
            meta_block = m.group(3)

            meta = {}
            for kv in re.finditer(r"- \*\*(.+?)\*\*: (.+)", meta_block):
                meta[kv.group(1).lower()] = kv.group(2).strip()

            tasks.append({
                "id": task_id,
                "title": title,
                "type": meta.get("type", "unknown"),
                "priority": meta.get("priority", "medium"),
                "description": meta.get("description", title),
                "depends": meta.get("depends", None),
            })

        return tasks

    # ── LLM-based routing (fallback for unknown type) ────────────────────────

    def _llm_route(self, task: dict) -> str:
        """Ask Claude which agent should handle this task."""
        response = self.client.messages.create(
            model="claude-haiku-4-5-20251001",
            max_tokens=64,
            system=(
                "Bạn là orchestrator. Chọn ĐÚNG MỘT agent phù hợp nhất cho task.\n"
                "Chỉ trả lời bằng một trong: researcher | writer | verifier | reporter"
            ),
            messages=[{
                "role": "user",
                "content": (
                    f"Task: {task['title']}\n"
                    f"Description: {task['description']}"
                )
            }],
        )
        choice = response.content[0].text.strip().lower()
        return choice if choice in self.agents else "researcher"

    def _route(self, task: dict) -> str:
        return AGENT_MAP.get(task["type"], self._llm_route(task))

    # ── TODO.md updater ──────────────────────────────────────────────────────

    def _mark_task_done(self, task_id: str):
        """Move task from Pending to Completed in TODO.md."""
        content = TODO_PATH.read_text(encoding="utf-8")

        # Replace  - [ ] **TASK-NNN**  with  - [x] **TASK-NNN**
        updated = re.sub(
            rf"(- )\[ \] (\*\*{re.escape(task_id)}\*\*)",
            r"\1[x] \2",
            content,
        )

        # Move the completed block under ## Completed section
        TODO_PATH.write_text(updated, encoding="utf-8")

    # ── Main entry point ─────────────────────────────────────────────────────

    def process(self):
        tasks = self._parse_pending_tasks()
        if not tasks:
            print("[Orchestrator] Không có task pending nào.")
            return

        # Sort: high > medium > low
        priority_order = {"high": 0, "medium": 1, "low": 2}
        tasks.sort(key=lambda t: priority_order.get(t["priority"], 1))

        print(f"[Orchestrator] Tìm thấy {len(tasks)} task(s) cần xử lý.")

        completed_outputs: dict[str, str] = {}  # task_id → output path

        for task in tasks:
            agent_name = self._route(task)
            agent = self.agents[agent_name]
            print(f"\n[Orchestrator] {task['id']} ({task['priority']}) → {agent_name.upper()}")
            print(f"  Task: {task['title']}")

            # Inject previous task output as context if depends-on exists
            if task.get("depends") and task["depends"] in completed_outputs:
                dep_path = completed_outputs[task["depends"]]
                try:
                    dep_content = Path(dep_path).read_text(encoding="utf-8")
                    task["context_from_dependency"] = dep_content[:2000]
                except Exception:
                    pass

            result_summary = agent.run(task, self.client)

            # Save output to file
            out_file = OUTPUT_DIR / f"{task['id']}_{agent_name}.md"
            out_file.write_text(result_summary, encoding="utf-8")
            completed_outputs[task["id"]] = str(out_file)

            # If Writer ran, automatically run Verifier on its output
            if agent_name == "writer":
                print(f"  → Auto-routing to VERIFIER for quality check")
                verify_task = {
                    "id": task["id"],
                    "title": f"Verify output of {task['id']}",
                    "type": "verify",
                    "priority": task["priority"],
                    "description": f"Kiểm tra chất lượng tài liệu do Writer tạo ra cho task: {task['title']}",
                    "context_from_dependency": result_summary[:3000],
                }
                verifier_result = self.agents["verifier"].run(verify_task, self.client)
                ver_file = OUTPUT_DIR / f"{task['id']}_verifier.md"
                ver_file.write_text(verifier_result, encoding="utf-8")

            self._mark_task_done(task["id"])
            print(f"  [✓] Hoàn thành — output: output/{out_file.name}")

        # Always run Reporter at the end to summarize the session
        print(f"\n[Orchestrator] Chạy REPORTER tổng kết phiên...")
        report_task = {
            "id": "SESSION-REPORT",
            "title": "Báo cáo tổng kết phiên xử lý",
            "type": "report",
            "priority": "low",
            "description": f"Tóm tắt {len(tasks)} task(s) vừa xử lý trong phiên này.",
            "completed_tasks": tasks,
            "output_files": list(completed_outputs.values()),
        }
        report_summary = self.agents["reporter"].run(report_task, self.client)
        report_file = OUTPUT_DIR / "SESSION_REPORT.md"
        report_file.write_text(report_summary, encoding="utf-8")
        print(f"  [✓] Báo cáo: output/SESSION_REPORT.md")
