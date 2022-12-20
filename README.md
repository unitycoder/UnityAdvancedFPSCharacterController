# Advanced First Person Shooter Character Controller
An open-source project focusing on creating the best lightweight yet feature-rich non-Rigidbody first person shooter character controller for Unity3D. Feel free to use it as a template for your project, but crediting this repo would be really appreciated!

Other than having the purpose of giving a great starting point/template for creating a character controller, this repository also provides a good way to structure a Unity project using Assembly Definitions. (All the more reason for one to actually check this Unity project instead of just importing the scripts to somewhere else!)

<br>

## Disclaimer
- There are portions of code here that I have [taken from tutorials and resources](#credits). Even though they are free to use I still want to clarify this.

<br>

## Features
- **Fully Commented**, just in case one finds something to be quite confusing. (Hopefully not 😊)
- **Control Settings**, you can select between toggling and holding crouch, inverting mouse Y axis for camera rotation, and more!
- Mostly uses **Event-based Input**, this project utilizes Unity's New Input System and InputAction.CallbackContext events.
- **Player Audio**, this repo comes with a bonus in-house script that manages player footsteps and jump sound effects.
- **Slope Movement**, bump-free and fully functional!
- **Smooth Crouching**, make crouching buttery smooth and not a jittery mess like the old CS 1.6 days.
- **Movement Smoothing**, since we're using the built-in Character Controller component as a base, we have to simulate the feeling of inertia and friction ourselves.
- **Camera Side Movement Tilting**, like the one in Half-Life and Quake.
- **Camera Headbobbing**, a must-have feature for realistic shooters and horror games!
- **Camera Controller Smoothing**, an adjustable float value that can be used to make the camera controller more cinematic.

<br>

## Key Notes
- **Prefabs** are located _at Assets\Project\Runtime\Prefabs\_.
- **In order for Player Audio to work**, make sure to add the following tags if they aren't already added:
  - _Material/Wood_
  - _Material/Stone_
  - _Material/Grass_
  - _Material/Metal_

<br>

## Sponsorship
I'd really appreciate it if someone were to donate me some cash. I am an aspiring software and game developer that currently do stuff solo, and I need funding to motivate me to do a lot better on my tasks so that I could deliver way better content.

<br>

## Contribution
Something's wrong with the code or you know better workarounds and alternatives? You can make a pull request. It will be very much appreciated!

<br>

## Credits
- This project would not have been possible without the help of wonderful people at [Samyam](https://www.youtube.com/@samyam)'s [Discord Server](https://discord.com/invite/B9bjMxj)
- [Event-based input](https://www.youtube.com/watch?v=8Yih0p2Kvy0&t=3s), [Camera headbobbing](https://www.youtube.com/watch?v=5MbR2qJK8Tc&t=1s), Smooth crouching, and [Slope movement](https://www.youtube.com/watch?v=GI5LAbP5slE) are implemented and modified from [Hero 3D](https://www.youtube.com/@hero3d899).
