@echo off

dotnet build SqlProfiler.sln -c Release -p:DebugType=None -p:Platform="Any CPU"

if errorlevel 1 goto error

goto exit
:error
pause
:exit
