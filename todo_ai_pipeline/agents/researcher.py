"""Researcher agent — tìm kiếm và tổng hợp thông tin cho một task."""

from .base_agent import BaseAgent


class ResearcherAgent(BaseAgent):
    name = "researcher"
    role_description = (
        "Bạn là Research Agent chuyên tổng hợp thông tin kỹ thuật. "
        "Nhiệm vụ: phân tích yêu cầu, tổng hợp kiến thức, trả về bản tóm tắt "
        "có cấu trúc rõ ràng để Writer và Verifier có thể dùng làm nền tảng."
    )

    def run(self, task: dict, client) -> str:
        recent_ctx = self.get_recent_context(5)

        system_prompt = (
            f"{self.role_description}\n\n"
            "## Lịch sử 5 công việc gần nhất của bạn:\n"
            f"{recent_ctx}\n\n"
            "Hãy trả lời bằng Markdown với các phần:\n"
            "1. **Tổng quan** — định nghĩa / khái niệm cốt lõi\n"
            "2. **Phân tích chi tiết** — các điểm quan trọng, so sánh nếu cần\n"
            "3. **Kết luận & Khuyến nghị** — best practice, điểm cần lưu ý\n"
            "4. **Nguồn tham khảo** — các tài liệu / spec / RFC liên quan (nếu biết)"
        )

        user_prompt = (
            f"**Task ID**: {task['id']}\n"
            f"**Tiêu đề**: {task['title']}\n"
            f"**Mô tả**: {task['description']}\n"
        )
        if task.get("context_from_dependency"):
            user_prompt += f"\n**Context từ task phụ thuộc**:\n{task['context_from_dependency']}"

        response = client.messages.create(
            model="claude-haiku-4-5-20251001",
            max_tokens=1500,
            system=system_prompt,
            messages=[{"role": "user", "content": user_prompt}],
        )

        result = response.content[0].text.strip()

        # Save to memory
        title_short = task["title"][:60]
        summary_short = result.split("\n")[0].replace("#", "").strip()[:200]
        self.save_job(task["id"], title_short, summary_short)

        header = f"# Research: {task['title']}\n**Task**: {task['id']} | **Agent**: Researcher\n\n---\n\n"
        return header + result
