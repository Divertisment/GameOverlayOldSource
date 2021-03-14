﻿// <copyright file="AreaLoadingState.cs" company="None">
// Copyright (c) None. All rights reserved.
// </copyright>

namespace GameHelper.RemoteObjects.States
{
    using System;
    using System.Collections.Generic;
    using Coroutine;
    using GameHelper.CoroutineEvents;
    using GameHelper.RemoteEnums;
    using GameOffsets.Objects.States;
    using ImGuiNET;

    /// <summary>
    /// Reads AreaLoadingState Game Object.
    /// </summary>
    public sealed class AreaLoadingState : RemoteObjectBase
    {
        private AreaLoadingStateOffset lastCache = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaLoadingState"/> class.
        /// </summary>
        /// <param name="address">address of the remote memory object.</param>
        internal AreaLoadingState(IntPtr address)
            : base(address)
        {
            CoroutineHandler.Start(this.OnPerFrame());
            CoroutineHandler.Start(this.OnGameStateChange());
        }

        /// <summary>
        /// Gets the game current Area Name.
        /// </summary>
        public string CurrentAreaName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the game is in loading screen or not.
        /// </summary>
        internal bool IsLoading { get; private set; }

        /// <summary>
        /// Converts the <see cref="AreaLoadingState"/> class data to ImGui.
        /// </summary>
        internal override void ToImGui()
        {
            base.ToImGui();
            ImGui.Text($"Current Area Name: {this.CurrentAreaName}");
            ImGui.Text($"Is Loading Screen: {this.IsLoading}");
            ImGui.Text($"Total Loading Time(ms): {this.lastCache.TotalLoadingScreenTimeMs}");
        }

        /// <inheritdoc/>
        protected override void CleanUpData()
        {
            this.lastCache = default;
            this.CurrentAreaName = string.Empty;
        }

        /// <inheritdoc/>
        protected override void UpdateData(bool hasAddressChanged)
        {
            var reader = Core.Process.Handle;
            var data = reader.ReadMemory<AreaLoadingStateOffset>(this.Address);
            this.IsLoading = data.IsLoading == 0x01;
            bool hasAreaChanged = false;
            if (data.CurrentAreaName.Buffer != IntPtr.Zero &&
                !this.IsLoading &&
                data.TotalLoadingScreenTimeMs > this.lastCache.TotalLoadingScreenTimeMs)
            {
                string areaName = reader.ReadStdWString(data.CurrentAreaName);
                this.CurrentAreaName = areaName;
                this.lastCache = data;
                hasAreaChanged = true;
            }

            if (hasAreaChanged)
            {
                CoroutineHandler.InvokeLater(new Wait(0.05d), () =>
                {
                    CoroutineHandler.RaiseEvent(RemoteEvents.AreaChanged);
                });
            }
        }

        private IEnumerator<Wait> OnPerFrame()
        {
            while (true)
            {
                yield return new Wait(GameHelperEvents.PerFrameDataUpdate);
                if (this.Address != IntPtr.Zero)
                {
                    this.UpdateData(false);
                }
            }
        }

        private IEnumerator<Wait> OnGameStateChange()
        {
            while (true)
            {
                yield return new Wait(RemoteEvents.StateChanged);
                if (Core.States.GameCurrentState == GameStateTypes.PreGameState)
                {
                    this.CleanUpData();
                }
            }
        }
    }
}
