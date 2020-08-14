﻿// <copyright file="Program.cs" company="None">
// Copyright (c) None. All rights reserved.
// </copyright>

namespace GameHelper
{
    using System;
    using System.IO;
    using ClickableTransparentOverlay;
    using GameHelper.Plugin;
    using GameHelper.UI;

    /// <summary>
    /// Class executed when the application starts.
    /// TODO: Make global config manager with profiles.
    /// TODO:   load the config from the file.
    /// TODO:   save the config on the file.
    /// TODO: Make Plugin Manager.
    /// TODO:   complete the DRAW PLUGIN UI function.
    /// TODO: Update pattern finder to not crash the overlay if pattern not found.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// function executed when the application starts.
        /// </summary>
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, exceptionArgs) =>
            {
                var errorText = "Program exited with message:\n " + exceptionArgs.ExceptionObject;
                File.AppendAllText("Error.log", $"{DateTime.Now:g} {errorText}\r\n{new string('-', 30)}\r\n");
                Environment.Exit(1);
            };

            var settings = CoreSettings.CreateOrLoadSettings();
            if (!settings.ShowTerminal)
            {
                Overlay.TerminalWindow = false;
            }

            PluginManager.Initialize();
            SettingsWindow.InitializeCoroutines(settings);
            Core.Initialize();
            Overlay.RunInfiniteLoop(); // Overlay disposes itself before exit.
            Core.Dispose();
        }
    }
}
