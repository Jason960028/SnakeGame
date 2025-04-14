// <copyright file="NetworkConnection.cs" company="UofU-CS3500">
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
// This file defines the NetworkConnection class, which provides
// a convenient wrapper for managing TCP network connections.
// The class handles creating, managing, and closing connections
// using StreamReader and StreamWriter for communication.
// Methods include Connect, Disconnect, Send, and ReadLine,
// along with properties to check if a connection is active.

namespace CS3500.Networking;

using System.Net.Sockets;
using System.Text;

/// <summary>
///   Wraps the StreamReader/Writer/TcpClient together so we
///   don't have to keep creating all three for network actions.
/// </summary>
public sealed class NetworkConnection : IDisposable
{
    /// <summary>
    ///   The connection/socket abstraction.
    /// </summary>
    private TcpClient tcpClient = new();

    /// <summary>
    ///   Reading end of the connection.
    /// </summary>
    private StreamReader? reader = null;

    /// <summary>
    ///   Writing end of the connection.
    /// </summary>
    private StreamWriter? writer = null;

    /// <summary>
    ///   Initializes a new instance of the <see cref="NetworkConnection"/> class.
    ///   <para>
    ///     Create a network connection object.
    ///   </para>
    /// </summary>
    /// <param name="tcpClient">
    ///   An already existing TcpClient.
    /// </param>
    public NetworkConnection(TcpClient tcpClient)
    {
        this.tcpClient = tcpClient;
        if (this.IsConnected)
        {
            // Only establish the reader/writer if the provided TcpClient is already connected.
            this.reader = new StreamReader(this.tcpClient.GetStream(), Encoding.UTF8);
            this.writer = new StreamWriter(tcpClient.GetStream(), new UTF8Encoding(false)) { AutoFlush = true }; // AutoFlush ensures data is sent immediately
        }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NetworkConnection"/> class.
    ///   <para>
    ///     Create a network connection object.  The tcpClient will be unconnected at the start.
    ///   </para>
    /// </summary>
    public NetworkConnection()
        : this(new TcpClient())
    {
    }

    /// <summary>
    /// Gets a value indicating whether the socket is connected.
    /// </summary>
    public bool IsConnected
    {
        get
        {
            if (this.tcpClient?.Client == null)
            {
                return false;
            }

            // Check if the socket is readable but has no data available, which indicates disconnection.
            // Also check if the underlying socket is still in a connected state.
            return !(this.tcpClient.Client.Poll(1, SelectMode.SelectRead) &&
                     this.tcpClient.Client.Available == 0) && this.tcpClient.Client.Connected;
        }
    }

    /// <summary>
    ///   Try to connect to the given host:port.
    /// </summary>
    /// <param name="host"> The URL or IP address, e.g., www.cs.utah.edu, or  127.0.0.1. </param>
    /// <param name="port"> The port, e.g., 11000. </param>
    public void Connect(string host, int port)
    {
        if (this.IsConnected)
        {
            throw new InvalidOperationException("Already connected.");
        }

        this.tcpClient.Connect(host, port);

        // Create a StreamReader and StreamWriter to handle network communication.
        this.reader = new StreamReader(this.tcpClient.GetStream(), Encoding.UTF8);
        this.writer = new StreamWriter(this.tcpClient.GetStream(), new UTF8Encoding(false)) { AutoFlush = true };
    }

    /// <summary>
    ///   Send a message to the remote server.  If the <paramref name="message"/> contains
    ///   new lines, these will be treated on the receiving side as multiple messages.
    ///   This method should attach a newline to the end of the <paramref name="message"/>
    ///   (by using WriteLine).
    ///   If this operation cannot be completed (e.g., because this NetworkConnection is not
    ///   connected), throw an InvalidOperationException.
    /// </summary>
    /// <param name="message"> The string of characters to send. </param>
    public void Send(string message)
    {
        if (!this.IsConnected || this.writer == null)
        {
            throw new InvalidOperationException("Not connected.");
        }

        this.writer.WriteLine(message);
    }

    /// <summary>
    ///   Read a message from the remote side of the connection.  The message will contain
    ///   all characters up to the first new line. See <see cref="Send"/>.
    ///   If this operation cannot be completed (e.g., because this NetworkConnection is not
    ///   connected), throw an InvalidOperationException.
    /// </summary>
    /// <returns> The contents of the message. </returns>
    public string ReadLine()
    {
        if (!this.IsConnected || this.reader == null)
        {
            throw new InvalidOperationException("Not connected.");
        }

        return this.reader.ReadLine() ?? string.Empty;
    }

    /// <summary>
    ///   If connected, disconnect the connection and clean
    ///   up (dispose) any streams.
    /// </summary>
    public void Disconnect()
    {
        if (this.IsConnected)
        {
            this.reader?.Dispose();
            this.writer?.Dispose();
            this.tcpClient.Close();

            this.tcpClient = new TcpClient();
            this.reader = null;
            this.writer = null;
        }
    }

    /// <summary>
    ///   Automatically called with a using statement (see IDisposable).
    /// </summary>
    public void Dispose()
    {
        this.Disconnect();
    }
}
