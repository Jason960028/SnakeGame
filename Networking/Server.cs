// <copyright file="Server.cs" company="UofU-CS3500">
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
// This file defines the Server class, provides functionality to start a TCP server.
// The server listens on a specified port, waits for incoming connections,
// and handle new connections asynchronously.
// The StartServer method manages this behavior by accepting TcpClient
// connections and invoking the provided callback for each connection.

namespace CS3500.Networking;

using System.Net;
using System.Net.Sockets;

/// <summary>
///   Represents a server task that waits for connections on a given
///   port and calls the provided delegate when a connection is made.
/// </summary>
public static class Server
{
    /// <summary>
    ///   Wait on a TcpListener for new connections. Alert the main program
    ///   via a callback (delegate) mechanism.
    /// </summary>
    /// <param name="handleConnect">
    ///   Handler for what the user wants to do when a connection is made.
    ///   This should be run asynchronously via a new thread.
    /// </param>
    /// <param name="port"> The port (e.g., 11000) to listen on. </param>
    /// <exception cref="SocketException">
    ///  Throw if a socket error occurs while listening for or accepting connections.
    /// </exception>
    /// <exception cref="Exception">
    ///  Throw if any other unexpected error occurs.
    /// </exception>
    public static void StartServer(Action<NetworkConnection> handleConnect, int port)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        // Run the server asynchronously in a separate task to avoid blocking the main thread
        _ = Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    // Asynchronously wait for an incoming client connection
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();

                    NetworkConnection networkConnection = new NetworkConnection(tcpClient);

                    // Start a new task to handle the connection, passing the networkConnection object to the callback
                    _ = Task.Run(() => handleConnect(networkConnection));
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                listener.Stop();
            }
        });
    }
}
