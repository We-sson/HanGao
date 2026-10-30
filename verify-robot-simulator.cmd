@echo off
setlocal
cd /d "%~dp0"
if not exist verify-logs mkdir verify-logs
set BUILD_LOG=verify-logs\build.log
set TEST_LOG=verify-logs\tests.log

echo ==== %DATE% %TIME% ==== > "%BUILD_LOG%"
dotnet --info >> "%BUILD_LOG%" 2>&1
echo. >> "%BUILD_LOG%"
echo === dotnet build RobotSimulator.AnyCAD === >> "%BUILD_LOG%"
dotnet build "RobotSimulator.AnyCAD\RobotSimulator.AnyCAD.csproj" -c Debug -p:Platform=x64 -v minimal >> "%BUILD_LOG%" 2>&1
set BUILD_EXIT=%ERRORLEVEL%
echo BUILD_EXIT=%BUILD_EXIT% >> "%BUILD_LOG%"

echo ==== %DATE% %TIME% ==== > "%TEST_LOG%"
echo === dotnet run RobotSimulator.AnyCAD.Tests === >> "%TEST_LOG%"
dotnet run --project "RobotSimulator.AnyCAD.Tests\RobotSimulator.AnyCAD.Tests.csproj" -c Debug -p:Platform=x64 >> "%TEST_LOG%" 2>&1
set TEST_EXIT=%ERRORLEVEL%
echo TEST_EXIT=%TEST_EXIT% >> "%TEST_LOG%"

echo.
echo ------------------ BUILD LOG ------------------
type "%BUILD_LOG%"
echo.
echo ------------------ TEST LOG -------------------
type "%TEST_LOG%"
echo.
echo Logs written to: %CD%\verify-logs
echo BUILD_EXIT=%BUILD_EXIT%  TEST_EXIT=%TEST_EXIT%
pause
