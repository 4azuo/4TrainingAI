# Tool tự động chạy Integration Test và chụp evidence cho TỪNG testcase.
#
# Với mỗi testcase trong testcases.json:
#   1. Gọi harness (chạy luồng thật qua đủ tầng) với id testcase.
#   2. Harness tự kiểm chứng + ghi evidence (output.log + before.png/after.png) vào evidence/<id>/.
#   3. Tool gom kết quả thành evidence/SUMMARY.md và đặt exit code.
#
# Cách dùng (chạy được dưới powershell 5.1 hoặc pwsh 7):
#   powershell -File tests/IT/tools/Run-IntegrationTests.ps1                     # chạy tất cả
#   powershell -File tests/IT/tools/Run-IntegrationTests.ps1 -TestCase TC-003
#   powershell -File tests/IT/tools/Run-IntegrationTests.ps1 -EvidenceDir <dir>  # đổi nơi lưu evidence

param(
    [string]$TestCase = "ALL",
    [string]$EvidenceDir = ""
)

$ErrorActionPreference = 'Stop'
# Hiển thị tiếng Việt đúng trên console (UTF-8)
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}

$itRoot   = Split-Path -Parent $PSScriptRoot            # .../tests/IT
$harness  = Join-Path $itRoot "harness"
$evidence = if ([string]::IsNullOrWhiteSpace($EvidenceDir)) { Join-Path $itRoot "evidence" } else { $EvidenceDir }
$manifest = Join-Path $itRoot "testcases\testcases.json"

if (-not $TestCase) { $TestCase = "ALL" }
New-Item -ItemType Directory -Force -Path $evidence | Out-Null
$evidence = (Resolve-Path $evidence).Path

Write-Host "==> Build harness IT" -ForegroundColor Cyan
dotnet build $harness -c Debug | Out-Null

# Đọc UTF-8 để tên testcase tiếng Việt không bị lỗi font khi in ra console
$cases = Get-Content -Raw -Encoding UTF8 $manifest | ConvertFrom-Json
if ($TestCase -ne "ALL") {
    $cases = $cases | Where-Object { $_.id -eq $TestCase }
    if (-not $cases) { throw "Không tìm thấy testcase '$TestCase' trong manifest." }
}

$results = @()
foreach ($c in $cases) {
    Write-Host "==> $($c.id): $($c.title)" -ForegroundColor Cyan
    & dotnet run --project $harness -c Debug -- $c.id $evidence
    $results += [pscustomobject]@{
        Id    = $c.id
        Title = $c.title
        Pass  = ($LASTEXITCODE -eq 0)
    }
}

# ----- Tổng hợp SUMMARY.md -----
$lines = @(
    "# Integration Test — Tổng hợp",
    "",
    "Chạy lúc: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')",
    ""
)
foreach ($r in $results) {
    $status = if ($r.Pass) { "PASS ✅" } else { "FAIL ❌" }
    $lines += "- **$($r.Id)** $($r.Title): $status — evidence: ``$($r.Id)/`` (output.log, before.png, after.png)"
}

New-Item -ItemType Directory -Force -Path $evidence | Out-Null
$summaryPath = Join-Path $evidence "SUMMARY.md"
$lines | Out-File -Encoding utf8 $summaryPath
Write-Host "==> Tổng hợp: $summaryPath" -ForegroundColor Green

$failCount = ($results | Where-Object { -not $_.Pass }).Count
if ($failCount -gt 0) {
    Write-Host "==> $failCount testcase FAIL" -ForegroundColor Red
    exit 1
}
Write-Host "==> Tất cả testcase PASS" -ForegroundColor Green
exit 0
