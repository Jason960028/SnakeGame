// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// Author:    Jason Chang
// Partner:   Soyoun Kim
// Date:      Nov 8 2024
// Course:    CS 3500, University of Utah, School of Computing
// Copyright: CS 3500 and Jason Chang - This work may not
//            be copied for use in Academic Coursework.
//
// I, Jason Chang, certify that I wrote this code from scratch and
// did not copy it in part or whole from another source.  All
// references used in the completion of the assignments are cited
// in my README file.
//
// File Contents:
// - This file defines a simple chat server designed to handle client connections
//   and relay messages between connected clients.
// - Manages client connections, allowing multiple clients to connect and communicate with each other.
// - Main method sets up the server, initializing the logger and starting the server on a
//   designated port.
// - HandleConnect method handles the communication with each client, reading incoming
//   messages and broadcasting them to all connected clients.
namespace CS3500.Chatting;

using CS3500.Networking;
using Microsoft.Extensions.Logging;

/// <summary>
///   A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    /// <summary>
    /// Instance used for logging server activity and events.
    /// </summary>
    private static ILogger logger = null!;

    /// <summary>
    /// A static list that holds all active <see cref="NetworkConnection"/> objects representing
    /// connected clients.
    /// </summary>
    private static List<NetworkConnection> connections = new();

    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    private static void Main(string[] args)
    {
        // Set up the logger with console and debug output, and set the logging level to Trace.
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Trace);
        });

        logger = loggerFactory.CreateLogger<ChatServer>();
        logger.LogInformation("Server starting...");
        Server.StartServer(HandleConnect, 11_000);
        Console.Read(); // don't stop the program.
    }

    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    /// <param name="connection">representing the user connection.</param>
    /// <exception cref="Exception">
    /// Thrown if an unexpected error occurs while reading from or writing to the connection.
    /// </exception>
    private static void HandleConnect(NetworkConnection connection)
    {
        lock (connections)
        {
            connections.Add(connection);
            logger.LogInformation("Connected to client.");
        }

        try
        {
            while (true)
            {
                var message = connection.ReadLine();
                logger.LogDebug("received message from client.");

                // Create a copy of the connections list to safely iterate over
                List<NetworkConnection> copiedConnections = [];
                lock (connections)
                {
                    copiedConnections.AddRange(connections);
                }

                // Broadcast the message to all clients
                foreach (var connection_ in copiedConnections)
                {
                    connection_.Send(message);
                }

                logger.LogDebug("sent message to clients.");
            }
        }
        catch (Exception)
        {
            lock (connections)
            {
                connections.Remove(connection);
            }

            logger.LogInformation("Disconnected with the client.");
        }
    }
}