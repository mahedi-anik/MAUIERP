# run-tests.ps1
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Running MAUI ERP Tests" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

Write-Host "`n1. Running Unit Tests..." -ForegroundColor Yellow
dotnet test tests/MAUIERP.UnitTests --verbosity normal

Write-Host "`n2. Running Integration Tests..." -ForegroundColor Yellow
dotnet test tests/MAUIERP.IntegrationTests --verbosity normal

Write-Host "`n=========================================" -ForegroundColor Green
Write-Host "All Tests Completed!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green