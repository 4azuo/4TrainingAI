"""Reporter agent — tổng hợp báo cáo tiến độ và kết quả phiên làm việc."""

from .base_agent import BaseAgent


class ReporterAgent(BaseAgent):
    name = "reporter"
    role_description = (
        "Bạn là Reporter Agent. Nhiệm vụ: tổng hợp kết quả làm việc thành báo cáo "
        "rõ ràng, súc tích cho team. Nêu bật tiến độ, rủi ro, dependency, và action items."
    )

    def run(self, task: dict, client) -> str:
        recent_ctx = self.get_recent_context(5)

        # Build task summary for prompt
        completed_tasks = task.get("completed_tasks", [])
        task_lines = "\n".join(
            f"- {t['id']} ({t['priority']}): {t['title']} → type={t['type']}"
            for t in completed_tasks
        ) or task.get("description", "")

        system_prompt = (
            f"{self.role_description}\n\n"
            "## Lịch sử 5 công việc gần nhất của bạn:\n"
            f"{recent_ctx}\n\n"
            "Cấu trúc báo cáo:\n"
            "1. **Executive Summary** — 3-5 bullet points tóm tắt phiên này\n"
            "2. **Tasks Completed** — bảng: Task ID | Tiêu đề | Agent | Trạng thái\n"
            "3. **Key Findings** — những phát hiện quan trọng từ Researcher/Verifier\n"
            "4. **Risks & Blockers** — rủi ro và task bị block (nếu có)\n"
            "5. **Next Actions** — việc cần làm tiếp theo"
        )

        user_prompt = (
            f"**Báo cáo cho**: {task['title']}\n"
            f"**Mô tả**: {task['description']}\n\n"
            f"**Danh sách tasks đã xử lý**:\n{task_lines}"
        )

        response = client.messages.create(
            model="claude-haiku-4-5-20251001",
            max_tokens=1200,
            system=system_prompt,
            messages=[{"role": "user", "content": user_prompt}],
        )

        result = response.content[0].text.strip()

        title_short = task["title"][:60]
        summary_short = f"Báo cáo {len(completed_tasks)} tasks. {task['description'][:60]}"
        self.save_job(task["id"], title_short, summary_short)

        from datetime import datetime
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M")
        header = (
            f"# Session Report — {timestamp}\n"
            f"**Task**: {task['id']} | **Agent**: Reporter\n\n---\n\n"
        )
        return header + result
