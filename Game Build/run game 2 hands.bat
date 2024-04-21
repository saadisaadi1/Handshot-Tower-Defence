@echo off

set "recommended_python_version=3.10.0"
set "recommended_pip_version=21.3.1"
set "python_version=0"
call :check_and_update_python
call :check_and_update_pip
call :check_and_install_package pyrealsense2
call :check_and_install_package mediapipe
call :check_and_install_package numpy
call :check_and_install_package opencv-python

REM Run the game executable
start "" "handshot tower defence.exe"

REM Pause for 11 seconds
timeout /t 10

REM Run your Python script
python "2 hands.py"

REM Pause to keep the command prompt window open (optional)
pause


REM Define the recommended versions


REM Function to check and update Python
:check_and_update_python
echo Checking if Python is installed
python --version > nul 2>&1
if %errorlevel% neq 0 (
    echo Python is not installed. Installing Python...
    REM Download and install Python
    powershell -Command "Invoke-WebRequest https://www.python.org/ftp/python/%recommended_python_version%/python-%recommended_python_version%-amd64.exe -OutFile python-installer.exe"
    python-installer.exe /quiet TargetDir=C:\Python
    set "PATH=%PATH%;C:\Python"
    echo Python has been installed.
) else (
	echo Python is already installed.
)
exit /b

REM Function to check and update pip
:check_and_update_pip
echo Checking if pip is installed
pip --version > nul 2>&1
if %errorlevel% neq 0 (
    echo pip is not installed. Installing pip...
    REM Download get-pip.py
    powershell -Command "Invoke-WebRequest https://bootstrap.pypa.io/get-pip.py -OutFile get-pip.py"
    python get-pip.py
    echo pip has been installed.
) else (
	echo pip is already installed..
)
exit /b

REM Function to check and install a Python package
:check_and_install_package
echo Checking if %~1 is installed...
pip show %1 > nul 2>&1
if %errorlevel% neq 0 (
    echo Installing %~1...
    pip install %~1
) else (
    echo %~1 is already installed.
)
exit /b