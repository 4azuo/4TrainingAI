# Gom kết quả test MỚI NHẤT thành một báo cáo HTML theo lần chạy.
#
# 1. Tạo thư mục reports/new/<yyyyMMdd-HHmmss>/
# 2. Copy tests/UT/report      -> reports/new/<ts>/ut
#    Copy tests/IT/evidence    -> reports/new/<ts>/it/evidence
# 3. Đọc trx + coverage + evidence để sinh report-data.js và copy asset HTML (skill test-report)
#
# Cách dùng:
#   powershell -File scripts/13-generate-report.ps1
#   powershell -File scripts/13-generate-report.ps1 -Title "Sprint 1"
#
# Chạy được dưới Windows PowerShell 5.1 lẫn PowerShell 7 (pwsh).

param(
    # Quy ước: console log (Write-Host) bằng tiếng Anh; comment tiếng Việt.
    # Phân tích testcase .md / output.log dùng anchor ASCII + VỊ TRÍ section (không khớp chuỗi
    # tiếng Việt) -> không vỡ khi đổi encoding/BOM. $Title là DỮ LIỆU (tiêu đề hiển thị report).
    [string]$Title = "RestaurantApp Test Report"
)

$ErrorActionPreference = 'Stop'
# Hiển thị tiếng Việt đúng trên console (UTF-8)
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}
$root  = Split-Path -Parent $PSScriptRoot
$skill = Join-Path $root ".claude\skills\test-report"

# Nguồn kết quả mới nhất
$srcUt  = Join-Path $root "tests\UT\report"
$srcIt  = Join-Path $root "tests\IT\evidence"

# ---------------- Tạo thư mục lần chạy + copy kết quả ----------------
$ts     = Get-Date -Format 'yyyyMMdd-HHmmss'
$RunDir = Join-Path $root (Join-Path "reports\new" $ts)
$utDir  = Join-Path $RunDir "ut"
$itEvid = Join-Path $RunDir "it\evidence"
New-Item -ItemType Directory -Force -Path $utDir, $itEvid | Out-Null

if (Test-Path $srcUt) {
    Copy-Item -Path (Join-Path $srcUt '*') -Destination $utDir -Recurse -Force
} else {
    Write-Host "    UT report not found: $srcUt (run 11-unit-test.ps1 first)" -ForegroundColor Yellow
}
if (Test-Path $srcIt) {
    # Copy evidence nhưng bỏ README.md mô tả thư mục
    Get-ChildItem -Path $srcIt -Force | Where-Object { $_.Name -ne 'README.md' } |
        ForEach-Object { Copy-Item -Path $_.FullName -Destination $itEvid -Recurse -Force }
} else {
    Write-Host "    IT evidence not found: $srcIt (run 12-integration-test.ps1 first)" -ForegroundColor Yellow
}

$trxPath  = Join-Path $utDir "test-results.trx"
$covPath  = Join-Path $utDir "coverage.cobertura.xml"

function Get-SimpleName([string]$dotted) {
    if (-not $dotted) { return "" }
    return ($dotted -split '\.')[-1]
}

# Deep-clone về kiểu thuần (ordered dict / mảng / scalar): cắt mọi tham chiếu dùng chung và
# bóc lớp PSObject. Windows PowerShell 5.1 báo "circular reference" / treo ConvertTo-Json khi
# graph có instance lặp lại; clone tạo instance mới ở mỗi nút nên serialize chạy ổn định.
function ConvertTo-Clean($o) {
    if ($null -eq $o) { return $null }
    if ($o -is [string]) { return [string]$o }
    if ($o -is [bool])   { return [bool]$o }
    if ($o -is [int] -or $o -is [long]) { return [long]$o }
    if ($o -is [double] -or $o -is [decimal] -or $o -is [single]) { return [double]$o }
    if ($o -is [System.Collections.IDictionary]) {
        $n = [ordered]@{}
        foreach ($k in $o.Keys) { $n[[string]$k] = ConvertTo-Clean $o[$k] }
        return $n
    }
    if ($o -is [System.Collections.IEnumerable]) {
        $a = New-Object System.Collections.ArrayList
        foreach ($e in $o) { [void]$a.Add((ConvertTo-Clean $e)) }
        return ,@($a.ToArray())
    }
    return [string]$o   # kiểu lạ → ép chuỗi
}

# ---------------- Coverage (cobertura) ----------------
$covByClass = @{}     # simpleName -> @{ file; pct; covered; valid }
$totalCovered = 0; $totalValid = 0
if (Test-Path $covPath) {
    [xml]$cx = Get-Content -Raw -Encoding UTF8 $covPath
    foreach ($cls in $cx.coverage.packages.package.classes.class) {
        $covered = 0; $valid = 0
        if ($cls.lines -and $cls.lines.line) {
            foreach ($ln in $cls.lines.line) {
                $valid++
                if ([int]$ln.hits -gt 0) { $covered++ }
            }
        }
        $totalCovered += $covered; $totalValid += $valid

        # chuẩn hoá đường dẫn file -> bắt đầu từ 'src/...'
        $file = [string]$cls.filename
        $file = $file -replace '\\', '/'
        $idx = $file.IndexOf('/src/')
        if ($idx -ge 0) { $file = $file.Substring($idx + 1) }
        elseif ($file -match '(^|/)(src/.*)$') { $file = $matches[2] }

        $simple = Get-SimpleName ([string]$cls.name)
        $pct = if ($valid -gt 0) { [math]::Round(100.0 * $covered / $valid, 1) } else { $null }
        if ($covByClass.ContainsKey($simple)) {
            $e = $covByClass[$simple]
            $e.covered += $covered; $e.valid += $valid
            $e.pct = if ($e.valid -gt 0) { [math]::Round(100.0 * $e.covered / $e.valid, 1) } else { $null }
        } else {
            $covByClass[$simple] = @{ file = $file; pct = $pct; covered = $covered; valid = $valid }
        }
    }
}
$totalCoverage = if ($totalValid -gt 0) { [math]::Round(100.0 * $totalCovered / $totalValid, 1) } else { $null }

# ---------------- UT results (trx) ----------------
$testsByClass = @{}    # testClassSimple -> List of @{ method; outcome; durationMs }
$utPassed = 0; $utFailed = 0; $utSkipped = 0; $utTotal = 0
if (Test-Path $trxPath) {
    [xml]$tx = Get-Content -Raw -Encoding UTF8 $trxPath
    $ns = New-Object System.Xml.XmlNamespaceManager($tx.NameTable)
    $ns.AddNamespace('t', 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010')

    # map testName -> className từ TestDefinitions
    $classByTest = @{}
    foreach ($ut in $tx.SelectNodes('//t:TestDefinitions/t:UnitTest', $ns)) {
        $tm = $ut.SelectSingleNode('t:TestMethod', $ns)
        if ($tm) { $classByTest[[string]$ut.name] = [string]$tm.className }
    }

    foreach ($r in $tx.SelectNodes('//t:Results/t:UnitTestResult', $ns)) {
        $utTotal++
        $name = [string]$r.testName
        $outcome = [string]$r.outcome
        switch ($outcome) {
            'Passed'    { $utPassed++ }
            'Failed'    { $utFailed++ }
            default     { $utSkipped++ }
        }
        $durMs = 0.0
        if ($r.duration) { try { $durMs = [math]::Round(([TimeSpan]::Parse([string]$r.duration)).TotalMilliseconds, 1) } catch {} }

        $className = $classByTest[$name]
        if (-not $className) { $className = ($name -replace '\.[^.]+$', '') }   # bỏ method ở cuối
        $clsSimple = Get-SimpleName $className
        $method = Get-SimpleName $name

        if (-not $testsByClass.ContainsKey($clsSimple)) { $testsByClass[$clsSimple] = New-Object System.Collections.ArrayList }
        [void]$testsByClass[$clsSimple].Add(@{ method = $method; outcome = $outcome; durationMs = $durMs })
    }
}

# ---------------- Gộp UT theo file src ----------------
# Mỗi test class <X>Tests ánh xạ tới class src <X> (vd MenuServiceTests -> MenuService).
$utFiles = New-Object System.Collections.ArrayList
$usedSrc = @{}
foreach ($clsSimple in ($testsByClass.Keys | Sort-Object)) {
    $srcSimple = $clsSimple -replace 'Tests$', ''
    $cov = $covByClass[$srcSimple]
    $srcFile = if ($cov) { $cov.file } else { "src/?/$srcSimple.cs" }
    $pct = if ($cov) { $cov.pct } else { $null }
    if ($cov) { $usedSrc[$srcSimple] = $true }

    $tlist = $testsByClass[$clsSimple]
    $p = ($tlist | Where-Object { $_.outcome -eq 'Passed' }).Count
    $f = ($tlist | Where-Object { $_.outcome -eq 'Failed' }).Count
    [void]$utFiles.Add([ordered]@{
        srcFile = $srcFile; testClass = $clsSimple; coverage = $pct
        linesCovered = $(if ($cov) { $cov.covered } else { $null })
        linesValid   = $(if ($cov) { $cov.valid } else { $null })
        passed = $p; failed = $f; count = $tlist.Count
        tests = @($tlist | ForEach-Object { [ordered]@{ method = $_.method; outcome = $_.outcome; durationMs = $_.durationMs } })
    })
}
# File src có coverage nhưng KHÔNG có UT (vd Views, interface) -> liệt kê riêng
$utUncovered = New-Object System.Collections.ArrayList
foreach ($srcSimple in ($covByClass.Keys | Sort-Object)) {
    if ($usedSrc.ContainsKey($srcSimple)) { continue }
    $cov = $covByClass[$srcSimple]
    [void]$utUncovered.Add([ordered]@{ srcFile = $cov.file; coverage = $cov.pct; linesCovered = $cov.covered; linesValid = $cov.valid })
}

# ---------------- IT evidence + testcases ----------------
# Lấy nội dung của section "## " thứ $index (0-based) trong file .md.
# Dùng theo VỊ TRÍ chứ không khớp tiêu đề tiếng Việt -> không phụ thuộc encoding/BOM/normalize,
# và không vỡ khi ai đó sửa chuỗi trong script. Mọi testcase .md có cùng thứ tự section:
#   [0] Tiền điều kiện · [1] Các bước · [2] Kết quả mong đợi · [3] Evidence.
function Get-SectionByIndex([string]$text, [int]$index) {
    $lines = $text -split "`r?`n"
    $out = New-Object System.Collections.ArrayList
    $cur = -1
    foreach ($l in $lines) {
        if ($l -match '^\#\#\s') {
            $cur++
            if ($cur -gt $index) { break }
            continue
        }
        if ($cur -eq $index) {
            $t = $l.Trim()
            if ($t) { [void]$out.Add(($t -replace '^\d+\.\s*', '' -replace '^[-*]\s*', '')) }
        }
    }
    return @($out)
}

$itScenarios = New-Object System.Collections.ArrayList
$itPassed = 0; $itFailed = 0; $itTotal = 0
$functions = New-Object System.Collections.ArrayList
$tcManifest = Join-Path $root "tests\IT\testcases\testcases.json"
if (Test-Path $tcManifest) {
    $cases = Get-Content -Raw -Encoding UTF8 $tcManifest | ConvertFrom-Json
    foreach ($c in $cases) {
        $id = [string]$c.id
        # KHÔNG đặt tên $title: PowerShell không phân biệt hoa thường nên sẽ đè tham số $Title.
        $tcTitle = [string]$c.title
        $func = ""; $steps = @(); $expected = @()
        $mdFile = Get-ChildItem -Path (Join-Path $root "tests\IT\testcases") -Filter "$id-*.md" -ErrorAction SilentlyContinue | Select-Object -First 1
        $scenarioFile = if ($mdFile) { "tests/IT/testcases/$($mdFile.Name)" } else { "" }
        if ($mdFile) {
            $md = Get-Content -Raw -Encoding UTF8 $mdFile.FullName
            # Tên chức năng = ô thứ 2 của HÀNG bảng in đậm ĐẦU TIÊN (| **...** | giá trị |).
            if ($md -match '(?m)^\|\s*\*\*[^*]+\*\*\s*\|\s*(.+?)\s*\|') { $func = $matches[1].Trim() }
            $steps    = Get-SectionByIndex $md 1   # section "Các bước"
            $expected = Get-SectionByIndex $md 2   # section "Kết quả mong đợi"
        }

        # evidence
        $result = "N/A"; $log = ""; $shotBefore = ""; $shotAfter = ""
        $evDir = Join-Path $itEvid $id
        $logPath = Join-Path $evDir "output.log"
        if (Test-Path $logPath) {
            $log = Get-Content -Raw -Encoding UTF8 $logPath
            # Khớp "<nhãn>: PASS/FAIL" qua dấu hai chấm -> không phụ thuộc nhãn tiếng Việt "Kết quả:".
            if ($log -match ':\s*(PASS|FAIL)') { $result = $matches[1] }
        }
        # Ảnh evidence trước/sau; fallback screenshot.png (report cũ chỉ có 1 ảnh -> coi như "sau").
        if (Test-Path (Join-Path $evDir "before.png")) { $shotBefore = "it/evidence/$id/before.png" }
        if (Test-Path (Join-Path $evDir "after.png"))  { $shotAfter  = "it/evidence/$id/after.png" }
        if (-not $shotAfter -and (Test-Path (Join-Path $evDir "screenshot.png"))) {
            $shotAfter = "it/evidence/$id/screenshot.png"
        }

        $itTotal++
        if ($result -eq 'PASS') { $itPassed++ } elseif ($result -eq 'FAIL') { $itFailed++ }
        if ($func -and -not $functions.Contains($func)) { [void]$functions.Add($func) }

        [void]$itScenarios.Add([ordered]@{
            scenarioFile = $scenarioFile
            cases = @([ordered]@{
                id = $id; title = $tcTitle; function = $func; result = $result
                steps = @($steps); expected = @($expected)
                log = $log; beforeScreenshot = $shotBefore; afterScreenshot = $shotAfter
            })
        })
    }
}

# ---------------- Build REPORT_DATA ----------------
$data = [ordered]@{
    meta = [ordered]@{
        title = $Title
        date  = (Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
        runId = Split-Path $RunDir -Leaf
    }
    ut = [ordered]@{
        total = $utTotal; passed = $utPassed; failed = $utFailed; skipped = $utSkipped
        totalCoverage = $totalCoverage
        linesCovered = $totalCovered; linesValid = $totalValid
        files = @($utFiles)
        uncovered = @($utUncovered)
    }
    it = [ordered]@{
        total = $itTotal; passed = $itPassed; failed = $itFailed
        functions = @($functions)
        scenarios = @($itScenarios)
    }
}

# Nhúng JSON (đã pretty-print, dễ đọc) vào report-data.js để report mở trực tiếp bằng file://.
$json = ConvertTo-Json (ConvertTo-Clean $data) -Depth 12
$jsContent = "window.REPORT_DATA = $json;"
$dataPath = Join-Path $RunDir "report-data.js"
# Ghi UTF8 không BOM để JS đọc chuẩn
[System.IO.File]::WriteAllText($dataPath, $jsContent, (New-Object System.Text.UTF8Encoding($false)))

# ---------------- Copy assets từ skill ----------------
foreach ($asset in @('report.html', 'report.css', 'report.js')) {
    $src = Join-Path $skill $asset
    if (-not (Test-Path $src)) { throw "Missing skill asset: $src" }
    Copy-Item -Force $src (Join-Path $RunDir $asset)
}

Write-Host "==> Report generated: $(Join-Path $RunDir 'report.html')" -ForegroundColor Green
Write-Host "    Open the file directly; the Done button needs report-server.py running (auto-started by VSCode)." -ForegroundColor Green
Write-Host "    UT: $utPassed/$utTotal pass, total coverage: $totalCoverage%   |   IT: $itPassed/$itTotal pass" -ForegroundColor Green
