// <copyright file="WebServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// Author:    Jason Chang
// Partner:   Soyoun Kim
// Date:      Dec 4 2024
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
// Webserver object that is driven by HTTP protocol.
// Clients are able to connect to this server by localhost, port 80.

namespace CS3500.WebServer;

using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using CS3500.Networking;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

/// <summary>
/// WebServer class.
/// <list type="bullet">
/// <item>Retrieves information from SQL server</item>
/// <item>Gets request through networkconnection in form of HTTP protocol and parses it.</item>
/// <item>Generates HTML showing data from SQL server.</item>
/// <item>Sends response to Cleint through networkconnection with HTTP protocol.</item>
/// </list>
/// </summary>
public static class WebServer
{
    /// <summary>
    /// The information necessary for the program to connect to the Database.
    /// </summary>
    private static string? connectionString;

    private static IPAddress iP = IPAddress.Parse("127.0.0.1");

    private static int port = 11000;

    /// <summary>
    /// Main function.
    /// </summary>
    public static void Main()
    {
        UpdateConnectionString();

        // Start the web server
        Thread serverThread = new Thread(StartServer);
        serverThread.Start();
        Console.WriteLine("Web server is running. Press Enter to stop.");
        Console.ReadLine();
        Environment.Exit(0);
    }

    /// <summary>
    /// Method that is handling actual server work.
    /// </summary>
    private static void StartServer()
    {
        TcpListener listener = new TcpListener(iP, port);

        listener.Start();
        Console.WriteLine($"Server started on port {port}...");
        while (true)
        {
            var client = listener.AcceptTcpClient();
            Thread clientHandling = new Thread(async () => await HandleClientAsync(client));
            clientHandling.Start();
        }
    }

    /// <summary>
    /// Handles Client.
    /// </summary>
    /// <param name="client">TCPClient object represents the connection.</param>
    /// <returns>Task.</returns>
    private static async Task HandleClientAsync(TcpClient client)
    {
        NetworkConnection connection = new NetworkConnection(client);

        // Read the request and process it
        var request = connection.ReadLine();
        Console.WriteLine($"Received request: {request}");

        // Parse the request (very basic for now)
        var requestParts = request.Split(' ');

        if (requestParts.Length < 3)
        {
            Console.WriteLine("Invalid request");
            connection.Disconnect();
            return;
        }

        var method = requestParts[0]; // GET, POST, etc.
        var url = requestParts[1]; // The URL path
        var protocolVersion = requestParts[2]; // HTTP/1.1, etc.

        string content = await GetContentbyURLAsync(url);

        string response = GenerateHttpResponse(content);

        // Send the response
        if (connection.IsConnected)
        {
            connection.Send(response);
        }

        // Close the connection
        connection.Disconnect();
    }

    /// <summary>
    /// Helper function to get right content for the given url.
    /// </summary>
    /// <param name="url">url for the page.</param>
    /// <returns>html content as a string.</returns>
    private static async Task<string> GetContentbyURLAsync(string url)
    {
        if (url == null)
        {
            Console.WriteLine("Something went wrong.");
            return "Something went wrong";
        }
        else if (url == "/")
        {
            return GenerateHomePageHTML();
        }
        else if (url == "/games")
        {
            return await GenerateGamesHTML();
        }
        else
        {
            string pattern = @"^\/games\?gid=(\d+)$";
            Match match = Regex.Match(url, pattern);
            int gid;

            if (match.Success)
            {
                gid = int.Parse(match.Groups[1].Value);
                return await GenerateGameHTMLAsync(gid);
            }
            else
            {
                Console.WriteLine("invalid url");
                return "Wrong url";
            }
        }
    }

    /// <summary>
    /// Helper function to add header.
    /// </summary>
    /// <param name="content">content (html) of the page.</param>
    /// <returns>full HTTP response with header as a string.</returns>
    private static string GenerateHttpResponse(string content)
    {
        // A very basic response
        return "HTTP/1.1 200 OK\r\n" +
               "Content-Type: html\r\n" +
               $"Content-Length: {content.Length}\r\n" +
               "\r\n" +
               content;
    }

    /// <summary>
    /// Helper function that generates HTML for the Homepage.
    /// </summary>
    /// <returns>HTML content representing the Homepage.</returns>
    private static string GenerateHomePageHTML()
    {
        return @"
        <html>
          <head>
            <link href=""https://fonts.googleapis.com/css2?family=Roboto&display=swap"" rel=""stylesheet"">
            <style>
              body {
                margin: 0; 
                padding: 0; 
                font-family: 'Roboto', sans-serif; 
                text-align: center; 
                background: linear-gradient(to right, #655560, #a4969b);
                color: #ffffff;
              }
              h3 {
                margin-top: 25rem;
                font-size: 3rem;
              }
              a {
                text-decoration: none;
                color: #ffffff;
                background: rgba(196, 202, 208, 1);
                padding: 0.5rem 1rem;
                border-radius: 0.5rem;
                transition: background 0.3s, color 0.3s;
                font-weight: bold;
              }
              a:hover {
                background: #ffffff;
                color: #4facfe;
              }
            </style>
          </head>
          <body>
            <h3>Welcome to the Snake Games Database!</h3>
            <p><a href=""/games"">View Games</a></p>
          </body>
        </html>";
    }

    /// <summary>
    /// Helper function that generates HTML for the games page.
    /// </summary>
    /// <returns>HTML content representing the games page.</returns>
    private static async Task<string> GenerateGamesHTML()
    {
        List<Game> games = await GetAllGamesAsync();
        var htmlbuilder = new StringBuilder();
        htmlbuilder.Append(@"
        <html>
          <head>
            <link href=""https://fonts.googleapis.com/css2?family=Roboto&display=swap"" rel=""stylesheet"">
            <style>
              body {
                margin: 0; 
                padding: 0; 
                font-family: 'Roboto', sans-serif; 
                text-align: center; 
                background: linear-gradient(to right, #655560, #a4969b);
                color: #ffffff;
              }
              h3 {
                margin-top: 10rem;
                font-size: 2rem;
              }
              table {
                margin: 2rem auto;
                border-collapse: collapse;
                background: #ffffff;
                color: #333;
                box-shadow: 0 4px 10px rgba(0,0,0,0.1);
                border-radius: 0.5rem;
                overflow: hidden;
              }
              th, td {
                padding: 1rem 2rem;
                border-bottom: 1px solid #ddd;
                font-size: 1.1rem;
              }
              th {
                background: #fcf7ff;
                color: #000000;
              }
              tr:hover td {
                background: #f0f0f0;
              }
              a {
                text-decoration: none;
                color: #000000;
                font-weight: bold;
              }
              a:hover {
                color: #eb0505;
              }
            </style>
          </head>
          <body>
            <h3>Available Games</h3>
            <table>
              <thead>
                <tr>
                  <th>ID</th><th>Start</th><th>End</th>
                </tr>
              </thead>
              <tbody>");
        foreach (var game in games)
        {
            htmlbuilder.AppendLine($@"<tr onclick=""window.location.href='/games?gid={game.GameId}';"" style=""cursor: pointer;"">");
            htmlbuilder.AppendLine($@"<td>{game.GameId}</td>");
            htmlbuilder.AppendLine($@"<td>{game.StartTime.ToString("G", CultureInfo.InvariantCulture)}</td>");
            htmlbuilder.AppendLine($@"<td>{game.EndTime?.ToString("G", CultureInfo.InvariantCulture) ?? "N/A"}</td>");
            htmlbuilder.AppendLine("</tr>");
        }

        htmlbuilder.Append(@"
              </tbody>
            </table>
          </body>
        </html>");

        return htmlbuilder.ToString();
    }

    /// <summary>
    /// Helper function that generates HTML for the specific game page.
    /// </summary>
    /// <returns>HTML content representing the specific game page.</returns>
    private static async Task<string> GenerateGameHTMLAsync(int gameID)
{
    List<Player> players = await GetAllPlayersAsync(gameID);
    var htmlbuilder = new StringBuilder();

    htmlbuilder.Append(@$"
        <html>
          <head>
            <link href=""https://fonts.googleapis.com/css2?family=Roboto&display=swap"" rel=""stylesheet"">
            <style>
              body {{
                margin: 0; 
                padding: 0; 
                font-family: 'Roboto', sans-serif; 
                text-align: center; 
                background: linear-gradient(to right, #655560, #a4969b);
                color: #ffffff;
              }}
              h3 {{
                margin-top: 10rem;
                font-size: 2rem;
              }}
              table {{
                margin: 2rem auto;
                border-collapse: collapse;
                background: #ffffff;
                color: #333;
                box-shadow: 0 4px 10px rgba(0,0,0,0.1);
                border-radius: 0.5rem;
                overflow: hidden;
              }}
              th, td {{
                padding: 1rem 2rem;
                border-bottom: 1px solid #ddd;
                font-size: 1.1rem;
              }}
              th {{
                background: #fcf7ff;
                color: #000000;
              }}
              tr:hover td {{
                background: #f0f0f0;
              }}
            </style>
          </head>
          <body>
            <h3>Stats for Game {gameID}</h3>
            <table>
              <thead>
                <tr>
                  <th>Player ID</th><th>Player Name</th><th>Max Score</th><th>Enter Time</th><th>Leave Time</th>
                </tr>
              </thead>
              <tbody>");

    foreach (Player p in players)
    {
        string joinTime = p.JoinTime.ToString("G", CultureInfo.InvariantCulture);
        string leaveTime = p.LeaveTime?.ToString("G", CultureInfo.InvariantCulture) ?? "N/A";
        htmlbuilder.AppendLine("    <tr>");
        htmlbuilder.AppendLine($@"<td>{p.ID}</td>
                                <td>{p.Name}</td>
                                <td>{p.MaxScore}</td>
                                <td>{joinTime}</td>
                                <td>{leaveTime}</td>");
        htmlbuilder.AppendLine("    </tr>");
    }

    htmlbuilder.Append(@"
              </tbody>
            </table>
          </body>
        </html>");

    return htmlbuilder.ToString();
}

    /// <summary>
    /// Helper function to retrieve all the games from the SQL server.
    /// </summary>
    /// <returns>List of Games.</returns>
    private static async Task<List<Game>> GetAllGamesAsync()
    {
        var games = new List<Game>();

        try
        {
            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT GameId, StartTime, EndTime FROM Games";

                await using (var command = new SqlCommand(query, connection))
                {
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var game = new Game
                            {
                                GameId = reader.GetInt32(0),
                                StartTime = reader.GetDateTime(1),
                                EndTime = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                            };

                            games.Add(game);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching games: {ex.Message}");
        }

        return games;
    }

    /// <summary>
    /// Helper function to get players that are in the given game id from the SQL server.
    /// </summary>
    /// <param name="gid">Game Id.</param>
    /// <returns>List of all the players in SQL.</returns>
    private static async Task<List<Player>> GetAllPlayersAsync(int gid)
    {
        var players = new List<Player>();

        try
        {
            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = $"SELECT SnakeId, Name, MaxScore, JoinTime, LeaveTime, GameId FROM Players WHERE GameId = {gid}";

                await using (var command = new SqlCommand(query, connection))
                {
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var player = new Player
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                MaxScore = reader.GetInt32(2),
                                JoinTime = reader.GetDateTime(3),
                                LeaveTime = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                GameId = reader.GetInt32(5),
                            };

                            players.Add(player);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching players: {ex.Message}");
        }

        return players;
    }

    /// <summary>
    /// Helper function to update connection string. Called at first the program starts.
    /// </summary>
    private static void UpdateConnectionString()
    {
        var builder = new ConfigurationBuilder();

        // Build the configuration to use the secrets.json file.
        builder.AddUserSecrets<MySecret>();
        IConfigurationRoot configuration = builder.Build();

        string? userId = configuration["UserID"];
        string? password = configuration["Password"];

        connectionString = new SqlConnectionStringBuilder()
        {
            DataSource = "cs3500.eng.utah.edu, 14330",
            InitialCatalog = "F2024_db_u1424260",
            UserID = userId,
            Password = password,
            ConnectTimeout = 15, // if the server doesn't connect in X seconds, give up
            Encrypt = false,
        }.ConnectionString;
    }

    /// <summary>
    /// private class that stores information about games.
    /// </summary>
    private class Game
    {
        public int GameId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// private class that stores information about Players.
    /// </summary>
    private class Player
    {
        public int ID { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MaxScore { get; set; }

        public DateTime JoinTime { get; set; }

        public DateTime? LeaveTime { get; set; }

        public int GameId { get; set; }
    }
}