# PowerShell CI Script for TransportTycoon
# Mirrors the steps in .gitlab-ci.yml

# Set environment variables (use defaults if not set)
$BUILD_CONFIGURATION = 'Release'
$ROOT = './TransportTycoon'
$SOLUTION_PATH = "$($ROOT)/TransportTycoon.slnx"
$MODEL_PATH = "$($ROOT)/TransportTycoon.Model"
$TEST_PATH = "$($ROOT)/TransportTycoon.Test"

# Write-Host "--- Lint: Checking code formatting ---"
# dotnet --version
# Write-Host "Checking code formatting against .editorconfig on Windows..."
# dotnet format $ROOT --verify-no-changes -v diag

Write-Host "--- Test: Model ---"
dotnet restore $TEST_PATH --packages .nuget
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet test --solution $SOLUTION_PATH --no-restore --report-spekt-junit "MethodFormat=Class;FailureBodyFormat=Verbose" -- --coverage --coverage-output-format cobertura --coverage-output ./coverage.cobertura.xml --results-directory ./TestResults
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# # Code coverage extraction (ReportGenerator must be installed)
# if (-not (Test-Path "./reportgenerator.exe")) {
#     dotnet tool install --tool-path . dotnet-reportgenerator-globaltool
# }

# # Merge coverage reports
# $coverageFiles = Get-ChildItem -Path "$env:TEST_PATH/TestResults" -Recurse -Filter "coverage.cobertura.xml" | Select-Object -ExpandProperty FullName
# if ($coverageFiles) {
#     ./reportgenerator "-reports:$env:TEST_PATH/TestResults/*/coverage.cobertura.xml" "-targetdir:report" "-reporttypes:Cobertura"
#     # Extract line-rate from Cobertura.xml
#     $cobertura = Get-Content ./report/Cobertura.xml | Select-Object -First 3 | Out-String
#     if ($cobertura -match 'line-rate="([0-9.]+)"') {
#         $lineRate = [double]$matches[1]
#         $coverage = $lineRate * 100
#         Write-Host ("TOTAL_COVERAGE={0:N2}" -f $coverage)
#     } else {
#         Write-Host "Could not extract coverage from Cobertura.xml"
#     }
# } else {
#     Write-Host "No coverage.cobertura.xml files found."
# }

# Write-Host "--- CI Script Complete ---"
