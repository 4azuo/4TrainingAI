# Build toàn bộ solution RestaurantApp (độc lập với thư mục hiện tại).
$ErrorActionPreference = 'Stop'
# Hiển thị tiếng Việt đúng trên console (UTF-8)
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}
$root = Split-Path -Parent $PSScriptRoot

Write-Host "==> Restore & build RestaurantApp.sln" -ForegroundColor Cyan
dotnet build (Join-Path $root "RestaurantApp.sln") -c Debug
if ($LASTEXITCODE -ne 0) { throw "Build failed (exit $LASTEXITCODE)." }

Write-Host "==> Build done." -ForegroundColor Green
