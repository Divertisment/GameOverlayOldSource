﻿// <copyright file="PluginBase.cs" company="None">
// Copyright (c) None. All rights reserved.
// </copyright>

namespace GameHelper.Plugin
{
#pragma warning disable SA1401 // Fields should be private
    /// <summary>
    /// Interface for creating plugins.
    /// </summary>
    /// <typeparam name="TSettings">plugin's setting class name.</typeparam>
    public abstract class PluginBase<TSettings> : IPlugin
        where TSettings : ISettings, new()
    {
        /// <summary>
        /// Gets or sets the plugin root directory folder.
        /// </summary>
        public string DllDirectory;

        /// <summary>
        /// Gets or sets the plugin settings.
        /// </summary>
        public TSettings Settings = new TSettings();

        /// <inheritdoc/>
        public abstract void OnDisable();

        /// <inheritdoc/>
        public abstract void OnEnable();

        /// <inheritdoc/>
        public abstract void OnLoad();

        /// <inheritdoc/>
        public abstract void DrawSettings();

        /// <inheritdoc/>
        public abstract void DrawUI();
    }
#pragma warning restore SA1401 // Fields should be private
}