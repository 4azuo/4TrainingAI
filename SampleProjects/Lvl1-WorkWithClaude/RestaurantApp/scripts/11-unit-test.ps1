# Chạy lại Unit Test (tests/UT) và sinh kết quả MỚI NHẤT vào tests/UT/report.
#
# Xoá sạch thư mục report cũ trước khi chạy để không lẫn kết quả lần trước.
# Output: tests/UT/report/test-results.trx + tests/UT/report/coverage.cobertura.xml
#
# Chạy được dưới Windows PowerShell 5.1 lẫn PowerShell 7 (pwsh).
$ErrorActionPreference = 'Stop'
# Hiển thị tiếng Việt đúng trên console (UTF-8)
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}
$root      = Split-Path -Parent $PSScriptRoot
$reportDir = Join-Path $root "tests\UT\report"

# Dọn report cũ
if (Test-Path $reportDir) { Remove-Item -Recurse -Force $reportDir }
New-Item -ItemType Directory -Force -Path $reportDir | Out-Null

Push-Location $root
try {
    Write-Host "==> Running unit tests (tests/UT) + coverage..." -ForegroundColor Cyan
    dotnet test tests/UT `
        --logger "trx;LogFileName=test-results.trx" `
        --collect:"XPlat Code Coverage" `
        --results-directory $reportDir
    if ($LASTEXITCODE -ne 0) {
        Write-Host "    Some unit tests FAILED/SKIPPED (continuing)." -ForegroundColor Yellow
        # Nếu lỗi 'Application Control policy has blocked' (0x800711C7) hoặc bị skip toàn bộ:
        # Smart App Control đang chặn DLL test chưa ký. Tắt nó trong Windows Security mới chạy được.
        Write-Host "    Hint: if you see 0x800711C7 / 'Application Control policy has blocked', Smart App Control is blocking unsigned test DLLs." -ForegroundColor Yellow
        Write-Host "          Turn it off: Windows Security > App & browser control > Smart App Control > Off." -ForegroundColor Yellow
    }

    # coverlet ghi coverage.cobertura.xml vào thư mục con GUID → kéo bản mới nhất lên report/
    $cov = Get-ChildItem -Path $reportDir -Recurse -Filter "coverage.cobertura.xml" |
           Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($cov) {
        Move-Item -Force $cov.FullName (Join-Path $reportDir "coverage.cobertura.xml")
    } else {
        Write-Host "    coverage.cobertura.xml not found (coverage will be empty)." -ForegroundColor Yellow
    }
    # Xoá mọi thư mục con GUID coverlet còn sót (report/ chỉ giữ trx + coverage ở gốc)
    Get-ChildItem -Path $reportDir -Directory | Remove-Item -Recurse -Force
}
finally {
    Pop-Location
}

Write-Host "==> Unit test report ready: $reportDir" -ForegroundColor Green
