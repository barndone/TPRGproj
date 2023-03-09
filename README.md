# Tactics RPG Project  

## About

This project is a part of a class assessment for building out an NPC for a game or simulation. In this case, the project is implementing two different pathfinding algorithms and a decision making algorithm to build an NPC opponent for a turn-based Tactics RPG. This opponent will control a party of units with different behaviors depending on their class.

For the purposes of the assessment, this project has implemented:
>The A* Pathfinding Algorithm (calculates the actual path the unit takes)  
>Dijkstra's (Modified to show the accessible tiles of a unit)  
>Decision Trees (controls how the AI will move their party)  

This project will support any version of **Unity 2020.3** or newer.


> :warning: **WARNING**  
> This project is a work in progress, as such some features may not be complete or *incredibly* janky!  

## Importing the Project  

You can either download a zip file of the project through the repository, or you can clone the repository with this URL:  
```text
https://github.com/barndone/TPRGproj.git
```

If you chose to download the zip file of the project, be sure to extract it to be able to open the project in unity.

## Opening the Project in Unity

To open the project in unity, after unzipping or cloning this project:  
* 1# Open the Unity Hub  
* 2# Press the 'ADD' button  
* 3# Navigate to the location of your ***TRPGproj folder***  
* 4# Select the ***PathfindingProj folder*** and press 'select folder'  

From there, you will be able to launch the project!

## Running the Game in-engine  

If you would like to run the game in the engine, navigate to the scenes folder in the project window and then open the Menu scene. This will allow you to play through the full gameplay loop without creating a build:

![image](https://user-images.githubusercontent.com/63937648/224168571-da4cc764-3ad9-4d2f-b4b8-5977f72db051.png)  

After the Menu scene is open, just press play at the top of the inspector!


## Building the Project  

> :warning: **WARNING**  
> This project has only been tested and designed for use on the PC, Mac & Linux Standalone Platform.  

To build the project, got to File and select Build Settings, check that the Platform is set to PC, Mac & Linux Standalone, and then press Build or Build and Run.

You will now have a standalone application for this project!

## License

This work is licensed under the **MIT License**. See [LICENSE.md](LICENSE.md) for details.

Copyright 2023 (c) Brandon Eiler
