```
Author:     Jason Chang
Partner:    Soyoun Kim
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Jason960028, soyouninutah
Repo:       https://github.com/uofu-cs3500-20-fall2024/assignment-eight-chatting-javascript.git
Date:       24-Nov-2024 Time: 23:59
Project:    Snake.Client
Copyright:  CS 3500, Jason Chang, Soyoun Kim - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:

    PROJECT INFORMATION
    
    The SnakeClient project is a Blazor-based application that enables users to play snake game.
    It mainly contains Draw() function and Connect() function
    It receives JSON that represents components of the snake game world continuously
    It draws the world on Canvas every frame.
    It sends the keyboard input to the server every frame. (arrows and wasd control.)
    It shows the alert message and informations on left top side of the canvas.
    It shows the ranking of snakes on the right top of the canvas.

    (Added in assignment 10)
    It updates the game status on ssms server.
    Added additional Player, Game and SqlConnector class to send Queries on SQL server. 

# Assignment Specific Topics (Additional Functionalities)
    
    - Client displays log on both debug window and the console.

    - Users are able to connect to a server with certain address and port, and provide the username.

    - Draw the state of the world, as described to it by the server.

    - Added pulse effect for the power up object to make game more dynamic.

    - Changed layout of HUD so user can easily tack of game stats.

    - Added ranking system based on each snake's live score.

# SoftWare Practice

    - Use of InvokeAsync to update the UI from another thread is crucial for Blazor's UI rendering.

# Contribution

    Jason Chang(50%):
        - Built the basic project with the lab code.
		- Dealt with clipping the canvas.
		- Implemented ranking and fixed overall design of the game.
		- Implemented functions to send keyboard input.
		
    
    Soyoun Kim(50%): 
        - Wrote Logsupport class.
		- Dealt with the models part(Storing the classes from JSON and Representing them on the canvas).
		- Implemented Alert System.
		- Made some changes to keyboard input system.

# Consulted Peers:

	N/A

# References:
    Logging in C# and .NET
        https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line
    Codes described in specification for clipping.
    Used Lab11 code as a base code.
    Asked ChatGPT to understand the lab code function.
	