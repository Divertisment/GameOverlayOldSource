﻿// <copyright file="SettingsWindow.cs" company="None">
// Copyright (c) None. All rights reserved.
// </copyright>

namespace GameHelper.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using ClickableTransparentOverlay;
    using Coroutine;
    using GameHelper.Plugin;
    using ImGuiNET;

    /// <summary>
    /// Creates the MainMenu on the UI.
    /// </summary>
    internal static class SettingsWindow
    {
        private static bool isSettingsWindowVisible = true;
        private static CoreSettings coreSettings = null;
        private static string currentlySelectedPlugin = "Core";

        /// <summary>
        /// Initializes the Main Menu.
        /// </summary>
        /// <param name="settings">CoreSettings instance to associate with the MainMenu.</param>
        internal static void InitializeCoroutines(CoreSettings settings)
        {
            coreSettings = settings;
            CoroutineHandler.Start(DrawSettingsWindow());
        }

        /// <summary>
        /// Draws the (core/plugins) names as ImGui buttons in a single group.
        /// </summary>
        private static void DrawNames()
        {
            var totalWidthAvailable = ImGui.GetContentRegionAvail().X * 0.2f;
            var buttonSize = new Vector2(totalWidthAvailable, 0);
            ImGui.PushItemWidth(totalWidthAvailable);
            ImGui.BeginGroup();
            bool tmp = true;
            ImGui.Checkbox("##CoreEnableCheckBox", ref tmp);
            ImGui.SameLine();
            if (ImGui.Button("Core##ShowSettingsButton", buttonSize))
            {
                currentlySelectedPlugin = "Core";
            }

            foreach (var pKeyValue in PluginManager.AllPlugins.ToList())
            {
                var pluginContainer = pKeyValue.Value;
                tmp = pluginContainer.Enable;
                if (ImGui.Checkbox($"##{pKeyValue.Key}EnableCheckbox", ref tmp))
                {
                    pluginContainer.Enable = !pluginContainer.Enable;
                    PluginManager.AllPlugins[pKeyValue.Key] = pluginContainer;
                }

                ImGui.SameLine();
                if (ImGui.Button($"{pKeyValue.Key}##ShowSettingsButton", buttonSize))
                {
                    currentlySelectedPlugin = pKeyValue.Key;
                }
            }

            ImGui.PopItemWidth();
            ImGui.EndGroup();
        }

        /// <summary>
        /// Draws the currently selected settings on ImGui.
        /// </summary>
        private static void DrawCurrentlySelectedSettings()
        {
            switch (currentlySelectedPlugin)
            {
                case "Core":
                    ImGui.BeginGroup();
                    if (ImGui.Checkbox("Show terminal on startup", ref coreSettings.ShowTerminal))
                    {
                        Overlay.TerminalWindow = coreSettings.ShowTerminal;
                    }

                    ImGui.Checkbox("Close Game Helper When Game Closes", ref coreSettings.CloseOnGameExit);
                    ImGui.EndGroup();
                    break;
                default:
                    if (PluginManager.AllPlugins.TryGetValue(currentlySelectedPlugin, out var pContainer))
                    {
                        ImGui.BeginGroup();
                        pContainer.Plugin.DrawSettings();
                        ImGui.EndGroup();
                    }

                    break;
            }
        }

        /// <summary>
        /// Draws the Settings Window.
        /// </summary>
        /// <returns>co-routine IWait.</returns>
        private static IEnumerator<Wait> DrawSettingsWindow()
        {
            while (true)
            {
                yield return new Wait(Overlay.OnRender);
                if (NativeMethods.IsKeyPressed(coreSettings.MainMenuHotKey))
                {
                    isSettingsWindowVisible = !isSettingsWindowVisible;
                    if (!isSettingsWindowVisible)
                    {
                        coreSettings.SafeToFile();
                    }
                }

                if (!isSettingsWindowVisible)
                {
                    continue;
                }

                bool isOverlayRunning = true;
                ImGui.SetNextWindowSizeConstraints(new Vector2(800, 600), new Vector2(1024, 1024));
                var isMainMenuExpanded = ImGui.Begin(
                    "Game Overlay Menu",
                    ref isOverlayRunning,
                    ImGuiWindowFlags.NoSavedSettings);
                Overlay.Close = !isOverlayRunning;
                if (!isOverlayRunning)
                {
                    coreSettings.SafeToFile();
                }

                if (!isMainMenuExpanded)
                {
                    ImGui.End();
                    continue;
                }

                DrawNames();
                ImGui.SameLine();
                DrawCurrentlySelectedSettings();
                ImGui.End();
            }
        }
    }
}
