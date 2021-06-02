# Snowblower Simulator 2k21
Created by Pascal, Julien and Robin a semester project for the class "Visualization, Simulation and Interaction - Virtual Reality I"

![](https://github.com/PapediPoo/Snowplow/blob/master/Images/snowblower_banner.png)

## What is it about?
In this game it is your job to clear a parking lot of as much snow as fast as possible. Both speed and cleanliness are important for getting a good score. So get to work!

## Important
Since we did not have access to VR-hardware (HMD and hand trackers), we had to try and EMULATE the VR experience. 
So although this project was build for for VR systems, you can't actually play it using a VR headset, but use Mouse and Keyboard instead.

## Downloading and starting the game
In this repo you will find the project files as well as a built version of the project
The build Windows version can be found [HERE](https://github.com/PapediPoo/Snowplow/tree/master/Build)

## How to play
Note that in the settings you can change between VR-mode and normal mode. This also changes the control scheme of the game. Simply toggle the setting in the options menu to switch between modes

<img src=https://github.com/PapediPoo/Snowplow/blob/master/Images/optionsmenu.png width=300>

### Normal controls
<img src=https://github.com/PapediPoo/Snowplow/blob/master/Images/normalmodecontrol.png width=300>
* **Move mouse**: Look around
* **W/S**: Move snowblower forwards/backwards
* **A/D**: Turn snowblower left/right
* **Q**: Toggle drive clutch
* **E**: Toggle auger clutch
* **Esc**: Back to menu

### VR-mode controls
<img src=https://github.com/PapediPoo/Snowplow/blob/master/Images/snowplowVRbuttons.png width=300>
We use the standard control scheme for Unity's XR Device Simulator
* **Left mouse button**:  Interact
* **Right mouse button**: Move camera
* **G**: Grip
* **Left shift**: Move left hand
* **Spacebar**: Move right hand
* **Left ctrl**: Rotate
* **Move mouse**: move current selection up/down/left/right
* **Scroll mouse wheel**: Move current selection inwards outwards

If you are in VR-mode, you can interact with the dashboard of the snowblower. This is also how you can control the machine. There are a total of **6 buttons** that you can interact with.

**Grabbing and interacting** with speed up/speed down buttons allows you to change the driving speed of the snowblower.

<img src=https://github.com/PapediPoo/Snowplow/blob/master/Images/speeddownVR.png height=150>  <img src=https://github.com/PapediPoo/Snowplow/blob/master/Images/speedupVR.png height=150>

The left and right control handles allow you to turn left and right. This only works if you are actually moving.

<img src=https://github.com/PapediPoo/Snowplow/blob/master/Images/LRcontrolVR.png height=150>

The auger and drive clutch buttons allow you to toggle the respective parts of the snowblower

Note: The auger of a snowblower is the spinning drum at the front of the machine. Turning it off will disable snow throwing capabilities.

<img src=https://github.com/PapediPoo/Snowplow/blob/master/Images/augerclutchVR.png height=150> <img src=https://github.com/PapediPoo/Snowplow/blob/master/Images/driveclutchVR.png height=150>
