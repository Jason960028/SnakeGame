// <copyright file="LogSupport.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// Author:    Soyoun Kim
// Partner:   Jason Chang
// Date:      Nov 19 2024
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
// This file Defines Logsupport Class used in Snake.Client Class.
// Expected to used also in server class in the future.

namespace CS3500.LogSupport;

using Microsoft.Extensions.Logging;

/// <summary>
/// Support class for Logging. only has the function to get Logger.
/// </summary>
/// <typeparam name="T">The Class that needs logging.</typeparam>
public static class LogSupport<T>
{
    /// <summary>
    /// getting the logger for this particular class.
    /// Use the logger for each class that needs logging.
    /// </summary>
    /// <returns>returns the logger.</returns>
    public static ILogger GetLogger()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        return loggerFactory.CreateLogger<T>();
    }
}
