// <copyright file="SnakeClient.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// Author:    Soyoun Kim
// Partner:   Jason Chang
// Date:      Dec 05 2024
// Course:    CS 3500, University of Utah, School of Computing
// Copyright: CS 3500 and Jason Chang - This work may not
//            be copied for use in Academic Coursework.
//
// I, Soyoun Kim, certify that I wrote this code from scratch and
// did not copy it in part or whole from another source.  All
// references used in the completion of the assignments are cited
// in my README file.
//
// File Contents:
// This file contains the partial snakeclien class.
// Helper functions for Draw function and Connect Function is on this file.

namespace Snake.Client.Pages;

using System.Drawing;
using System.Text.Json;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Snake.Client.Models;

/// <summary>
/// SnakeClient class itself.
/// </summary>
public partial class SnakeClient
{
    /// <summary>
    /// Snakes in the world. The key is ID, Value is snake class itself.
    /// Snakes are removed if they are disconnected.
    /// </summary>
    private Dictionary<int, Snake> snakes = new Dictionary<int, Snake>();

    /// <summary>
    /// Walls in the world. The key is ID, Value is Wall class itself.
    /// </summary>
    private Dictionary<int, Wall> walls = new Dictionary<int, Wall>();

    /// <summary>
    /// PowerUps in the world. The key is ID, Value is PowerUp class itself.
    /// PowerUps are removed if they are disconnected.
    /// </summary>
    private Dictionary<int, PowerUp> powerUps = new Dictionary<int, PowerUp>();

    private int worldSize = 2000;
    private int snakeId;
    private string direction = "none";
    private ElementReference backgroundImage;
    private ElementReference wallImage;
    private string serverAddress = "localhost";
    private int port = 11000;
    private double pulseTime = 0; // Used for the pulsing effect
    private int gameId = 0;

    /// <summary>
    /// Use of SqlConnection.
    /// </summary>
    private SqlConnector sqlConnector = new SqlConnector();

    /// <summary>
    /// Sends the Keyboard input to the server.
    /// </summary>
    /// <param name="key">keyboard input.</param>
    [JSInvokable]
    public void HandleKeyDown(string key)
    {
        this.direction = key switch
        {
            "ArrowUp" or "w" or "W" => "up",
            "ArrowDown" or "s" or "S" => "down",
            "ArrowLeft" or "a" or "A" => "left",
            "ArrowRight" or "d" or "D" => "right",
            _ => "none",
        };
    }

    /// <summary>
    /// Gets Head of given snake.
    /// </summary>
    /// <param name="s">snake.</param>
    /// <returns>head of the snake.</returns>
    private static Point2D GetHead(Snake s)
    {
        return s.Body.Count > 0 ? s.Body[s.Body.Count - 1] : new Point2D();
    }

    /// <summary>
    /// Helper function to get the centering point.
    /// </summary>
    /// <returns>Centering Point.</returns>
    private Point2D GetCenteringPoint()
    {
        Point2D head = this.GetHeadOfThisSnake();
        Point2D center = new Point2D();
        center.X = head.X;
        center.Y = head.Y;

        if (center.X > ((this.worldSize / 2) - (this.GUIWidth / 2)))
        {
            center.X = (this.worldSize / 2) - (this.GUIWidth / 2);
        }

        if (center.X < -((this.worldSize / 2) - (this.GUIWidth / 2)))
        {
            center.X = -((this.worldSize / 2) - (this.GUIWidth / 2));
        }

        if (center.Y > ((this.worldSize / 2) - (this.GUIHeight / 2)))
        {
            center.Y = (this.worldSize / 2) - (this.GUIHeight / 2);
        }

        if (center.Y < -((this.worldSize / 2) - (this.GUIHeight / 2)))
        {
            center.Y = -((this.worldSize / 2) - (this.GUIHeight / 2));
        }

        return center;
    }

    /// <summary>
    /// Sends the direction stored in class to the server.
    /// </summary>
    private void SendDirectionToServer()
    {
        if (this.direction == "none")
        {
            return;
        }

        if (this.snakes.Count > 0 && this.worldSize > 0 && this.walls.Count > 0)
        {
            ControlCommand c = new ControlCommand { Moving = this.direction };
            string jsonstring = JsonSerializer.Serialize(c);
            this.server.Send(jsonstring);
            this.logger.LogTrace("Sent keyboard input to the server.");
        }
    }

    /// <summary>
    /// Disconnects the client from the server.
    /// </summary>
    private void Disconnect()
    {
        if (this.snakes.TryGetValue(this.snakeId, out Snake? snake))
        {
            // Update LeaveTime for the player's snake
            snake.LeaveTime = DateTime.Now;

            // Update the database with the leave time
            _ = this.sqlConnector.UpdatePlayerLeaveTime(snake.Id, snake.LeaveTime, this.gameId);
            this.logger.LogInformation($"Updated LeaveTime for Snake ID: {snake.Id}");
        }

        _ = this.sqlConnector.EndGameAsync(this.gameId, DateTime.Now);

        // Handle client-side disconnection
        this.server.Disconnect();
        this.networkStatus = "Disconnected";
        this.logger.LogInformation("Client disconnected from the server.");
        this.snakes.Clear();

        // Update the UI to reflect the disconnection
        this.StateHasChanged();
    }

    /// <summary>
    ///   Attempt to connect to the server and begin the drawing process.
    /// </summary>
    private async void Connect()
    {
        this.logger.LogInformation("Connecting!");
        this.gameId++;
        _ = this.sqlConnector.AddGameAsync(this.gameId, DateTime.Now, new DateTime(2000, 1, 01, 00, 00, 00, 000));

        await Task.Run(() =>
        {
            this.errorMessage = string.Empty;
            this.networkStatus = "Connecting...";
            try
            {
                this.server.Connect(this.serverAddress, this.port);
                this.networkStatus = "Connected";
                this.ConnectTime = DateTime.Now;
                this.logger.LogInformation("Connected to the server");
            }
            catch (Exception e)
            {
                this.errorMessage = e.Message;
                this.networkStatus = "Error";
                this.logger.LogError("Failed to connect to the server");
            }

            try
            {
                if (this.userID.Length > 16)
                {
                    this.userID = this.userID.Substring(0, 16); // Ensure it fits the protocol requirement
                }

                this.server.Send(this.userID); // Send player's name to the server
                this.logger.LogInformation($"Player name '{this.userID}' sent to the server.");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to send player name: {ex.Message}");
            }

            try
            {
                string id = this.server.ReadLine();
                string worldSize = this.server.ReadLine();

                if (int.TryParse(id, out this.snakeId) && int.TryParse(worldSize, out this.worldSize))
                {
                    this.logger.LogDebug($"Received Player ID: {this.snakeId}, World Size: {this.worldSize}");
                }
                else
                {
                    throw new Exception("Invalid player ID or world size received from the server.");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error parsing Player ID and World Size: {ex.Message}");
                this.networkStatus = "Error";
            }
        });

        this.StateHasChanged();

        if (this.server.IsConnected)
        {
            await Task.Run((Action)(() =>
            {
                try
                {
                    while (true)
                    {
                        string worldJSON = this.server.ReadLine();
                        lock (this.worldModel)
                        {
                            this.StoreObjectFromJSON(worldJSON);
                        }

                        this.SendDirectionToServer();
                    }
                }
                catch (Exception e)
                {
                    this.errorMessage = e.Message;
                    this.networkStatus = "Error";
                }

                this.logger.LogError("Connection must have failed.");
            }));

            foreach (var snake in this.snakes.Values)
            {
                // _ = this.sqlConnector.UpdatePlayerGameId(snake.Id, this.gameId);
                _ = this.sqlConnector.AddPlayerAsync(snake.Id, snake.Name, snake.Score, snake.StartTime, snake.LeaveTime, this.gameId);

                this.logger.LogInformation($"Incremented GameId for Snake ID: {snake.Id} to {this.gameId}");
            }
        }
    }

    /// <summary>
    /// Receives the JSON from the server as string, stores it in the appropriate dictionary.
    /// </summary>
    /// <param name="jsonstring">JSON received.</param>
    private void StoreObjectFromJSON(string jsonstring)
    {
        if (jsonstring.Contains("snake"))
        {
            Snake s = JsonSerializer.Deserialize<Snake>(jsonstring)!;
            if (s == null)
            {
                this.logger.LogError("Received wrong JSON");
            }
            else
            {
                // Check if the snake already exists in the dictionary
                if (this.snakes.TryGetValue(s.Id, out Snake? existingSnake))
                {
                    // Update existing snake
                    existingSnake.Score = s.Score;
                    existingSnake.Died = s.Died;
                    existingSnake.DisConnected = s.DisConnected;
                    existingSnake.Body = s.Body;
                    existingSnake.Direction = s.Direction;
                    existingSnake.Alive = s.Alive;
                    existingSnake.Joined = s.Joined;

                    if (s.DisConnected)
                    {
                        // Update LeaveTime for the player's snake
                        existingSnake.LeaveTime = DateTime.Now;
                        _ = this.sqlConnector.UpdatePlayerLeaveTime(existingSnake.Id, existingSnake.LeaveTime, this.gameId);
                        this.logger.LogInformation($"Updated LeaveTime for Snake ID: {existingSnake.Id}");
                    }

                    existingSnake.UpdateMaxScore();
                    _ = this.sqlConnector.UpdatePlayerScore(existingSnake.Id, existingSnake.MaxScore, this.gameId);
                }
                else
                {
                    // Add new snake to the dictionary
                    this.snakes[s.Id] = s;
                    this.logger.LogInformation($"Added new snake with ID {s.Id} and name {s.Name}.");

                    // Optionally, add the new player to the database
                    _ = this.sqlConnector.AddPlayerAsync(s.Id, s.Name, s.Score, s.StartTime, s.LeaveTime, this.gameId);

                    // this.gameId = this.sqlConnector.GetLatestGameId();
                    this.logger.LogInformation($"Game started with GameId: {this.gameId}");
                }
            }

            // this.AlertEvent(s);
        }
        else if (jsonstring.Contains("wall"))
        {
            Wall w = JsonSerializer.Deserialize<Wall>(jsonstring)!;
            if (w == null)
            {
                this.logger.LogError("Received wrong JSON");
            }
            else
            {
                this.walls[w.Id] = w;
            }
        }
        else if (jsonstring.Contains("power"))
        {
            PowerUp p = JsonSerializer.Deserialize<PowerUp>(jsonstring)!;
            if (p == null)
            {
                this.logger.LogError("Received Wrong JSON");
            }
            else
            {
                this.powerUps[p.Id] = p;
            }
        }
        else
        {
            this.logger.LogError("Something wrong has been received.");
        }
    }

    /// <summary>
    /// Updates the event alert related with the snakes.
    /// </summary>
    /// <param name="s">snake.</param>
    private void AlertEvent(Snake s)
    {
        if (s.DisConnected)
        {
            this.UpdateAlert($"Snake {s.Name} disconnected.");
            this.logger.LogTrace("Updated Alert As Disconnected.");

            if (this.snakes.TryGetValue(this.snakeId, out Snake? snake))
            {
                snake.LeaveTime = DateTime.Now;
                _ = this.sqlConnector.UpdatePlayerLeaveTime(this.snakeId, snake.LeaveTime, this.gameId);
                this.logger.LogInformation($"Updated LeaveTime for Snake ID: {snake.Id}");
            }
        }
        else if (s.Died)
        {
            this.UpdateAlert($"Snake {s.Name} Died.");
            this.logger.LogTrace("Updated Alert As Died.");
        }

        if (s.Joined)
        {
            this.UpdateAlert($"Snake {s.Name} joined.");
            this.logger.LogTrace("Updated Alert As Joined.");
        }
    }

    /// <summary>
    /// Draws the background.
    /// </summary>
    private async Task DrawBackGround()
    {
        // Clip the view so objects outside the canvas are not shown
        await this.context.BeginPathAsync();
        await this.context.RectAsync(0, 0, this.GUIWidth, this.GUIHeight);
        await this.context.ClipAsync();

        // Save the current state of the transformation matrix
        await this.context.SaveAsync();

        // Get the position of the snake's head
        Point2D center = this.GetCenteringPoint();

        // Calculate offsets to center the head
        double offsetX = center.X - (this.GUIWidth / 2);
        double offsetY = center.Y - (this.GUIHeight / 2);

        // Apply transformations to center the view on the snake's head
        await this.context.TranslateAsync(-offsetX, -offsetY);

        await this.context.DrawImageAsync(this.backgroundImage, -this.worldSize / 2, -this.worldSize / 2, this.worldSize, this.worldSize);
    }

    /// <summary>
    /// Get the head of this player.
    /// </summary>
    /// <returns>head.</returns>
    private Point2D GetHeadOfThisSnake()
    {
        Point2D head = new Point2D();

        lock (this.worldModel)
        {
            if (this.snakes.TryGetValue(this.snakeId, out Snake? s))
            {
                if (s == null)
                {
                    return new Point2D();
                }

                head = GetHead(s);
            }
        }

        return head;
    }

    /// <summary>
    /// Draws the model.
    /// </summary>
    /// <returns>Task.</returns>
    private async Task DrawModel()
    {
        List<Snake> snakesNow = new List<Snake>();
        List<Wall> wallsNow = new List<Wall>();
        List<PowerUp> powerUpsNow = new List<PowerUp>();

        lock (this.worldModel)
        {
            snakesNow = this.snakes.Values.ToList();
            wallsNow = this.walls.Values.ToList();
            powerUpsNow = this.powerUps.Values.ToList();
        }

        this.logger.LogTrace("Drawing...");

        // Draw walls
        foreach (var wall in wallsNow)
        {
            await this.DrawWall(wall);
        }

        // Draw and check power-ups
        foreach (var powerUp in powerUpsNow)
        {
            await this.DrawPowerUP(powerUp);
        }

        // Draw snakes
        foreach (var snake in snakesNow)
        {
            if (snake.Body.Count < 1 || snake.DisConnected)
            {
                lock (this.worldModel)
                {
                    this.snakes.Remove(snake.Id);
                }

                this.logger.LogDebug($"{snake.Name} disconnected.");
            }
            else
            {
                await this.DrawSnake(snake);
            }
        }

        this.logger.LogTrace("Drawing finished!");
    }

    /// <summary>
    /// Draws the wall as a series of rectangles along its length.
    /// </summary>
    /// <param name="wall">Wall to be drawn.</param>
    /// <returns>Task.</returns>
    private async Task DrawWall(Wall wall)
    {
        // Calculate the length of the wall
        double length = Math.Sqrt(Math.Pow(wall.EndPoint.X - wall.StartPoint.X, 2) +
                                  Math.Pow(wall.EndPoint.Y - wall.StartPoint.Y, 2));

        // Calculate the angle of rotation for the wall image
        double angle = Math.Atan2(wall.EndPoint.Y - wall.StartPoint.Y, wall.EndPoint.X - wall.StartPoint.X);

        await this.context.SaveAsync();

        // Translate and rotate the context to align with the wall
        await this.context.TranslateAsync(wall.StartPoint.X, wall.StartPoint.Y);
        await this.context.RotateAsync((float)angle);

        await this.context.DrawImageAsync(this.wallImage, 0, -25, length, 50);

        // Restore the context state
        await this.context.RestoreAsync();
    }

    /// <summary>
    /// Draws PowerUp on the canvas.
    /// </summary>
    /// <param name="powerUp">Powerup Item.</param>
    /// <returns>Task.</returns>
    private async Task DrawPowerUP(PowerUp powerUp)
    {
        if (powerUp.Died)
        {
            lock (this.worldModel)
            {
                this.powerUps.Remove(powerUp.Id);
            }

            this.logger.LogInformation($"PowerUp {powerUp.Id} collected and removed.");
        }
        else
        {
            // Calculate pulse effect
            this.pulseTime += 0.1; // Adjust speed as needed
            double scale = 1 + (0.2 * Math.Sin(this.pulseTime)); // Pulses between 1.0 and 1.2
            double opacity = 0.5 + (0.5 * Math.Abs(Math.Sin(this.pulseTime))); // Pulses opacity between 0.5 and 1.0

            // Apply glow effect
            await this.context.SetGlobalAlphaAsync((float)opacity); // Adjust opacity
            await this.context.BeginPathAsync();
            await this.context.ArcAsync(powerUp.Location.X, powerUp.Location.Y, 8 * scale, 0, 2 * Math.PI); // Scale the size
            await this.context.SetFillStyleAsync("rgba(255, 0, 0, 0.8)"); // Red with some transparency
            await this.context.FillAsync();
            await this.context.ClosePathAsync();

            // Reset alpha for other drawings
            await this.context.SetGlobalAlphaAsync(1.0F);
        }
    }

    /// <summary>
    /// Draws the snake.
    /// </summary>
    /// <param name="snake">Snake to be drawn.</param>
    /// <returns>Task.</returns>
    private async Task DrawSnake(Snake snake)
    {
        if (!snake.Alive)
        {
            await this.context.SetStrokeStyleAsync("black");
        }
        else
        {
            await this.SetSnakeColor(snake.Id);
        }

        await this.context.SetLineWidthAsync(10);
        await this.context.SetLineCapAsync(Blazor.Extensions.Canvas.Canvas2D.LineCap.Round);
        await this.context.BeginPathAsync();

        await this.context.MoveToAsync(snake.Body[0].X, snake.Body[0].Y);

        for (int i = 1; i < snake.Body.Count; i++)
        {
            await this.context.LineToAsync(snake.Body[i].X, snake.Body[i].Y);
        }

        await this.context.StrokeAsync();
        await this.context.ClosePathAsync();

        Point2D head = GetHead(snake);
        await this.context.SetTextAlignAsync(Blazor.Extensions.Canvas.Canvas2D.TextAlign.Center);
        await this.context.SetTextBaselineAsync(Blazor.Extensions.Canvas.Canvas2D.TextBaseline.Middle);
        await this.context.SetFontAsync("10px Arial");
        await this.context.SetFillStyleAsync("white");

        await this.context.FillTextAsync(snake.Name, head.X, head.Y);
        await this.context.FillTextAsync(snake.Score.ToString(), head.X, head.Y + 20);
    }

    /// <summary>
    /// Set color of the snake based on the id.
    /// </summary>
    /// <param name="id">ID of the snake.</param>
    /// <returns>Task.</returns>
    private async Task SetSnakeColor(int id)
    {
        switch (id % 8)
        {
            case 0:
                await this.context.SetStrokeStyleAsync("#007FFF");
                break;
            case 1:
                await this.context.SetStrokeStyleAsync("#FFFF00");
                break;
            case 2:
                await this.context.SetStrokeStyleAsync("#FF00FF");
                break;
            case 3:
                await this.context.SetStrokeStyleAsync("#FFA500");
                break;
            case 4:
                await this.context.SetStrokeStyleAsync("#00FFFF");
                break;
            case 5:
                await this.context.SetStrokeStyleAsync("#800080");
                break;
            case 6:
                await this.context.SetStrokeStyleAsync("#9ACD32");
                break;
            case 7:
                await this.context.SetStrokeStyleAsync("#8B4513");
                break;
        }
    }

    /// <summary>
    /// Draws HeadsUpDisplay.
    /// </summary>
    private async Task DrawHUD(double fps, double nps, int timeInSeconds)
    {
        // Left HUD for stats and network info
        double hudWidth = 300;
        double hudHeight = 200;

        // Set the position of the Left HUD (top-left corner)
        double hudX = 10;
        double hudY = 10;

        // Draw semi-transparent background for Left HUD
        await this.context.SetFillStyleAsync("rgba(0, 0, 0, 0.6)");
        await this.context.FillRectAsync(hudX, hudY, hudWidth, hudHeight);

        // Draw border around Left HUD
        await this.context.SetStrokeStyleAsync("white");
        await this.context.SetLineWidthAsync(2);
        await this.context.StrokeRectAsync(hudX, hudY, hudWidth, hudHeight);

        // Set font and color for text
        await this.context.SetFontAsync("16px Arial");
        await this.context.SetFillStyleAsync("white");
        await this.context.SetTextAlignAsync(TextAlign.Left);
        await this.context.SetTextBaselineAsync(TextBaseline.Top);

        // Line height for spacing
        double lineHeight = 22;
        double textX = hudX + 10;
        double textY = hudY + 10;

        // Display game stats
        await this.context.FillTextAsync($"FPS: {fps:F1}", textX, textY);
        textY += lineHeight;
        await this.context.FillTextAsync($"NPS: {nps:F1}", textX, textY);
        textY += lineHeight;
        await this.context.FillTextAsync($"Elapsed Time: {timeInSeconds} s", textX, textY);
        textY += lineHeight;

        // Draw a separator line
        textY += 5;
        await this.context.BeginPathAsync();
        await this.context.MoveToAsync(hudX + 5, textY);
        await this.context.LineToAsync(hudX + hudWidth - 5, textY);
        await this.context.SetStrokeStyleAsync("white");
        await this.context.SetLineWidthAsync(1);
        await this.context.StrokeAsync();
        await this.context.ClosePathAsync();
        textY += 10;

        // Display network status
        await this.context.FillTextAsync($"Network Status: {this.networkStatus}", textX, textY);
        textY += lineHeight;

        // Display error message if any
        if (!string.IsNullOrEmpty(this.errorMessage))
        {
            await this.context.SetFillStyleAsync("red");
            await this.context.FillTextAsync($"Error: {this.errorMessage}", textX, textY);
            await this.context.SetFillStyleAsync("white");
            textY += lineHeight;
        }

        // Display alert message
        if (this.server.IsConnected && !string.IsNullOrEmpty(this.AlertMessage))
        {
            await this.context.FillTextAsync($"Alert: {this.AlertMessage}", textX, textY);
            textY += lineHeight;
        }

        // Right HUD for player rankings
        double playerHudWidth = 250;
        double playerHudHeight;

        // Get the list of players (snakes) and rank them by score
        List<Snake> snakesNow;
        lock (this.worldModel)
        {
            snakesNow = this.snakes.Values.OrderByDescending(s => s.Score).ToList();
        }

        // Limit the number of players displayed to prevent HUD overflow
        int maxPlayersToDisplay = 10;
        bool morePlayers = false;
        if (snakesNow.Count > maxPlayersToDisplay)
        {
            snakesNow = snakesNow.Take(maxPlayersToDisplay).ToList();
            morePlayers = true;
        }

        int numPlayerLines = snakesNow.Count + 1;
        if (morePlayers)
        {
            numPlayerLines += 1;
        }

        playerHudHeight = (numPlayerLines * lineHeight) + 20;

        // Position the player HUD on the right side
        double playerHudX = this.GUIWidth - playerHudWidth - 10;
        double playerHudY = 10;

        // Draw semi-transparent background for player HUD
        await this.context.SetFillStyleAsync("rgba(0, 0, 0, 0.6)");
        await this.context.FillRectAsync(playerHudX, playerHudY, playerHudWidth, playerHudHeight);

        // Draw border around player HUD
        await this.context.SetStrokeStyleAsync("white");
        await this.context.SetLineWidthAsync(2);
        await this.context.StrokeRectAsync(playerHudX, playerHudY, playerHudWidth, playerHudHeight);

        // Set text properties
        await this.context.SetFontAsync("16px Arial");
        await this.context.SetTextAlignAsync(TextAlign.Left);
        await this.context.SetTextBaselineAsync(TextBaseline.Top);

        // Start drawing player rankings
        double playerTextX = playerHudX + 10;
        double playerTextY = playerHudY + 10;

        await this.context.SetFillStyleAsync("white");
        await this.context.FillTextAsync("Rankings:", playerTextX, playerTextY);
        playerTextY += lineHeight;

        int rank = 1;
        foreach (var snake in snakesNow)
        {
            // Highlight the player's own snake
            if (snake.Id == this.snakeId)
            {
                await this.context.SetFillStyleAsync("yellow");
            }
            else
            {
                await this.context.SetFillStyleAsync("white");
            }

            string playerInfo = $"{rank}. {snake.Name} | Score: {snake.Score}";
            await this.context.FillTextAsync(playerInfo, playerTextX, playerTextY);
            playerTextY += lineHeight;
            rank++;
        }

        if (morePlayers)
        {
            await this.context.SetFillStyleAsync("white");
            await this.context.FillTextAsync("...", playerTextX, playerTextY);
        }
    }
}