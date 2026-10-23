$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $root

$port = 5080

Write-Host ""
Write-Host "  API Web Northwind" -ForegroundColor Cyan
Write-Host "  ----------------------------------------" -ForegroundColor DarkGray

# Si quedó una instancia anterior de la API ocupando el puerto, se cierra.
$listeners = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
foreach ($listener in @($listeners)) {
    if (-not $listener) { continue }
    $proc = Get-Process -Id $listener.OwningProcess -ErrorAction SilentlyContinue
    if ($proc -and $proc.ProcessName -eq 'APIWeb') {
        Write-Host "  Cerrando instancia anterior de APIWeb (PID $($proc.Id))..." -ForegroundColor Yellow
        Stop-Process -Id $proc.Id -Force
    }
}

Start-Sleep -Milliseconds 700

$stillBusy = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
if ($stillBusy) {
    $pidOcupado = ($stillBusy | Select-Object -First 1).OwningProcess
    $proc = Get-Process -Id $pidOcupado -ErrorAction SilentlyContinue
    Write-Host "  El puerto $port está ocupado por '$($proc.ProcessName)' (PID $pidOcupado)." -ForegroundColor Red
    Write-Host "  Ciérralo o cambia el puerto en iniciar-api.ps1." -ForegroundColor Red
    Read-Host "  Presiona Enter para salir"
    exit 1
}

Write-Host "  URL:      http://localhost:$port/" -ForegroundColor Green
Write-Host "  Swagger:  http://localhost:$port/swagger" -ForegroundColor Green
Write-Host "  Dashboard:http://localhost:$port/dashboard.html" -ForegroundColor Green
Write-Host "  (Ctrl+C para detener la API)" -ForegroundColor DarkGray
Write-Host ""

dotnet run --project "$root\APIWeb10\APIWeb" --urls "http://localhost:$port"

Write-Host ""
Write-Host "  La API se detuvo." -ForegroundColor Yellow
