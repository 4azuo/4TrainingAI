"""Verifier agent — kiểm tra chất lượng và tính chính xác của output từ Writer."""

from .base_agent import BaseAgent


class VerifierAgent(BaseAgent):
    name = "verifier"
    role_description = (
        "Bạn là Verifier Agent. Nhiệm vụ: review tài liệu / spec kỹ thuật để "
        "phát hiện lỗi logic, thiếu sót, mâu thuẫn, hoặc vi phạm best practice. "
        "Trả về review report với mức độ: PASS / WARN / FAIL."
    )

    def run(self, task: dict, client) -> str:
        recent_ctx = self.get_recent_context(5)

        system_prompt = (
            f"{self.role_description}\n\n"
            "## Lịch sử 5 công việc gần nhất của bạn:\n"
            f"{recent_ctx}\n\n"
            "Cấu trúc review report:\n"
            "1. **Verdict**: PASS | WARN | FAIL (bold, dòng đầu tiên)\n"
            "2. **Summary**: 1-2 câu tóm tắt nhận xét tổng thể\n"
            "3. **Findings**: danh sách các vấn đề tìm thấy (severity: HIGH/MED/LOW)\n"
            "4. **Suggestions**: đề xuất cải thiện cụ thể\n"
            "5. **Checklist compliance**: kiểm tra các tiêu chí chuẩn"
        )

        content_to_verify = task.get("context_from_dependency", task.get("description", ""))
        user_prompt = (
            f"**Task ID**: {task['id']}\n"
            f"**Tiêu đề review**: {task['title']}\n\n"
            f"**Nội dung cần verify**:\n\n{content_to_verify}"
        )

        response = client.messages.create(
            model="claude-haiku-4-5-20251001",
            max_tokens=1200,
            system=system_prompt,
            messages=[{"role": "user", "content": user_prompt}],
        )

        result = response.content[0].text.strip()

        # Extract verdict for memory title
        verdict = "PASS"
        for line in result.split("\n")[:5]:
            for v in ("FAIL", "WARN", "PASS"):
                if v in line.upper():
                    verdict = v
                    break

        title_short = f"[{verdict}] {task['title'][:50]}"
        summary_short = f"Verify {task['id']}: {verdict}. {task['description'][:60]}"
        self.save_job(task["id"], title_short, summary_short)

        header = f"# Verification Report: {task['title']}\n**Task**: {task['id']} | **Agent**: Verifier\n\n---\n\n"
        return header + result
