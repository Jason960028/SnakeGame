```
Author:     Jason Chang
Partner:    Soyoun Kim
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Jason960028, soyouninutah
Repo:       https://github.com/uofu-cs3500-20-fall2024/assignment-eight-chatting-javascript.git
Date:       08-Nov-2024 Time: 23:59 (when submission was completed) 
Project:    ChatClient.Client
Copyright:  CS 3500, Jason Chang, Soyoun Kim - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:

    PROJECT INFORMATION
    
    The Chat Client project is a Blazor-based application that enables real-time communication over a network connection.
    The client features an intuitive UI for sending and receiving messages, and it differentiates between messages sent
    by the user and those received from others.

    The UI layout includes:
        -This file contains a Blazor component written in C# and Razor syntax,
         defining a Chat Client page with an interactive UI for sending and 
         receiving messages over a network connection.
        -Includes field for user to enter their user name and message and 
         button to connect and disconnect to the server.
        -A display area for showing received and sent chat messages, formatted to
         differentiate between messages sent by the user and those received from others.

    The @code block includes:
        -Methods to handle connecting and disconnecting from the server,
        message input updates, and sending messages.
        -Method to determine if a message was sent by the current user,
        handle text input and key press events.

# Assignment Specific Topics (Additional Functionalities)
    
    - Client displays log on both debug window and the console.

    - Client receives the user id on the top side, allowing users to write their id when sending a message.

    - Shows chat message on the right side if it is sent by the same id, and shows it on the left side otherwise.

    - The chat shows up delightful speech bubbles to life in the UI, adding charm to every message.

# SoftWare Practice

    - Use of InvokeAsync to update the UI from another thread is crucial for Blazor's UI rendering.

    - HandleSendMessageKeyPress method formats the message with a timestamp and sender ID before sending.

    - ILogger is used to log events and errors.

# Consulted Peers:

	N/A

# References:
    Logging in C# and .NET
        https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line
	