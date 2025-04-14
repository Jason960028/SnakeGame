// <copyright file="ControlCommand.cs" company="UofU-CS3500">
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
// File Contents: Class used in representing Control Command of the elements of worldmodel.

namespace Snake.Client.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Class used in representing control command of the elements of worldmodel.
/// </summary>
public class ControlCommand
{
    /// <summary>
    /// Gets or Sets the direction that user indicates to move.
    /// </summary>
    [JsonPropertyName("moving")]
    public string Moving { get; set; } = "none";
}
