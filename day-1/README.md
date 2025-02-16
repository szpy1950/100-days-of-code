# AutoFish Rework - Stardew Valley Mod

## About

AutoFish Rework is a mod for Stardew Valley that automates the fishing process. It handles casting, catching fish, and managing stamina by automatically consuming food when needed. The mod includes quality-of-life features like fast bite times and automatic recast capabilities.

## Game & API Versions
- Stardew Valley: 1.5.6
- SMAPI: 3.18.6

## Setup Instructions (Arch Linux)

1. Install .NET SDK:
```bash
sudo pacman -S dotnet-sdk
sudo pacman -S dotnet-runtime-5.0
```

2. Install JetBrains Rider:
   - Install JetBrains Toolbox from AUR or download from JetBrains website
   - Use Toolbox to install Rider IDE

## Project Structure

The mod consists of three main files:

### ModEntry.cs
- Entry point of the mod
- Handles configuration loading
- Manages game state updates
- Controls fishing time windows (6:00 AM to 10:00 PM)

### ModConfig.cs
- Defines configuration options:
  - FoodIndex: Inventory slot for food items
  - FastBite: Toggle for instant fish bites

### GameState.cs
- Implements state machine pattern for fishing automation
- States include:
  - Default: Initial idle state
  - Fishing: Active fishing state
  - CollectingFish: Handling caught fish
  - WaitingForFishing: Managing recast and stamina

## In-Game Usage

1. Equip your fishing rod
2. Place food in the configured inventory slot (default: slot 12)
3. Start fishing normally
4. The mod will automatically:
   - Cast the line
   - Catch fish when there's a bite
   - Manage stamina by eating when needed
   - Recast the line after catching fish

Note: Use WASD keys to stop auto-fishing.

## Development Skills Learned

1. Reverse Engineering
   - Used Rider's decompilation tools to study existing Stardew Valley mods
   - Learned to read and understand compiled C# code

2. .NET Development
   - Created a C# project using .NET 5.0
   - Worked with state machine patterns
   - Handled game events and reflection

3. Mod Development
   - Set up NuGet package management for Stardew Valley SDK
   - Referenced required game assemblies:
     - StardewValley.dll
     - StardewModdingAPI.dll
     - MonoGame.Framework.dll
   - Learned to compile mods into DLL format

4. Development Environment
   - Configured Rider IDE for mod development
   - Set up build configurations for Stardew Valley modding
   - Managed project dependencies through .csproj files

## Notes
- Tested on Arch Linux with .NET 5.0
- Requires SMAPI (Stardew Modding API)
- Built using Pathoschild.Stardew.ModBuildConfig

## Future Development
Feel free to contribute or modify the mod. The state machine pattern makes it easy to add new features or modify existing behavior.
