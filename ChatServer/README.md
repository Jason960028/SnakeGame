```
Author:     Jason Chang
Partner:    Soyoun Kim
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Jason960028, soyouninutah
Repo:       https://github.com/uofu-cs3500-20-fall2024/assignment-eight-chatting-javascript.git
Date:       08-Nov-2024 Time: 23:59 (when submission was completed) 
Project:    ChatServer
Copyright:  CS 3500, Jason Chang, Soyoun Kim - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:

    PROJECT INFORMATION

    This project is designed as a simple, multi-client chat server which primarily done in the ChatServer class.

    ChatServer CLASS FEATURES
    
        - Main Method: Starts the server on port 11_000, using the HandleConnect method to manage client interactions
         and Initializes a logging factory to log to both console and debug output.
        
        - HandleConnect Method: 
            * Adds a new NetworkConnection to connections when a client connects.
            * Enters a loop to continuously receive messages from the connected client.
            * Each message is read and then broadcasted to all active connections.
            * Removes the connection if an exception occurs, expecially when a client disconnects.

# Assignment Specific Topics

    - Logging:
        * Logs are essential for monitoring and debugging. Logging is set up in the Main method
          using LoggerFactory.Create, outputting to the console and debug with a minimum log level of Trace.
        * Log Levels:
            Information: Logs major events, such as server start and client connections.
            Debug: Captures detailed message.
            Error: Records unexpected failures.
    - Client-Server Communication:
        * Client connections are stored in a thread-safe list. 
          Messages from one client are broadcasted to all others efficiently.
    - Concurrency and Thread Safety: 
        * Locks are used to prevent race conditions when managing connections.

# SoftWare Practice

    - Use of ILogger provides a structured way to log events and monitor server activity.

    - The use of lock statements to synchronize access to the connections to prevent race conditions.

    - Creating a copy of the connections list before broadcasting messages helps prevent potential issues
      if the original list is modified during the message broadcast.

# Consulted Peers:

	N/A

# References:

	Logging in C# and .NET
        https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line
    
    The lock statement - ensure exclusive access to a shared resource
        https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock