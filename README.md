![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/48de47e6-1e2d-4d99-a8f2-e748f087275e)


### **Introduction**
This project is about creating a game that can be played with intel RealSense depth camera, made by me Saadi Saadi and my partner Ali Haj under the supervision of Ohad Meir one of the developers at Intel Realsense team.

The game is a tower defence archery game, firing arrows on targets and enemies, you can use either keyboard or just your hands using Intel RealSense depth camera to track them.

The game is made with Unity and c#, and hand detection and tracking is made in python - using libraris such as pyRealSense2, mediapipe, cv2 ..., and there is a connection between the Unity application and the python application using sockets.


### **Modifying The Project** 
If you want to change/modify the project you can download Unity from https://Unity.com/download, and download "Unity Project" folder from here and extract it, and open it using "Unity Hub" (this would be downloaded when downloading Unity) by clicking the open button and choosing the directory.

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/fc688c29-69e3-4e5c-9398-76eb923d7bb1)

Unity Hub may ask you to download a specific Unity version that the project can run on.

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/4cd862a5-173c-486c-a5cf-dea98bc7147a)

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/434aa2da-c3a2-4dbe-be56-0ee58a7fdbbf)

that way you can change in the project and play the game using keyboard, but if you have a RealSense camera you can run Unity (that way c# server would start lestining for connection) after that connect the camera and run one of the python files that you can take from "Python Files" directory (that way python client would connect to the server and start sending data that would control the game).


### **Just Playing** 
If you want just to play the game download "Game Build" folder from here, extract it and run one of this 2 run files.
![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/4c803abb-cbcc-48e9-b59d-8836fb2366e9)


### **About The Game**
This game is an archery tower defence game, you are an archer on top of a castle your job is to defend the castle, and finesh the 9 waves before dying. After each wave the game gives you 3 abilities to choose one of them. Most of the abilities are permanent that update the arrows, for example the bouncing ability make your arrow jump, there are two abilities that you need to do specific actions to apply them: when choosing spikes ability, you need to put the spikes on the groud, and if you choose the special attack ability, you can use it once in each wave and make alot of arrows fall from the sky, you can control the game either with the keyboard or with intel RealSense depth camera.

by default you are set to control  the game with the camera, but you can switch between camera and keyboard by pressing the control button.


### **Game Buttons**
![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/380b1fdf-2897-47bb-a5eb-05ed4948c55c)

The first button is for exiting the game.

The second button is for enabling and disabling music.

The third button is for enabling and disabling sound effects.

The fourth button is for switching between keyboard and Intel Realsense camera.

The fifth button is only relevant if you are using the camera, "R" means your right hand is the stronger hand, "L" means that your left hand is the stronger hand, in this game the strong hand is the one that shoots the arrows and the weak hand holds the bow.

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/9e14faca-d74d-41b8-8011-bd27b38a6140)

This button is only relevant if you are using the camera, it switches between 3 modes:
1) RGB mode: this mode is the default mode and it's the best when you are playing with the camera, because it also shows the landmarks of the hands and fingers
2) Depth ColorMap mode: this mode shows the depth of everything in the image. Objects are more blue when they are too closer to the camera, and more red the more they are far.
3) No Image mode: disables the image window. This is useful when playing with keyboard because then it's irrelevant.

   
### **Playing With KeyBoard**
To control with keyboard use down and up arrows for changing bow’s angle.

Left and right arrows for moving left and right.

"Space" or "Z" for firing an arrow, hold "Space" or "Z" to control arrows power.

Use mouse to choose abiltiy at the end of each wave.

When choosing the spikes ability use the four arrows to place the spikes and press space after that, to put it down.

If you have chosen special ability, you can press "X" to trigger it.

As said before when playing with keyboard it's recommended to disable the image window.


### **Playing With Intel Realsense Camera**

Firstly, when using the camera to play, you need to know that all actions are done using the positions of the hands and fingers so the depth and open fingers count is depending on that, so you need to keep your hands in a certain way that the palm of the hands is facing the camera and fingers are above like in this picture.

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/21e0ace8-71a9-42c1-bc62-3cb2dfcf3b47)

here are some good and bad examples:

![image](https://github.com/saadisaadi1/Handshot-Tower-Defence/assets/50622237/c3afb857-a599-4927-94f2-1818c14a03b5)

To change bow's angle use the left hand by moving it up and down.

To move left and right move both hands to the left and to the right, the middle point between them is that specify where you are in the game, keep your hands close to each other.

To fire an arrow: 
1) Keep your left hand closed all the time.
2) Close your right hand to catch an arrow.
3) Then bring it closer and away from you to specify the power.
4) Then open it to set the arrow free.
   
To choose an abitly at the end of each wave:
1) Keep your right hand behind your left hand (further from the camera than left).
2) Choose the abilty that you want with the left hand (same as changing bow's angle by moving the hand up and down) a pointer appears next the ability that you are pointing to.
3) After pointing to a specific ability keep your left hand where it is, and bring your right hand closer to the camera (closer than left), that way you choose the ability that you pointed to in step 2.
   
If you chose spikes abilty:
1) Move left and right with your hands to choose where to put the spikes in the x-axis.
2) Keep your left where it is and bring your right hand closer to the camera and further to choose where to put the spikes in the z-axis.
3) Close your left hand and show one finger in the right hand to put it on the ground that you chose in the previous steps.

If you chose special abilty and want to trigger it:
1) Show 4 fingers in each hand to trigger that attack.
2) Move left and right with your hands to choose where to put the target in the x-axis.
3) Keep your left where it is and bring your right hand closer to the camera and further to choose where to put the target in the z-axis.

As said before it's recommended to keep RGB mode in the image window.
If your left hand is stronger than right hand, you can click the button that changes the stronger hand, and then in all the instructions that we talked about in this section, flip right hand and left hand (for example right hand changes the bow's angle...).


### **Gameplay Video**
https://drive.google.com/file/d/1ZdyQh6gC1k_02LOzpg1VJQxVGKxIiWim/view?usp=sharing
