# Hook: PostToolUse (Edit|Write)
# Tự chạy `dotnet format` trên file C# vừa được Claude sửa, để code luôn đúng convention.
# Claude Code truyền dữ liệu hook qua STDIN dạng JSON.

$ErrorActionPreference = 'SilentlyContinue'

$raw = [Console]::In.ReadToEnd()
if (-not $raw) { exit 0 }

try { $payload = $raw | ConvertFrom-Json } catch { exit 0 }

# Đường dẫn file vừa bị tác động
$path = $payload.tool_input.file_path
if (-not $path) { exit 0 }

# Chỉ format file C#
if ($path -notlike '*.cs') { exit 0 }

Write-Host "[hook] format-on-save: dotnet format -> $path"
dotnet format RestaurantApp.sln --include $path 2>$null

exit 0
