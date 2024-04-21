![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/48de47e6-1e2d-4d99-a8f2-e748f087275e)

This project is about making a game that works using intel realsense depth camera , made by me Saadi Saadi and my partner Ali Haj under the supervision of Ohad Meir one of the heads of Intel Realsense team
the game is a tower defence archery game , where the player is an archer on top of a castle that shoots targets and enemies , you can use either keyboard or just your hands using intel realsense depth camera to track them
the game is made with unity and c# , and hand detection and tracking is made in python - using libraris such as pyrealsense2 , mediapipe , cv2 ... , and there is a connection between the unity application and the python application using sockets

if you want to change/modefy the project you can download Unity from https://unity.com/download, and download "Unity Project" folder from here and extract it , and open it using "Unity Hub" (this would be downloaded when downloading Unity) by clicking the open button and choosing the directory

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/fc688c29-69e3-4e5c-9398-76eb923d7bb1)

Unity Hub may ask you to download a specific Unity version that the project can run on

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/4cd862a5-173c-486c-a5cf-dea98bc7147a)

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/434aa2da-c3a2-4dbe-be56-0ee58a7fdbbf)

that way you can change in the project and play the game using keyboard , but if you have a realsense camera you can run unity (that way c# server would start lestining for connection) after that connect the camera and run one of the python files that you can take from "Python Files" directory (that way python client would connect to the server and start sending data that would control the game)

if you want just to play the game download "Game Build" folder from here , extract it and run one of the 4 run files
![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/d2448601-5058-4fce-8969-d8b2a8272539)

for the first time run a file that would download the python libraries , and from second time and on use one of the files that run the game instantly
