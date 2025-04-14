```
- Author:     Jason Chang
- Partner:    Soyoun Kim
- Start Date: Nov 24 2024
- Course:     CS3500 Software Practice
- GitHub ID:  Jason960028, soyouninutah
- Repo:       https://github.com/uofu-cs3500-20-fall2024/assignment-eight-chatting-javascript.git
- Commit Date: 05-Dec-2024 Time: 23:59 (when submission is ready to be evaluated)
- Solution: Chatting
- Copyright:  CS 3500, Jason Chang, Soyoun Kim - This work may not be copied for use in Academic Coursework.
```

# Overview of Chatting functionality
	
	- Chatting
	1. Server
	the server controls receiving the message and sending the message through NetworkConntection Class.
	Sends a message to each client as it got a message from a client.

	2. Client
	Client deals with the user input and interface. User can connect to the server or not with the button on top of it. It gets the ID and Name of the user and sends a message to the server with corresponding information. User interface shows the message from the same id on the right side, and message from different id on the left side.

	Server and Client writes their Log on the debug and Console output.

	- Snake
	Projects for the snake game.
	Now the snake project only has functionality for the client part.
	User can connect to the server or not with the button on top of it.
	The client writes its log on the debug and Console output.
	Snake.Client class draws the world on web page with Canvas library.
	It aslo sends the keyboard input to the server.
	Also, it updates the information of snake world on the ssms server in real-time

	- Webserver
	Webserver represents the information of past snake games.
	This project reads game information from ssms server,
	represent it into html,
	and sends it via TCP protocol(Networking class).


# Time Expenditures:
	
	1. Assignment One        Predicted Hours: 7       Actual Hours: 5
	2. Assignment Two        Predicted Hours: 8       Actual Hours: 7
	3. Assignment Three      Predicted Hours: 10      Actual Hours: 9
	4. Assignment Four       Predicted Hours: 10      Actual Hours: 7
	5. Assignment Five       Predicted Hours: 10      Actual Hours: 10
	6. Assignment Six        Predicted Hours: 10      Actual Hours: 10
	7. Assignment Seven      Predicted Hours: 12      Actual Hours: 15
    8. Assignment Eight      Predicted Hours: 10      Actual Hours: 9
	9. Assignment Nine       Predicted Hours: 15      Actual Hours: 15
	10. Assignment Ten       Predicted Hours: 12      Actual Hours: 12


# Reflection for the assignment Ten
	Had hard time identifying connection with SQL server.

# Contribution
	Soyoun Kim(50%):
		- Implemented WebServer Class.
		- Helped debugging SQL connection and data storage process in SnakeClient Class.

	Jason Chang(50%):
		- Implemented SQL connection in SnakeClient Class.
		- Added CSS Features on Webserver HTML.

# Reference for Assignment 10
	ChatGPT for understanding the operation of SQLConnector.
    https://www.khanacademy.org/computing/computers-and-internet/xcae6f4a7ff015e7d:the-internet/xcae6f4a7ff015e7d:web-protocols/a/hypertext-transfer-protocol-http
