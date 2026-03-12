# Stop all dotnet processes to free ports before starting the AppHost.
# Run from PowerShell: .\stop-dotnet.ps1

Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force
Write-Host "All dotnet processes stopped. You can start the AppHost now."
