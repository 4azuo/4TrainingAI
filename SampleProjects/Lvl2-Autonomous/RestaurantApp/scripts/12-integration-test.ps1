# Chạy lại Integration Test (tests/IT) và sinh evidence MỚI NHẤT vào tests/IT/evidence.
#
# Xoá sạch evidence cũ trước khi chạy (giữ lại README.md mô tả thư mục).
# Output: tests/IT/evidence/<TC>/output.log + before.png/after.png, kèm SUMMARY.md
#
# Chạy được dưới Windows PowerShell 5.1 lẫn PowerShell 7 (pwsh).
$ErrorActionPreference = 'Stop'
# Hiển thị tiếng Việt đúng trên console (UTF-8)
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}
$root        = Split-Path -Parent $PSScriptRoot
$ps          = (Get-Process -Id $PID).Path     # host hiện tại, thay cho 'pwsh' hard-code
$evidenceDir = Join-Path $root "tests\IT\evidence"

# Dọn evidence cũ nhưng giữ README.md
if (Test-Path $evidenceDir) {
    Get-ChildItem -Path $evidenceDir -Force |
        Where-Object { $_.Name -ne 'README.md' } |
        Remove-Item -Recurse -Force
} else {
    New-Item -ItemType Directory -Force -Path $evidenceDir | Out-Null
}

Write-Host "==> Running integration tests (tests/IT)..." -ForegroundColor Cyan
$tool = Join-Path $root "tests\IT\tools\Run-IntegrationTests.ps1"
& $ps -File $tool -TestCase ALL -EvidenceDir $evidenceDir
$itExit = $LASTEXITCODE
if ($itExit -ne 0) { Write-Host "    Some integration testcases FAILED (continuing)." -ForegroundColor Yellow }

Write-Host "==> Integration test evidence ready: $evidenceDir" -ForegroundColor Green
exit $itExit
