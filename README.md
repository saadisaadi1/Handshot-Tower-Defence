![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/48de47e6-1e2d-4d99-a8f2-e748f087275e)

### **Introduction**
This project is about making a game that works using intel realsense depth camera , made by me Saadi Saadi and my partner Ali Haj under the supervision of Ohad Meir one of the heads of Intel Realsense team
the game is a tower defence archery game shooting targets and enemies , you can use either keyboard or just your hands using intel realsense depth camera to track them
the game is made with unity and c# , and hand detection and tracking is made in python - using libraris such as pyrealsense2 , mediapipe , cv2 ... , and there is a connection between the unity application and the python application using sockets

### **modefying The Project** 
If you want to change/modefy the project you can download Unity from https://unity.com/download, and download "Unity Project" folder from here and extract it , and open it using "Unity Hub" (this would be downloaded when downloading Unity) by clicking the open button and choosing the directory

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/fc688c29-69e3-4e5c-9398-76eb923d7bb1)

Unity Hub may ask you to download a specific Unity version that the project can run on

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/4cd862a5-173c-486c-a5cf-dea98bc7147a)

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/434aa2da-c3a2-4dbe-be56-0ee58a7fdbbf)

that way you can change in the project and play the game using keyboard , but if you have a realsense camera you can run unity (that way c# server would start lestining for connection) after that connect the camera and run one of the python files that you can take from "Python Files" directory (that way python client would connect to the server and start sending data that would control the game)

### **Just Playing** 
If you want just to play the game download "Game Build" folder from here , extract it and run one of this 2 run files
![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/4c803abb-cbcc-48e9-b59d-8836fb2366e9)

### **About The Game**
this game is an archery tower defence game , you are an archer on top of a castle your job is to defend the castle , and finesh the 9 waves before dying , after each wave the game gives you 3 abilities to choose one of them , most of the abilities are perminant that update the arrows for example the bouncing ability make your arrow jump , there are two abilities that you need to do specific actions to apply them : when choosing spikes ability , you need to put the spikes on the groud , and if you choose the special attack ability , you can use it once in each wave and make alot of arrows fall from the sky, you can control the game either with the keyboard or with intel realsense depth camera 

by default you are set to control  the game with the camera , but you can switch between camera and keyboard by pressing the control button

### **Game Buttons**
![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/380b1fdf-2897-47bb-a5eb-05ed4948c55c)

the first button is for exiting the game
the second button is for enabling and disabling music
the third button is for enabling and disabling sound effects
the fourth button is for switching between keyboard and Intel Realsense camera
the fifth button is only relevant if you are using the camera, "R" means your right hand is the stronger hand , "L" means that your left hand is the stronger hand
in this game the strong hand is the one that shoots the arrows and the weak hand holds the bow

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/9e14faca-d74d-41b8-8011-bd27b38a6140)

this button is only relevant if you are using the camera , it switches between 3 modes:
1) RGB mode , this mode is the default mode and it's the best when you are playing with the camera , because it also shows the landmarks of the hands and fingers
2) Depth ColorMap mode , this mode shows the dipth of every thing in the image , things are more blue when they are too close to the camera , and more red the more they are far
3) No image Mode , disables the image window this is useful when playing with keyboard because then it's irrelevant
   
### **Playing With KeyBoard**
to control with keyboard use down and up arrows for changing bow’s angle
left and right arrows for moving left and right
"Space" or "Z" for firing an arrow , hold "Space" or "Z" to control arrows power.
use moust to choose abiltiy at the end of each wave
when choosing the spikes ability use the four arrows to place the spikes and press space after that , to put it down
if you have chosen special ability , you can press Z to trigger it

as said before when playing with keyboard it's recommended to disable the image window

### **Playing With Intel Realsense Camera**

first of all , you need to know that when using the camera to play you need to know that all actions are done using the positions of the hands and fingers so the depth and open fingers count is depending on that , so you need to keep your hands in a certain way that the palm of the hands is facing the camera and fingers are above like in this picture

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/21e0ace8-71a9-42c1-bc62-3cb2dfcf3b47)

here are some good and bad examples 

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/c3afb857-a599-4927-94f2-1818c14a03b5)

to change bow's angle use the left hand by moving it up and down
to move left and right move both left and right hands, the middle point between them is that specify where you are in the game, keep your hands close to each other
to fire an arrow: 
1) keep your left hand closed all the time
2) close your right hand to catch an arrow
3) then bring it closer and away from you to specify the power
4) then open it to set the arrow free
   
to choose an abitly at the end of each wave:
1) keep your right hand behind your left hand (further from the camera than left)
2) choose the abilty that you want with the left hand (same as changing bow's angle by moving the hand up and down) a pointer appears next the ability that you are pointing to
3) after pointing to a specific ability keep your left hand where it is , and bring your right hand closer to the camera (closer than left), that way you choose the ability that you pointed to in step 2)
   
if you chose spikes abilty:
1) move left and right with your hands to choose where to put the spikes in the x-axis
2) keep your left where it is and bring your right hand closer to the camera and further to choose where to put the spikes in the z-axis
3) close your left hand and show one finger in the right hand to put it on the ground that you chosen in the previous steps

if you chose special abilty and want to trigger it:
1) show 4 fingers in each hand to trigger that attack
2) move left and right with your hands to choose where to put the target in the x-axis
3) keep your left where it is and bring your right hand closer to the camera and further to choose where to put the target in the z-axis

as said before it's recommended to keep RGB mode in the image window 
and if your hand is stronger than right hand , you can click the button that changes the stronger hand , and then in all the instructions that we talked about in this section , flip right hand and left hand (for example right hand changes the bow's angle...)
### **Gameplay Video**
https://drive.google.com/file/d/1ZdyQh6gC1k_02LOzpg1VJQxVGKxIiWim/view?usp=sharing
