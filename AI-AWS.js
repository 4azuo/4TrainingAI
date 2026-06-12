/* ════════════════════════════════════════════════════════════
   TAB MANIFEST — nguồn duy nhất định nghĩa các tab của trang này.
   Thêm tab mới = thêm 1 dòng ở đây + 1 file trong thư mục AI-AWS/.
   Logic chung (nav, search, tabs, mermaid) nằm ở common.js.
   Nội dung bám theo AWS Certified Generative AI Developer – Professional.
   ════════════════════════════════════════════════════════════ */
var TABS = [
  { id: 'overview',     label: '1. Tổng quan AWS AI',     file: 'AI-AWS/01-overview.html' },
  { id: 'bedrock',      label: '2. Amazon Bedrock',       file: 'AI-AWS/02-bedrock.html' },
  { id: 'knowledge',    label: '3. Knowledge Bases (RAG)', file: 'AI-AWS/03-knowledgebase.html' },
  { id: 'agents',       label: '4. Bedrock Agents',       file: 'AI-AWS/04-agents.html' },
  { id: 'guardrails',   label: '5. Guardrails & Safety',  file: 'AI-AWS/05-guardrails.html' },
  { id: 'sagemaker',    label: '6. SageMaker',            file: 'AI-AWS/06-sagemaker.html' },
  { id: 'amazonq',      label: '7. Amazon Q',             file: 'AI-AWS/07-amazonq.html' },
  { id: 'aiservices',   label: '8. AI Services quản lý',  file: 'AI-AWS/08-aiservices.html' },
  { id: 'vectordb',     label: '9. Embeddings & Vector',  file: 'AI-AWS/09-vectordb.html' },
  { id: 'integration',  label: '10. Ví dụ tích hợp',      file: 'AI-AWS/10-integration.html' },
  { id: 'security',     label: '11. Bảo mật & Vận hành',  file: 'AI-AWS/11-security.html' },
  { id: 'summary',      label: '12. So sánh & Tổng kết',  file: 'AI-AWS/12-summary.html' },
  { id: 'history',      label: '📋 Lịch sử',              file: 'AI-AWS/13-history.html' }
];
