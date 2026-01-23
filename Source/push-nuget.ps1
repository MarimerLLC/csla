# Push all NuGet packages to nuget.org
$packagesPath = "S:\src\rdl\csla\bin\packages"

# Prompt for NuGet API key
$apiKey = Read-Host -Prompt "Enter your NuGet API key" -AsSecureString
$apiKeyPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($apiKey))

# Get all .nupkg files (exclude .snupkg symbol packages)
$packages = Get-ChildItem -Path $packagesPath -Filter "*.nupkg" | Where-Object { $_.Name -notlike "*.snupkg" }

if ($packages.Count -eq 0) {
    Write-Host "No .nupkg files found in $packagesPath" -ForegroundColor Yellow
    exit
}

Write-Host "Found $($packages.Count) package(s) to push:" -ForegroundColor Cyan
$packages | ForEach-Object { Write-Host "  - $($_.Name)" }
Write-Host ""

foreach ($package in $packages) {
    Write-Host "Pushing $($package.Name)..." -ForegroundColor Cyan
    dotnet nuget push $package.FullName --api-key $apiKeyPlain --source https://api.nuget.org/v3/index.json --skip-duplicate
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  Success!" -ForegroundColor Green
    } else {
        Write-Host "  Failed to push $($package.Name)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Green
