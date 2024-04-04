@echo off
REM This line prevents echoing of commands to the console
REM run the game 

pip install pyrealsense2
pip install mediapipe
pip install numpy
pip install cv2
pip install datetime
pip install socket

start "" "handshot tower defence.exe"


REM Pause for 11 seconds
timeout /t 11

REM Run your Python script
python "4 hands.py"

REM Pause to keep the command prompt window open (optional)
pause
 