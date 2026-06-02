"""Base agent with persistent memory management (max 100 jobs, FIFO eviction)."""

import re
from datetime import datetime
from pathlib import Path


MAX_MEMORY_JOBS = 100

MEMORY_DIR = Path(__file__).parent.parent / "memory"


class BaseAgent:
    name: str  # override in subclass
    role_description: str  # override in subclass

    def __init__(self):
        self.memory_path = MEMORY_DIR / f"{self.name}.md"
        MEMORY_DIR.mkdir(exist_ok=True)
        if not self.memory_path.exists():
            self._init_memory_file()

    # ── Memory I/O ──────────────────────────────────────────────────────────

    def _init_memory_file(self):
        self.memory_path.write_text(
            f"# {self.name.capitalize()} Memory Log\n"
            f"<!-- MAX {MAX_MEMORY_JOBS} ENTRIES — oldest entries removed automatically -->\n\n"
            "## Job History\n\n",
            encoding="utf-8",
        )

    def load_memory(self) -> str:
        """Return raw memory file content."""
        return self.memory_path.read_text(encoding="utf-8")

    def _parse_jobs(self, content: str) -> list[dict]:
        """Extract list of jobs from memory markdown."""
        jobs = []
        # Each job block starts with: ### DATE | TASK-ID | Title
        pattern = re.compile(
            r"### (\d{4}-\d{2}-\d{2}) \| (TASK-\S+) \| (.+?)\n(.*?)(?=\n### |\Z)",
            re.DOTALL,
        )
        for m in pattern.finditer(content):
            jobs.append({
                "date": m.group(1),
                "task_id": m.group(2),
                "title": m.group(3).strip(),
                "summary": m.group(4).strip(),
            })
        return jobs

    def _render_jobs(self, jobs: list[dict]) -> str:
        lines = []
        for job in jobs:
            lines.append(f"### {job['date']} | {job['task_id']} | {job['title']}\n{job['summary']}\n")
        return "\n".join(lines)

    def save_job(self, task_id: str, title: str, summary: str):
        """Append a completed job to memory; evict oldest if > MAX_MEMORY_JOBS."""
        # Trim summary to stay under ~100 words
        words = summary.split()
        if len(words) > 100:
            summary = " ".join(words[:100]) + "..."

        content = self.load_memory()
        jobs = self._parse_jobs(content)

        new_job = {
            "date": datetime.now().strftime("%Y-%m-%d"),
            "task_id": task_id,
            "title": title,
            "summary": summary,
        }
        jobs.append(new_job)

        # Evict oldest jobs beyond the limit
        if len(jobs) > MAX_MEMORY_JOBS:
            evicted = len(jobs) - MAX_MEMORY_JOBS
            jobs = jobs[evicted:]

        header = (
            f"# {self.name.capitalize()} Memory Log\n"
            f"<!-- MAX {MAX_MEMORY_JOBS} ENTRIES — oldest entries removed automatically -->\n\n"
            "## Job History\n\n"
        )
        self.memory_path.write_text(header + self._render_jobs(jobs), encoding="utf-8")

    def get_recent_context(self, n: int = 10) -> str:
        """Return last N jobs as context string for prompt injection."""
        content = self.load_memory()
        jobs = self._parse_jobs(content)
        recent = jobs[-n:] if len(jobs) >= n else jobs
        if not recent:
            return "(Chưa có lịch sử công việc)"
        lines = [f"- [{j['date']}] {j['task_id']}: {j['title']}\n  {j['summary']}" for j in recent]
        return "\n".join(lines)

    # ── Abstract interface ───────────────────────────────────────────────────

    def run(self, task: dict, client) -> str:
        """Execute the task. Must be overridden. Returns result summary."""
        raise NotImplementedError
