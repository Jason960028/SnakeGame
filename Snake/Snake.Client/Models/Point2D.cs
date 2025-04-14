// <copyright file="Point2D.cs" company="UofU-CS3500">
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
// File Contents: Point2D class used in representing location of the elements of worldmodel.

namespace Snake.Client.Models;

/// <summary>
/// Class used in representing the location of the point.
/// </summary>
public class Point2D
{
    /// <summary>
    /// Gets or Sets the X coordinate of the point.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Gets or Sets the Y coordinate of the point.
    /// </summary>
    public int Y { get; set; }
}
