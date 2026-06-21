# Hook: PreToolUse (Edit|Write)
# Chặn Claude ghi đè các vùng nhạy cảm (secret, file build sinh ra).
# Trả exit code 2 + thông báo ra STDERR => Claude Code sẽ HUỶ thao tác.

$ErrorActionPreference = 'SilentlyContinue'

$raw = [Console]::In.ReadToEnd()
if (-not $raw) { exit 0 }

try { $payload = $raw | ConvertFrom-Json } catch { exit 0 }

$path = $payload.tool_input.file_path
if (-not $path) { exit 0 }

# Các mẫu đường dẫn cấm sửa
$blocked = @('*\.env', '*secrets*', '*.pfx', '*\bin\*', '*\obj\*')

foreach ($pattern in $blocked) {
    if ($path -like $pattern) {
        [Console]::Error.WriteLine("[hook] CHAN: khong duoc ghi vao vung bao ve -> $path")
        exit 2
    }
}

exit 0
