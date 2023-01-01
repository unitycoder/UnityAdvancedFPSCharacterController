# Advanced First Person Shooter Character Controller
An open-source project focused on creating the best lightweight yet feature-rich non-Rigidbody first person shooter character controller for Unity3D. Feel free to use it as a template for your project, but crediting this repo would be really appreciated!

Other than having the purpose of giving a great starting point/template for implementing a character controller to make your game actually playable, this repository also provides a good way to structure a Unity project using Assembly Definitions. (All the more reason for one to actually check this Unity project instead of just importing the scripts to somewhere else!)

<br>

## Before You Use
**Disclaimer** - There are portions of code here that I have [taken from tutorials and resources](#credits). Even though they are free to use I still want to clarify that I use assets and resources that may be outside of my field of expertise at the moment because I am constantly learning something new everyday.<br>

Before you use this repo as a template in any means, this project is currently being actively developed and many things are subject to change. Kindly star the repo before forking or cloning, please!

<br>

## Table of Contents
This README will be pretty lengthy in the near future. So, if you want to skip over some stuff or just want to cut straight to the chase, this one's for you!

| Table of Contents               |
| ------------------------------- |
| [Features](#features)           |
| [Key Notes](#key-notes)         |
| [Sponsorship](#sponsorship)     |
| [Contribution](#contribution)   |
| [Credits](#credits)             |

<br>

## Upcoming Features and Updates
This repo now had its **first** stable release, but there will be more as I refactor old and unhealthy code to make better iterations of existing features, and introduce completely new ones!
### Settings UI
Change controls settings in runtime without re-building the project with new changes made to the Player GameObject's Inpsector! It also comes with a working save system.
### Mobile Integration
This repo will soon have support for mobile controls!
### Others
- Possible revamp of how Player Input is being managed in preparation for the upcoming mobile controls.
- New and better slope detection and movement!
- Save System with Binary Reader/Writer and JSON examples.

<br>

## Features
### Fully Commented
All the essential scripts in this project are fully commented to a certain degree, this is to make sure that you learn how the code works as you inspect it.
### Settings and Preferences
This character controller comes with adjustable values and changeable control types that may suit your needs. You can select between toggling or holding crouch, inverting the camera controller's mouse input Y axis, and more!
### Uses Event-based Input
This project utilizes Unity's New Input System and its InputAction.CallbackContext events, making event-based player input possible. This feature improves the performance of the game loop because all the inputs are edge-triggered and does not run on every frame.
### Player Audio
This project comes with a bonus in-house script that manages player footsteps and jump sound effects.
### Coyote Time
This feature makes jumping off edges easier.
### Slope Movement
A character controller will not be fully-functional if it doesn't have slope movement! This makes sure that going up or down on slanted surfaces will be seamless and would not cause any bumpy movement.
### Smooth Crouching
This feature makes crouching a smoother experience and not a jittery mess that simply just snaps the controller's transform/collider into tall and short heights and vice versa.
### Movement Smoothing
Since we're using the built-in Character Controller component as a base, we have to simulate the feeling of inertia and friction ourselves unlike in Rigidbodies where it happens without extra coding.
### Camera Headbobbing
Camera Headbobbing is separated into two sub-features that we'll call move bobbing and land bobbing. **Move Bobbing** is when the camera noticeably shakes in a pre-set pattern that makes walking and running more immersive. **Land Bobbing** is when the camera indents itself downwards as the controller lands from a fall, giving that satisfying <i>oomph</i> that adds up to the immersion. Both of these features are good for realistic shooters and horror games!
### Camera Side-Movement Tilting
This tilts the camera towards the direction relative to the X axis of the controller's movement input. It gives off an effect similar to games like Quake and Half-Life.
### Camera Controller Smoothing
This project provides an adjustable float value that can be used to make the camera controller more cinematic when toned down.

<br>

## Key Notes
- This project **uses Cinemachine**. (Never really intended to, but I just didn't remove it lol)
- **Prefabs** are located at <i>Assets\Project\Runtime\Prefabs\.</i>
- **In order for Player Audio to work**, make sure you have tags that correspond with the footstep sound effects you're trying to add. For example:
  - _Material/Wood_
  - _Material/Stone_
  - _Material/Grass_
  - _Material/Metal_<br>
  My implementation of footsteps requires an array of strings (your tags) found at MaterialManager.cs and multiple AudioClip arrays inside an array (yeah, I did not stutter, arrays in an array). **More info at PlayerAudio.cs, but here's a rundown - Make sure the indexes of the AudioClip arrays match the indexes of the strings at MaterialManager. (eg materialTags[0] = Material/Wood, then footsteps[0].audioClip should have wooden footstep sound effects.**

<br>

## Sponsorship
I'd really appreciate it if someone were to donate me some cash. I am an aspiring software and game developer that currently do stuff solo, and I need funding to motivate me to do a lot better on my tasks so that I could deliver way better content.

<br>

## Contribution
Something's wrong with the code or you know better workarounds and alternatives? You can make a pull request. It will be very much appreciated!

<br>

## Credits
- This project would not have been possible without the help of wonderful people at [Samyam](https://www.youtube.com/@samyam)'s [Discord Server](https://discord.com/invite/B9bjMxj)!
- [Event-based input](https://www.youtube.com/watch?v=8Yih0p2Kvy0&t=3s), [Camera headbobbing](https://www.youtube.com/watch?v=5MbR2qJK8Tc&t=1s), Smooth crouching, and [Slope movement](https://www.youtube.com/watch?v=GI5LAbP5slE) are implemented and modified from [Hero 3D](https://www.youtube.com/@hero3d899).
