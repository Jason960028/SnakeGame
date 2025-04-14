// <copyright file="Snake.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// Author:    Soyoun Kim
// Partner:   Jason Chang
// Date:      Nov 24 2024
// Course:    CS 3500, University of Utah, School of Computing
// Copyright: CS 3500 and Jason Chang - This work may not
//            be copied for use in Academic Coursework.
//
// I, Soyoun Kim, certify that I wrote this code from scratch and
// did not copy it in part or whole from another source.  All
// references used in the completion of the assignments are cited
// in my README file.
//
// File Contents: Class that defines the snake received by server.

namespace Snake.Client.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Snake object received by the server.
/// </summary>
public class Snake
{
    /// <summary>
    /// Gets or sets the ID of the snake.
    /// </summary>
    [JsonPropertyName("snake")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the snake.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the body of the snake.
    /// </summary>
    [JsonPropertyName("body")]
    public List<Point2D> Body { get; set; } = new List<Point2D>();

    /// <summary>
    /// Gets or sets the direction of the snake.
    /// </summary>
    [JsonPropertyName("dir")]
    public Point2D Direction { get; set; } = new Point2D();

    /// <summary>
    /// Gets or sets the score of the snake.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the snake died.
    /// </summary>
    [JsonPropertyName("died")]
    public bool Died { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the snake is alive.
    /// </summary>
    [JsonPropertyName("alive")]
    public bool Alive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the snake disconnected.
    /// </summary>
    [JsonPropertyName("dc")]
    public bool DisConnected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the snake joined.
    /// </summary>
    [JsonPropertyName("join")]
    public bool Joined { get; set; }

    /// <summary>
    /// Gets the maximum score of the snake. This value is tracked locally and not deserialized from the server.
    /// </summary>
    [JsonIgnore]
    public int MaxScore { get; private set; }

    /// <summary>
    /// Gets or sets the time when the snake joined the game. This value is tracked locally.
    /// </summary>
    [JsonIgnore]
    public DateTime StartTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the time when the snake left the game. This value is tracked locally.
    /// </summary>
    [JsonIgnore]
    public DateTime LeaveTime { get; set; } = new DateTime(2000, 1, 01, 00, 00, 00, 000);

    /// <summary>
    /// Updates the maximum score of the snake if the current score is higher.
    /// </summary>
    public void UpdateMaxScore()
    {
        if (this.Score > this.MaxScore)
        {
            this.MaxScore = this.Score;
        }
    }
}
