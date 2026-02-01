# Сборка NexusAI и создание установщика (Inno Setup)
# Требуется: .NET 8 SDK, опционально Inno Setup (для .exe установщика)

$ErrorActionPreference = "Stop"
$ProjectRoot = $PSScriptRoot
$PublishPath = Join-Path $ProjectRoot "bin\Release\net8.0-windows\win-x64\publish"

Write-Host "=== NexusAI: publish ===" -ForegroundColor Cyan
Push-Location $ProjectRoot
try {
    dotnet publish NexusAI.sln -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    if (Test-Path $PublishPath) {
        Write-Host "Publish OK: $PublishPath" -ForegroundColor Green
        # Запуск приложения для теста (опционально)
        $exe = Join-Path $PublishPath "NexusAI.exe"
        if (Test-Path $exe) {
            Write-Host "Запуск приложения для теста..." -ForegroundColor Cyan
            Start-Process -FilePath $exe -WorkingDirectory $PublishPath
        }
    }

    # Inno Setup — если iscc в PATH
    $iscc = Get-Command iscc -ErrorAction SilentlyContinue
    if ($iscc) {
        Write-Host "=== Сборка установщика (Inno Setup) ===" -ForegroundColor Cyan
        $issPath = Join-Path $ProjectRoot "installer\NexusAI.iss"
        & iscc $issPath
        if ($LASTEXITCODE -eq 0) {
            $outDir = Join-Path $ProjectRoot "installer-output"
            Get-ChildItem $outDir -Filter "*.exe" -ErrorAction SilentlyContinue | ForEach-Object {
                Write-Host "Установщик: $($_.FullName)" -ForegroundColor Green
            }
        }
    }
    else {
        Write-Host "Inno Setup не найден (iscc). Установщик не собран. Запуск из: $PublishPath" -ForegroundColor Yellow
    }
}
finally {
    Pop-Location
}
