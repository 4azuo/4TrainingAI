# Mở report CHƯA DONE cũ nhất để review.
#
# Report chưa done = thư mục còn nằm trong reports/new/ (bấm Done sẽ chuyển sang reports/done/).
# Cũ nhất = <yyyyMMdd-HHmmss> nhỏ nhất (tên là timestamp -> sort tăng dần lấy đầu tiên).
#
# Việc làm:
#   1. Tìm thư mục con cũ nhất trong reports/new/ (không có -> thoát êm).
#   2. Đảm bảo report-server.py đang chạy (cổng 8770) để nút Done hoạt động;
#      chưa chạy thì tự khởi động nền.
#   3. Mở report.html của lần chạy đó bằng trình duyệt mặc định (file://).
#
# Cách dùng:
#   powershell -File scripts/15-open-uncheck-report.ps1
#
# Chạy được dưới Windows PowerShell 5.1 lẫn PowerShell 7 (pwsh).
# Quy ước: console log (Write-Host) tiếng Anh; comment tiếng Việt.

$ErrorActionPreference = 'Stop'
# Hiển thị tiếng Việt đúng trên console (UTF-8)
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}

$scripts = $PSScriptRoot
$root    = Split-Path -Parent $scripts
$newDir  = Join-Path $root "reports\new"

# ---------------- 1. Tìm report chưa done cũ nhất ----------------
if (-not (Test-Path $newDir)) {
    Write-Host "==> No pending report: $newDir does not exist." -ForegroundColor Yellow
    return
}
$uncheck = Get-ChildItem -Path $newDir -Directory -ErrorAction SilentlyContinue |
          Sort-Object Name | Select-Object -First 1
if (-not $uncheck) {
    Write-Host "==> No pending report under reports/new (all done?)." -ForegroundColor Yellow
    return
}
$reportHtml = Join-Path $uncheck.FullName "report.html"
if (-not (Test-Path $reportHtml)) {
    Write-Host "==> Uncheck pending run '$($uncheck.Name)' has no report.html: $reportHtml" -ForegroundColor Yellow
    return
}

# ---------------- 2. Đảm bảo report-server.py đang chạy ----------------
# Kiểm tra cổng 8770 bằng cách thử kết nối loopback (chạy được trên cả PS 5.1 lẫn PS 7).
function Test-PortInUse([int]$port) {
    $client = New-Object System.Net.Sockets.TcpClient
    try {
        $iar = $client.BeginConnect('127.0.0.1', $port, $null, $null)
        # Chờ tối đa 300ms; kịp kết nối -> server đang chạy.
        if ($iar.AsyncWaitHandle.WaitOne(300) -and $client.Connected) { return $true }
        return $false
    } catch {
        return $false
    } finally {
        $client.Close()
    }
}

if (Test-PortInUse 8770) {
    Write-Host "==> report-server already running on port 8770." -ForegroundColor DarkGray
} else {
    $serverPy = Join-Path $scripts "report-server.py"
    if (Test-Path $serverPy) {
        # Khởi động nền, cửa sổ ẩn; server tự thoát êm nếu cổng đã bận.
        Start-Process -FilePath "python" -ArgumentList @($serverPy) `
            -WorkingDirectory $root -WindowStyle Hidden | Out-Null
        Write-Host "==> Started report-server.py (background) so the Done button works." -ForegroundColor DarkGray
    } else {
        Write-Host "==> report-server.py not found; Done button may not work: $serverPy" -ForegroundColor Yellow
    }
}

# ---------------- 3. Mở report ----------------
Write-Host "==> Opening uncheck pending report: $($uncheck.Name)" -ForegroundColor Green
Write-Host "    $reportHtml" -ForegroundColor Green
Start-Process $reportHtml
