@echo off
powershell -noprofile -executionPolicy RemoteSigned -file "%~dp0\build\Retrieve-NativeFdb.ps1"
powershell -noprofile -executionPolicy RemoteSigned -file "%~dp0\build\build.ps1"
