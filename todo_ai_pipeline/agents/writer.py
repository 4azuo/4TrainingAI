"""Writer agent — viết tài liệu kỹ thuật, spec, hoặc content dựa trên research."""

from .base_agent import BaseAgent


class WriterAgent(BaseAgent):
    name = "writer"
    role_description = (
        "Bạn là Technical Writer Agent. Nhiệm vụ: soạn thảo tài liệu kỹ thuật "
        "rõ ràng, chính xác, có cấu trúc tốt. Viết cho đối tượng là software engineer. "
        "Sử dụng Markdown. Ngắn gọn nhưng đủ ý."
    )

    def run(self, task: dict, client) -> str:
        recent_ctx = self.get_recent_context(5)

        system_prompt = (
            f"{self.role_description}\n\n"
            "## Lịch sử 5 công việc gần nhất của bạn:\n"
            f"{recent_ctx}\n\n"
            "Cấu trúc tài liệu bạn cần viết:\n"
            "1. **Overview** — mục đích và phạm vi tài liệu\n"
            "2. **Background / Context** — thông tin nền cần biết\n"
            "3. **Chi tiết kỹ thuật** — phần chính, có thể dùng bảng, diagram text, code snippet\n"
            "4. **Implementation notes** — hướng dẫn áp dụng thực tế\n"
            "5. **Checklist** — danh sách kiểm tra trước khi deploy / sử dụng"
        )

        user_prompt = (
            f"**Task ID**: {task['id']}\n"
            f"**Tiêu đề**: {task['title']}\n"
            f"**Yêu cầu**: {task['description']}\n"
        )
        if task.get("context_from_dependency"):
            user_prompt += (
                f"\n**Research context (từ task phụ thuộc)**:\n"
                f"{task['context_from_dependency'][:2000]}"
            )

        response = client.messages.create(
            model="claude-haiku-4-5-20251001",
            max_tokens=2000,
            system=system_prompt,
            messages=[{"role": "user", "content": user_prompt}],
        )

        result = response.content[0].text.strip()

        title_short = task["title"][:60]
        summary_short = f"Viết tài liệu: {task['description'][:80]}"
        self.save_job(task["id"], title_short, summary_short)

        header = f"# Document: {task['title']}\n**Task**: {task['id']} | **Agent**: Writer\n\n---\n\n"
        return header + result
