using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;

#nullable enable
namespace AutoFishRework
{
    public enum GameStateType
    {
        Default,
        Fishing,
        CollectingFish,
        WaitingForFishing,
        ExampleState
    }

    public abstract record GameState
    {
        private GameState(Farmer Player, ModEntry Mod, IReflectionHelper Reflection, GameStateType previousState)
        {
            this.Player = Player;
            this.Mod = Mod;
            this.Reflection = Reflection;
            this.PreviousStateType = previousState;
        }

        protected int? _fishingPodIndex;  // Add here
        protected int? _facingDirection;
        
        private Farmer Player { get; }
        private ModEntry Mod { get; }
        private ModConfig Config => this.Mod.Config;
        private IReflectionHelper Reflection { get; }
        protected IMonitor Monitor => this.Mod.Monitor;
        
        protected GameStateType PreviousStateType { get; private set; }
        
        public abstract GameStateType StateType { get; }
        
        public abstract GameState Next();

        public static GameState DefaultState(Farmer Player, ModEntry Mod, IReflectionHelper Reflection)
        {
            return new DefaultIdleState(Player, Mod, Reflection, GameStateType.Default);
        }

        protected GameState(GameState original)
        {
            this.Player = original.Player;
            this.Mod = original.Mod;
            this.Reflection = original.Reflection;
            this.PreviousStateType = original.StateType;
        }
        
        private FishingRod? GetUsingFishingRod()
        {
            return this.Player.CurrentTool is FishingRod currentTool && 
                   (currentTool.inUse() || currentTool.castedButBobberStillInAir) ? currentTool : null;
        }

        private void ClickFishingRod()
        {
            this.Player.CurrentTool.DoFunction(
                this.Player.currentLocation, 
                (int)this.Player.GetToolLocation(false).X, 
                (int)this.Player.GetToolLocation(false).Y, 
                1, 
                this.Player
            );
        }
        
        protected void LogStateChange()
        {
            if (PreviousStateType != StateType)
            {
                this.Monitor.Log($"Transitioning from {PreviousStateType} to {StateType}", LogLevel.Debug);
            }
        }

        public record DefaultIdleState : GameState
        {
            public override GameStateType StateType => GameStateType.Default;
            
            // Constructor for initial state
            public DefaultIdleState(Farmer Player, ModEntry Mod, IReflectionHelper Reflection, GameStateType previousState) 
                : base(Player, Mod, Reflection, previousState)
            {
                this.Mod.shouldEndFishing = false;
                LogStateChange();
            }

            // Constructor for state transitions
            public DefaultIdleState(GameState original) 
                : base(original)
            {
                this.Mod.shouldEndFishing = false;
                LogStateChange();
            }

            public override GameState Next()
            {
                FishingRod? rod = this.GetUsingFishingRod();
                if (rod == null)
                    return this; 
       
                if (rod.isTimingCast)
                {
                    if (this.Config.FastBite)
                        rod.castingPower = 1f;
                    return new Fishing(this);
                }
        
                return this;
            }
        } 

        public record Fishing : GameState
        {
            public override GameStateType StateType => GameStateType.Fishing;

            public Fishing(GameState original) : base(original)
            {
                LogStateChange();
            }

            public override GameState Next()
            {
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.W) || 
                    keyboardState.IsKeyDown(Keys.A) || 
                    keyboardState.IsKeyDown(Keys.S) || 
                    keyboardState.IsKeyDown(Keys.D))
                {
                    this.Monitor.Log($"Setting shouldEndFishing to true", LogLevel.Debug);
                    this.Mod.shouldEndFishing = true;
                    
                    this.Monitor.Log($"ShouldEndFishing? --> {this.Mod.shouldEndFishing}", LogLevel.Debug); 
                }

                FishingRod? rod = this.GetUsingFishingRod();
                if (rod == null)
                {
                    return new CollectingFish(this); // Use the transition constructor>
                }
                
                // Max power when timing cast
                if (rod.isTimingCast)
                {
                    if (this.Config.FastBite)
                        rod.castingPower = 1f;
                    return this;
                }

                // Wait states
                if (rod.isCasting || rod.castedButBobberStillInAir)
                    return this;
                
                if (rod.isFishing)
                {
                    if (!rod.isNibbling)
                    {
                        if (this.Config.FastBite && (double)rod.timeUntilFishingBite > 0.0)
                            rod.timeUntilFishingBite = 0f; 
                        return this;
                    }
                    else
                    {
                        // Fish is nibbling, click to catch
                        if (!rod.isReeling && !rod.hit)
                        {
                            this.ClickFishingRod();
                        }
                        //return new CollectingFish(this);
                        return this;
                    }
                }
                
                bool fishCaught = this.Reflection.GetField<bool>(rod, "fishCaught").GetValue();
                int whichFish = this.Reflection.GetField<int>(rod, "whichFish").GetValue();
                string itemCategory = this.Reflection.GetField<string>(rod, "itemCategory").GetValue();
                bool lastCatchWasJunk = this.Reflection.GetField<bool>(rod, "lastCatchWasJunk").GetValue();

                // Check if we've caught something and waiting for click
                if (fishCaught && whichFish != -1)
                {
                    this.Monitor.Log($"Caught item {whichFish} of type {itemCategory}, isJunk: {lastCatchWasJunk}, waiting for click", LogLevel.Debug);
            
                    // You could simulate the click here if you want
                    // rod.DoFunction(Game1.player.currentLocation, 0, 0, 1, Game1.player);
            
                    return new CollectingFish(this);
                } 
                
                return this;
            }
        }
       
        public record CollectingFish : GameState
        {
            public override GameStateType StateType => GameStateType.CollectingFish;

            public CollectingFish(GameState original) : base(original)
            {
                LogStateChange();
            }

            public override GameState Next()
            {
                FishingRod? rod = this.GetUsingFishingRod();

                if (rod != null)
                {
                    bool fishCaught = this.Reflection.GetField<bool>(rod, "fishCaught").GetValue();
                    int whichFish = this.Reflection.GetField<int>(rod, "whichFish").GetValue();
        
                    if (fishCaught && whichFish != -1)
                    {
                        int fishQuality = this.Reflection.GetField<int>(rod, "fishQuality").GetValue();
                        string itemCategory = this.Reflection.GetField<string>(rod, "itemCategory").GetValue();
                        bool fromFishPond = this.Reflection.GetField<bool>(rod, "fromFishPond").GetValue();

                        // Create and add the item
                        Item itemToAdd;
                        if (itemCategory == "Object")
                        {
                            itemToAdd = new StardewValley.Object(whichFish, 1, quality: fishQuality);
                            if (whichFish == GameLocation.CAROLINES_NECKLACE_ITEM)
                            {
                                (itemToAdd as StardewValley.Object).questItem.Value = true;
                            }
                        }
                        else
                        {
                            this.Monitor.Log($"Unknown item category: {itemCategory}", LogLevel.Error);
                            return this;
                        }

                        // Add to inventory
                        Game1.player.addItemToInventoryBool(itemToAdd);
            
                        // Play sound and cleanup
                        Game1.player.currentLocation.localSound("coin");
                        this.Reflection.GetField<int>(rod, "recastTimerMs").SetValue(200);
            
                        // Call doneFishing to clean up the rod state
                        this.Reflection.GetMethod(rod, "doneFishing").Invoke(Game1.player, !fromFishPond);

                        return new WaitingForFishing(this);
                    }
                }
                else
                {
                    this.Monitor.Log($"ROD IS NULL ERROR AT STATE {StateType}", LogLevel.Debug);
                    return new WaitingForFishing(this);
                }

                
                return this;
            }
        }

        public record WaitingForFishing : GameState
        {
            public override GameStateType StateType => GameStateType.WaitingForFishing;
            private bool hasInitiatedRecast = false;  // Add this flag
            private int StartFishingTime = 600;
            private int EndFishingTime = 2200;
            
            // private int? _facingDirection;
            // private int? _fishingPodIndex;

            public WaitingForFishing(GameState original) : base(original)
            {
                LogStateChange();
            }

            public override GameState Next()
            {
                
                int currentTime = Game1.timeOfDay;
                bool isAfterEndTime = currentTime >= EndFishingTime;
                bool isBeforeStartTime = currentTime <= 200 || currentTime < StartFishingTime;

                if (isAfterEndTime || isBeforeStartTime)
                {
                    this.Monitor.Log($"Outside fishing hours (Current time: {currentTime})", LogLevel.Debug);
                    return new DefaultIdleState(this);
                }
                
                

                double staminaToEat = 30.0;
                
                
                // this.Monitor.Log($"END ?? -> {this.Mod.shouldEndFishing}", LogLevel.Debug);
                
                // test for ending the fishing if a movement was detected previously
                if (this.Mod.shouldEndFishing)
                {
                    this.Monitor.Log($"ENDING XXXXXX ", LogLevel.Debug); 
                    return new DefaultIdleState(this);
                }
                else
                {
                    this.Monitor.Log($"NOT ending fishing...", LogLevel.Debug); 
                }
                
               
                // tries to eat and recast
                if (this.Player != null && (double)this.Player.Stamina < staminaToEat &&
                    this.Player.CurrentTool is FishingRod && this.GetFood() != null)
                {
                    this._fishingPodIndex = this.Player.CurrentToolIndex;
                    this._facingDirection = this.Player.FacingDirection;
                    this.Player.CurrentToolIndex = this.Config.FoodIndex - 1;
                    this.Player.eatHeldObject();
                    
                    this.Monitor.Log($"Player ate food at state: {StateType}", LogLevel.Debug);

                    return this;
                }

                if (this.Player.CurrentTool is FishingRod currentTool)
                {
                    hasInitiatedRecast = true;  // Set flag to prevent multiple recasts
            
                    // Start the casting animation
                    ((Tool)currentTool).beginUsing(
                        this.Player.currentLocation, 
                        (int)this.Player.GetToolLocation(false).X, 
                        (int)this.Player.GetToolLocation(false).Y, 
                        this.Player
                    );
            
                    this.Monitor.Log($"Initiating recast from state: {StateType}", LogLevel.Debug);
                    
                    FishingRod? rod2 = this.GetUsingFishingRod();
    
                    // If we have a rod in use, check if it's ready for timing
                    if (rod2 != null && rod2.isTimingCast)
                    {
                        if (this.Config.FastBite)
                            rod2.castingPower = 1f;
                        return new Fishing(this);
                    }

                    return this;

                }
 
                

 
                
                FishingRod? rod = this.GetUsingFishingRod();
        
                // If we have a rod in use, check if it's ready for timing
                // if (rod != null && rod.isTimingCast)
                // {
                //     if (this.Config.FastBite)
                //         rod.castingPower = 1f;
                //     return new Fishing(this);
                // }
        
                // If no rod is in use and we haven't tried recasting yet
                if (rod == null && !hasInitiatedRecast)
                {
                    if (this.Player.CurrentTool is FishingRod currentTool2)
                    {
                        hasInitiatedRecast = true;  // Set flag to prevent multiple recasts
                
                        // Start the casting animation
                        ((Tool)currentTool2).beginUsing(
                            this.Player.currentLocation, 
                            (int)this.Player.GetToolLocation(false).X, 
                            (int)this.Player.GetToolLocation(false).Y, 
                            this.Player
                        );
                
                        this.Monitor.Log($"Initiating recast from state: {StateType}", LogLevel.Debug);
                        
                        FishingRod? rod2 = this.GetUsingFishingRod();
        
                        // If we have a rod in use, check if it's ready for timing
                        if (rod2 != null && rod2.isTimingCast)
                        {
                            if (this.Config.FastBite)
                                rod2.castingPower = 1f;
                            return new Fishing(this);
                        }
                    
                    }
                    else
                    {
                        if (!Player.isEating)
                        {
                            // switching back to fishing rod
                            this.Monitor.Log($"Initiating recast from state: {StateType}", LogLevel.Debug); 
                            this.Player.CurrentToolIndex = 2;
                        }
                    }
                }
                
                
                if ((double)this.Player.stamina < staminaToEat && !this.Player.isEating)
                {
                    return new DefaultIdleState(this);
                }
                
        
                return this; 
            }

            
            private StardewValley.Object? GetFood()
            {
                if (this.Config.FoodIndex > 0 && 
                    this.Config.FoodIndex <= this.Player.Items.Count && 
                    this.Player.Items[this.Config.FoodIndex - 1] is StardewValley.Object o && 
                    o.Edibility > 0)
                {
                    var foodValue = GetFoodValue(o);
                    if (foodValue.stamina > 0 && foodValue.health >= 0)
                        return o;
                }
                return null;
            }

            private static (int stamina, int health) GetFoodValue(StardewValley.Object o)
            {
                string[] strArray = Game1.objectInformation[o.ParentSheetIndex].Split('/', StringSplitOptions.None);
                return (Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[1]));
            }
            
        }

    }
}