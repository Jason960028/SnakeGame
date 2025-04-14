// <copyright file="SqlConnector.cs" company="UofU-CS3500">
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
// File Contents: Class that handles SQL connection and managing game attributes.

namespace Snake.Client.Models;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Provides methods for interacting with the database for the Snake game,
/// including managing games, players, and their attributes.
/// </summary>
public class SqlConnector
{
    private readonly string connectionString = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlConnector"/> class, setting up a connection string
    /// for the database using user secrets for secure storage of credentials.
    /// </summary>
    /// <remarks>
    /// This constructor uses the <see cref="ConfigurationBuilder"/> to load user secrets containing the
    /// database credentials, including the user ID and password. The connection string is then
    /// constructed using these credentials and preconfigured parameters for the database server.
    /// </remarks>
    public SqlConnector()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<SqlConnector>();
        IConfigurationRoot configuration = builder.Build();

        string? userId = configuration["UserID"];
        string? password = configuration["Password"];
        this.connectionString = new SqlConnectionStringBuilder()
        {
            DataSource = "cs3500.eng.utah.edu, 14330",
            InitialCatalog = "F2024_db_u1424260",
            UserID = userId,
            Password = password,
            ConnectTimeout = 15,
            Encrypt = false,
        }.ConnectionString;
    }

    /// <summary>
    /// Adds a new game record to the database with the provided game ID, start time, and leave time.
    /// </summary>
    /// <param name="gameId">The unique identifier of the game.</param>
    /// <param name="startTime">The start time of the game.</param>
    /// <param name="leaveTime">The end time of the game.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task AddGameAsync(int gameId, DateTime startTime, DateTime leaveTime)
    {
        try
        {
            await using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                // Enable manual insertion of the GameId if it is an identity column
                var query = "INSERT INTO Games (GameId, StartTime, EndTime) VALUES (@gameId, @startTime, @leaveTime)";

                await using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@gameId", gameId);
                    command.Parameters.AddWithValue("@startTime", startTime);
                    command.Parameters.AddWithValue("@leaveTime", leaveTime);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding game: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the end time of an existing game in the database.
    /// </summary>
    /// <param name="gameId">The unique identifier of the game.</param>
    /// <param name="endTime">The end time to be set for the game.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task EndGameAsync(int gameId, DateTime endTime)
    {
        try
        {
            await using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var query = "UPDATE Games SET EndTime = @endTime WHERE GameId = @gameId";
                await using (var commend = new SqlCommand(query, connection))
                {
                    commend.Parameters.AddWithValue("@endTime", endTime);
                    commend.Parameters.AddWithValue("@gameId", gameId);
                    await commend.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ending game: {ex.Message}");
        }
    }

    /// <summary>
    /// Adds a new player record to the database with the specified attributes.
    /// </summary>
    /// <param name="snakeId">The unique identifier of the snake (player).</param>
    /// <param name="name">The name of the player.</param>
    /// <param name="maxScore">The maximum score achieved by the player.</param>
    /// <param name="joinTime">The time the player joined the game.</param>
    /// <param name="leaveTime">The time the player left the game (nullable).</param>
    /// <param name="gameId">The unique identifier of the game the player is part of.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task AddPlayerAsync(int snakeId, string name, int maxScore, DateTime joinTime, DateTime? leaveTime, int gameId)
    {
        try
        {
            await using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Players (SnakeId, Name, MaxScore, JoinTime, LeaveTime, GameId) VALUES (@snakeId, @name, @maxScore, @joinTime, @leaveTime, @gameId)";
                await using (var commend = new SqlCommand(query, connection))
                {
                    commend.Parameters.AddWithValue("@gameId", gameId);
                    commend.Parameters.AddWithValue("@snakeId", snakeId);
                    commend.Parameters.AddWithValue("@name", name);
                    commend.Parameters.AddWithValue("@joinTime", joinTime);
                    commend.Parameters.AddWithValue("@leaveTime", leaveTime);
                    commend.Parameters.AddWithValue("@maxScore", maxScore);
                    await commend.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding player: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the maximum score for a specific player in a specific game.
    /// </summary>
    /// <param name="snakeId">The unique identifier of the snake (player).</param>
    /// <param name="maxScore">The new maximum score to be set.</param>
    /// <param name="gameId">The unique identifier of the game the player is part of.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdatePlayerScore(int snakeId, int maxScore, int gameId)
    {
        try
        {
            await using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var query = "UPDATE Players SET MaxScore = @maxScore WHERE SnakeId = @snakeId AND GameId = @gameId";
                await using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@maxScore", maxScore);
                    command.Parameters.AddWithValue("@snakeId", snakeId);
                    command.Parameters.AddWithValue("@gameId", gameId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating player score: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the leave time for a specific player in a specific game.
    /// </summary>
    /// <param name="snakeId">The unique identifier of the snake (player).</param>
    /// <param name="leaveTime">The new leave time to be set for the player.</param>
    /// <param name="gameId">The unique identifier of the game the player is part of.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdatePlayerLeaveTime(int snakeId, DateTime leaveTime, int gameId)
    {
        try
        {
            await using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var query = "UPDATE Players SET LeaveTime = @leaveTime WHERE SnakeId = @snakeId AND GameId = @gameId";
                await using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@leaveTime", leaveTime);
                    command.Parameters.AddWithValue("@snakeId", snakeId);
                    command.Parameters.AddWithValue("@gameId", gameId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating player leave time: {ex.Message}");
        }
    }
}
