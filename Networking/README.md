```
Author:     Jason Chang
Partner:    Soyoun Kim
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  Jason960028, soyouninutah
Repo:       https://github.com/uofu-cs3500-20-fall2024/assignment-eight-chatting-javascript.git
Date:       08-Nov-2024 Time: 23:59 (when submission was completed) 
Project:    Networking
Copyright:  CS 3500, Jason Chang, Soyoun Kim - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:

    PROJECT INFORMATION

    This project implements a simple networking layer for managing TCP connections in C#. 

    It includes two primary classes:
        1. NetworkConnection: A class that provides interface for managing TCP network connections,
           including the establishment of connections, sending and receiving messages, and properly 
           closing connections.
        2. Server: A static class manages a TCP server. The server listens for incoming connections 
           on a specified port and asynchronously handles new connections using a callback mechanism.
    
    CLASS FEATURES
    
    NetworkConnection Class
        - Connect: connect to the given host:port.
        - IsConnected: To check if the connection is active.
        - Send: Sends a message to the connected server.
        - ReadLine: Reads a line of text from the connected server.
        - Disconnect: Closes the connection and cleans up any related components.
        - Dispose: Ensures that components are properly released.

    Server Class
        - StartServer: Starts a TCP server on a specified port and handles incoming connections asynchronously.
          The handleConnect delegate specifies the behavior when a new connection is established.
        
    
	
# Assignment Specific Topics

    - Use of TcpClient and TcpListener classes for establishing and managing TCP connections.
    - Using Task.Run and asynchronous methods to handle multiple connection simultaneously.
    - Using the IDisposable interface to ensure resources are properly released.

# SoftWare Practice

    - Implementing IDisposable for the NetworkConnection class and properly releasing resources,
      such as network streams and sockets, ensures that resources are not leaked.

    - Using the Dispose pattern and handling cleanup efficiently.

    - The use of asynchronous methods, such as AcceptTcpClientAsync and Task.Run, helps manage multiple client connections.

    - The separation of responsibilities into classes makes the code easier to maintain, extend, and test.
	
# Consulted Peers:

	N/A

# References:
    Asynchronous programming with async and await
        https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
    TcpListener Class
        https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=net-8.0
    
    

	