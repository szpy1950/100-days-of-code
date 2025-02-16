using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using Microsoft.Xna.Framework;
using StardewValley.Util;

namespace AutoFishRework
{
    public class ModEntry : Mod
    {
        public ModConfig Config;
        private GameState currentState;
        public bool shouldEndFishing { get; set; } = false;
        public int StartFishingTime { get; set; } = 600;  // 6:00 AM
        public int EndFishingTime { get; set; } = 2200;   // 10:00 PM
        
        // protected IMonitor Monitor => this.Monitor;

        public override void Entry(IModHelper helper)
        {
            // Load config.json
            this.Config = helper.ReadConfig<ModConfig>();

            // Subscribe to update ticked event
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Don't process if game isn't ready
            if (!Context.IsWorldReady || Game1.player == null)
                return;

            // Initialize state if needed
            if (this.currentState == null)
            {
                this.currentState = GameState.DefaultState(Game1.player, this, this.Helper.Reflection);
            }

            // Update state
            this.currentState = this.currentState.Next();
        }
    }
}