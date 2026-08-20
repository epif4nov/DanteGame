````markdown
# Dante's Inferno: Circulus Nonus

> A 2D platformer inspired by Dante Alighieri's *Divine Comedy*, developed with C#, .NET 8 and MonoGame.

## Overview

**Dante's Inferno: Circulus Nonus** is a 2D platformer set in the ninth circle of Hell.

The player descends into **Cocytus**, the frozen lake at the bottom of Hell, facing enemies, environmental hazards and increasingly dangerous challenges.

The project was created to explore game development with C#, object-oriented programming, game architecture and the MonoGame framework.

## Features

- 2D platformer gameplay
- Player movement and controls
- Collision detection
- Enemy system
- Combat mechanics
- Environmental hazards and traps
- Level-based game structure
- Game state management
- Custom game assets
- Object-oriented game architecture

## Tech Stack

| Technology | Purpose |
|------------|---------|
| C# | Programming language |
| .NET 8 | Runtime and development platform |
| MonoGame | Game framework |
| MVC | Application architecture |
| Visual Studio | Development environment |

## Game Concept

The game takes inspiration from the final part of Dante's journey through Hell.

The player reaches the **ninth circle**, where the landscape changes from fire and stone to an enormous frozen lake.

The main environment is **Cocytus**, where traitors are imprisoned in the ice.

The project focuses on translating this setting into a 2D gameplay experience while keeping the gameplay centered around exploration, movement, combat and environmental hazards.

## Gameplay

The player can:

- Move through the level
- Jump across platforms
- Interact with the environment
- Avoid traps
- Fight enemies
- Progress through the level
- Reach the final encounter

The environment is designed around the frozen landscape of the ninth circle.

## Architecture

The project follows an MVC-inspired structure to separate game logic, state and presentation.

```text
                    ┌─────────────────┐
                    │      Game       │
                    └────────┬────────┘
                             │
              ┌──────────────┼──────────────┐
              │              │              │
              ▼              ▼              ▼
       ┌────────────┐ ┌────────────┐ ┌────────────┐
       │   Model    │ │    View    │ │ Controller │
       └────────────┘ └────────────┘ └────────────┘
              │              │              │
              │              │              │
              ▼              ▼              ▼
         Game state       Rendering       Input
         Entities         Sprites         Gameplay
         Mechanics        UI              Control
````

## Project Structure

```text
DanteGame/
│
├── DanteGame/
│   ├── ...
│   └── ...
│
├── DanteGame.slnx
└── README.md
```

The project source code is organized around the core game systems and MonoGame application lifecycle.

## Core Systems

### Player

Responsible for:

* Movement
* Jumping
* Player state
* Collision interaction
* Combat interaction

### Enemies

Enemies provide the main combat challenge and interact with the player and environment.

### Traps

Environmental hazards are used to make movement through the level more challenging.

### Level

The level system defines the game environment, platforms, obstacles and gameplay progression.

### Rendering

MonoGame is used to render:

* Characters
* Environment
* Objects
* Effects
* UI elements

## Controls

| Key     | Action            |
| ------- | ----------------- |
| `A / D` | Move left / right |
| `Space` | Jump              |
| `...`   | Other actions     |

> Controls may change as the project develops.

## Getting Started

### Requirements

* .NET 8 SDK
* Visual Studio 2022 or another compatible .NET IDE
* MonoGame development environment

### Clone the repository

```bash
git clone https://github.com/epif4nov/DanteGame.git
cd DanteGame
```

### Run the project

Open the solution:

```text
DanteGame.slnx
```

Build and run the project from your IDE.

## Development Goals

The project is intended to provide practical experience with:

* C# game development
* Object-oriented programming
* MonoGame
* Game loops
* Input handling
* Collision detection
* Entity management
* Rendering
* Game architecture
* Separation of responsibilities

## Future Improvements

Possible future improvements include:

* Additional levels
* More enemy types
* Expanded combat system
* Boss encounters
* Additional environmental mechanics
* Improved animations
* Sound effects and music
* Save/load system
* More detailed UI
* Additional visual effects

## Inspiration

The project is inspired by **Dante Alighieri's *Divine Comedy***, particularly the depiction of the ninth circle of Hell and Cocytus.

The game is an original implementation and is intended as a programming and game-development project.

## Status

🚧 **In development**

The project is actively used to explore 2D game development with C#, .NET and MonoGame.

````

### Description репозитория

Поставь:

> **2D platformer inspired by Dante's Divine Comedy, built with C#, .NET 8 and MonoGame.**

### Topics

```text
csharp
dotnet
dotnet8
monogame
gamedev
game-development
2d-game
platformer
mvc
oop
````
