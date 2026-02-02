# Nexus AI - Release Build Script
# Version: 1.0.0

$ErrorActionPreference = "Stop"

Write-Host "🚀 Nexus AI - Release Build Script" -ForegroundColor Cyan
Write-Host "===================================" -ForegroundColor Cyan
Write-Host ""

# Read version
$version = Get-Content -Path "VERSION" -Raw
$version = $version.Trim()
Write-Host "📦 Version: $version" -ForegroundColor Green

# Clean previous builds
Write-Host ""
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean --configuration Release
if (Test-Path "bin\Release") { Remove-Item -Recurse -Force "bin\Release" }
if (Test-Path "publish") { Remove-Item -Recurse -Force "publish" }

# Build Release
Write-Host ""
Write-Host "🔨 Building Release configuration..." -ForegroundColor Yellow
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

# Publish self-contained single-file executable
Write-Host ""
Write-Host "📦 Publishing self-contained executable (win-x64)..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -o "publish\win-x64"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Publish failed!" -ForegroundColor Red
    exit 1
}

# Copy documentation
Write-Host ""
Write-Host "📄 Copying documentation..." -ForegroundColor Yellow
Copy-Item "README.md" -Destination "publish\win-x64\"
Copy-Item "LICENSE" -Destination "publish\win-x64\"
Copy-Item "RELEASE_NOTES.md" -Destination "publish\win-x64\"
Copy-Item "VERSION" -Destination "publish\win-x64\"

# Create ZIP archive
Write-Host ""
Write-Host "🗜️  Creating ZIP archive..." -ForegroundColor Yellow
$zipName = "NexusAI-v$version-win-x64.zip"
if (Test-Path $zipName) { Remove-Item -Force $zipName }

Compress-Archive -Path "publish\win-x64\*" -DestinationPath $zipName -CompressionLevel Optimal

Write-Host ""
Write-Host "✅ Build completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "📦 Output:" -ForegroundColor Cyan
Write-Host "   - Folder: publish\win-x64\" -ForegroundColor White
Write-Host "   - Archive: $zipName" -ForegroundColor White
Write-Host ""
Write-Host "📊 Package size:" -ForegroundColor Cyan
$size = (Get-Item $zipName).Length / 1MB
Write-Host "   $([math]::Round($size, 2)) MB" -ForegroundColor White

Write-Host ""
Write-Host "🎉 Ready for distribution!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Test the executable: publish\win-x64\NexusAI.exe" -ForegroundColor White
Write-Host "2. Create GitHub release with tag v$version" -ForegroundColor White
Write-Host "3. Upload $zipName as release asset" -ForegroundColor White
Write-Host ""
