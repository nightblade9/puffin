# Puffin

Cross-platform 2D C# game engine. This project is under heavy development.

**Builds:**
- Master: [![Build Status](https://travis-ci.org/nightblade9/puffin.svg?branch=master)](https://travis-ci.org/nightblade9/puffin-engine)
- Dev: [![Build Status](https://travis-ci.org/nightblade9/puffin.svg?branch=dev)](https://travis-ci.org/nightblade9/puffin-engine)

# Why Use Puffin?

- Focus on your game logic, not low-level plumbing
- Focused and tailored for 2D games
- Runs and makes games for Linux, Windows, and Mac
- Easy to get started and low learning-curve
- Leverage your existing C# skills and the .NET ecosystem

# Getting Started

- Clone this repository (binaries are not available yet)
- Build it by running `dotnet build`. Note: you will need .NET SDK 3.1 and MonoGame.
- Run `dotnet run --project Puffin.CommandLineInterface` to build a new project template
- Type `cd NewPuffin` and `dotnet run` to build/run the template project

You should be greeted with an empty screen with a placeholder image.

## Creating Your First Screen

Puffin uses an entity-component architecture where components represent functionality (like a sprite to draw or a text/label to display on-screen). Components live in "screens" which represent screens of your game (eg. main game screen, inventory screen).

To add some functionality to the default screen:

- Open up `MyGame.cs`. You'll see it changes the screen to `CoreGameScreen.cs` in the `Ready` function.
- Open up `CoreGameScreen.cs`. It simply adds a new entity with a new sprite:

```csharp
this.Add(
    new Entity()
        .Add(new SpriteComponent("Bird.png")));
```

This creates a new entity, adds a `SpriteComponent` to it which should appear with the image of `Bird.png`, and adds the entity to the screen. When the game runs, it renders that entity at its position.
