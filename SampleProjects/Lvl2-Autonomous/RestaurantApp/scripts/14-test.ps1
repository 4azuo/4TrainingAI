# Orchestrator: chạy TOÀN BỘ pipeline test trong một lệnh.
#   01-build → 11-unit-test → 12-integration-test → 13-generate-report
#
# UT/IT có thể FAIL mà vẫn tiếp tục để sinh report; chỉ build hỏng mới dừng.
#
# Cách dùng:
#   powershell -File scripts/14-test.ps1
#   powershell -File scripts/14-test.ps1 -Title "Sprint 1"
#
# Chạy được dưới Windows PowerShell 5.1 lẫn PowerShell 7 (pwsh).
param(
    [string]$Title = "Báo cáo Test RestaurantApp"
)

$ErrorActionPreference = 'Stop'
# Hiển thị tiếng Việt đúng trên console (UTF-8) — áp cho cả output của script con
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}
$scripts = $PSScriptRoot
$root    = Split-Path -Parent $scripts
$ps      = (Get-Process -Id $PID).Path     # host hiện tại, thay cho 'pwsh' hard-code

Write-Host "==> [1/4] Build" -ForegroundColor Cyan
& $ps -File (Join-Path $scripts "01-build.ps1")
if ($LASTEXITCODE -ne 0) { throw "Build failed — aborting pipeline." }

Write-Host "==> [2/4] Unit test" -ForegroundColor Cyan
& $ps -File (Join-Path $scripts "11-unit-test.ps1")

Write-Host "==> [3/4] Integration test" -ForegroundColor Cyan
& $ps -File (Join-Path $scripts "12-integration-test.ps1")

Write-Host "==> [4/4] Generate report" -ForegroundColor Cyan
& $ps -File (Join-Path $scripts "13-generate-report.ps1") -Title $Title

# Tìm thư mục report mới nhất để in đường dẫn
$latest = Get-ChildItem -Path (Join-Path $root "reports\new") -Directory -ErrorAction SilentlyContinue |
          Sort-Object Name -Descending | Select-Object -First 1
Write-Host ""
if ($latest) {
    Write-Host "==> Done. Report: $(Join-Path $latest.FullName 'report.html')" -ForegroundColor Green
} else {
    Write-Host "==> Done, but no report folder found under reports/new." -ForegroundColor Yellow
}
