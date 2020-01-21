# Puffin

[![Build Status](https://travis-ci.org/nightblade9/puffin.svg?branch=master)](https://travis-ci.org/nightblade9/puffin-engine)

Cross-platform 2D C# game engine. This project is under heavy development.

# Why Use Puffin?

- Focus on your game logic, not low-level plumbing
- Focused and tailored for 2D games
- Runs and makes games for Linux, Windows, and Mac
- Easy to get started and low learning-curve
- Leverage your existing C# skills and the .NET ecosystem

# Getting Started

- Add this repository as a `git submodule` within your project (NuGet packages are not available yet)
- Build it by running `dotnet build`. Note: you will need .NET SDK 3.1 and MonoGame.
- Copy/paste `Templates/MonoGame` into the root of your repo
- Go to the repo root and add references to Puffin:
    - `dotnet add MyGame.csproj reference puffin/Puffin.Core`
    - `dotnet add MyGame.csproj reference puffin/Puffin.UI` (if you want buttons/etc.)
- Type `dotnet run` or press F5 in VSCode to verify it works

You should be greeted with an empty screen with an image.

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

# UI Controls

Puffin includes a separate `Puffin.UI` assembly which includes UI controls. Note that they rely on images in `Content/Puffin/UI` to work.

## Button

A button with an image, text label, and on-click event handler. To create:

```csharp
var button = new Button("Click me!", () => this.points++).Move(16, 16);
```

This creates a button with the caption `Click me!` Clicking the button increments the local variable `points` by one. The button sits at `(16, 16)` on screen.

To reskin the button, change the `Content/Puffin/UI/Button.png` image to something else. Note that you cannot yet resize the image.

# Keyboard Handling

For keyboard input, Puffin doesn't expose key-press information directly; instead, it exposes information about `PuffinAction`s. Each `PuffinAction` maps to one or more keys (eg. by default, the `W` key maps to `PuffinAction.Up`).

This allows developers/players to arbitrarily rebind actions keys without rewriting lots of code.

## Creating Custom Actions

To create your own custom actions, simply create a new `Enum` and add actions to your game constructor, like so:

```csharp
// MyGame.cs
class MyGame : PuffinGame {

    public enum CustomAction {
        Next,
        Previous,
    }

    public MyGame() {
        this.actionToKeys[CustomAction.Next] = new List<Keys>()
        {
            Keys.Space,
            Keys.Enter,
        };
    }
}
```

This allows you to write code like `entity.GetIfHas<KeyboardComponent>().IsKeyDown(CustomAction.Next)`, which would return `true` if either the space or enter keys are currently held down.

# Is Puffin Slow?

I ran a simple MonoGame project and an analogous Puffin project, where I render as many copies of a sprite as possible until the FPS drops from 60 to 50.

Both MonoGame and Puffin exhibit similar performance characteristics; on my test machine, MonoGame reached around 2200 sprites, while Puffin reached around 2000-2100 sprites.