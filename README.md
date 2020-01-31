# Puffin

[![Build Status](https://travis-ci.org/nightblade9/puffin.svg?branch=master)](https://travis-ci.org/nightblade9/puffin-engine)

Puffin is a cross-platform 2D C# game engine built on top of MonoGame. It ships with:

- An extensible entity/component system
- Pre-made components for images, spritesheets, audio, mouse, and keyboard input
- Dynamic font loading/resizing at runtime
- 2D tilemaps
- Fast AABB collision resolution for entities and tilemaps

This project is currently under heavy development.

# Why Use Puffin?

- Focus on your game logic, not low-level plumbing
- Focused and tailored for 2D games
- Runs and makes games for Linux, Windows, and Mac
- Easy to get started and low learning-curve
- Leverage your existing C# skills and the .NET ecosystem

# Getting Started

- Add this repository as a `git submodule` within your project (NuGet packages are not available yet)
- Build it by running `dotnet build`. Note: you will need .NET SDK 3.1 and MonoGame.
- Run `dotnet new console --name MyGame` to create a new game project
- Add the relevant references to Puffin:
    - `dotnet add MyGame reference Puffin\Puffin.Core`
    - `dotnet add MyGame reference Puffin\Puffin.Infrastructure.MonoGame`
    - `dotnet add MyGame package MonoGame.Framework.DesktopGL.Core`
- Download the `Open Sans` font from Google Fonts and add the `-Regular.ttf` to `MyGame/Content`

## Creating Your First Screen

Puffin uses an entity-component architecture where components represent functionality (like a sprite to draw or a text/label to display on-screen). Components live in "screens" which represent screens of your game (eg. main game screen, inventory screen).

To add some functionality to the default screen:

- Create a new `FirstScene` class that extends `Puffin.Core.Scene`
- Add a constructor which calls `this.Add(new Entity().Label("Hello from Puffin!"));`
- Modify your game class to override `protected void Ready` and call `this.ShowScene(new FirstScene())`
- Type `dotnet run` or press F5 in VSCode. It should run and you should see `Hello from Puffin!`

To add an image:

```csharp
this.Add(new Entity().Add(new SpriteComponent("Bird.png")));
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

This allows you to write code like `entity.Get<KeyboardComponent>().IsKeyDown(CustomAction.Next)`, which would return `true` if either the space or enter keys are currently held down.

# TileMaps

If your game includes a 2D map on a grid, you can use the `Puffin.Core.TileMap` class. An example below creates a new 20x10 map of floor tiles with a border of non-walkable wall tiles.

It assumes your spritesheet has a floor tile then a wall tile (from left to right). Entities need a collision component if they are not to move on solid tiles.

```csharp
var map = new TileMap(20, 10, "dungeon.png", 32, 32);
map.Define("Floor", 0, 0);
map.Define("Wall", 1, 0, true);

for (var y = 0; y < 10; y++) {
    for (var x = 0; x < 20; x++) {
        if (x == 0 || y == 0 || x == 19 || y == 9) {
            map[x, y] = "Wall";
        } else {
            map[x, y] = "Floor";
        }
    }
}
```

# Collision Detection and Response

## Collision/Overlap Detection

If you simply want collision detection (did two things collide? Are they overlapping?) You can add an `OverlapComponent` to your entity (`.Overlap(...)`). This allows you to specify functions when another entity with an `OverlapComponent` overlaps yours.

```csharp
var coin = new Entity().Sprite("coin.png").Overlap(40, 40, 0, 0, (e) => {
    if (e == player) {
        // ... play coin noise ...
        // ... increment coins by +1
    }
});
```

The second set of coordinates specify an offset of the overlap, relative to the entity origin. This is useful for things like creating an overlap area larger than an entity's sprite, or noting overlap only if the player walks into the bottom part of a door:

```csharp
var door = new Entity().Sprite("door.png") // eg. 32x60 image
    .Overlap(32, 20, 0, 40); // 32x20 overlap that's at (0, 40)
```

## Collision Response

Puffin provides built-in support for AABB (axis-aligned or non-rotated bounding boxes), including high-speed ones, and is resistent to "tunneling" (high-speed, small objects going through solid walls/etc. because they move so fast).

By default, Puffin checks for entity/tile collisions (with solid tiles) and entity/entity collisions (as long as both have a `CollisionComponent`):

```csharp
var player = new Entity().Colour(32, 32, 0xFFFFFF).FourWayMovement(200).Collide(32, 32, true);
var wall = new Entity().Colour(128, 16, 0x666666).Collide(128, 16).Move(50, 50);
```

The player will collide with the wall in all directions, as well as any tiles with `solid=true` for any `TileMap` instances in the current `Scene`.

The third parameter, `slideOnCollide`, if true, makes the entity slide along the object in the non-colliding direction, rather than stopping abruptly. It is useful for things like smooth character/NPC movement around solid objects.

# Performance

I ran a simple MonoGame project and an analogous Puffin project, where I render as many copies of a sprite as possible until the FPS drops from 60 to 50.

Both MonoGame and Puffin exhibit similar performance characteristics; on my test machine, MonoGame reached around 2200 sprites, while Puffin reached around 1900 sprites.

# Publishing your Game

To create a self-contained zip (including .NET Core), you can publish your app via the usual `dotnet publish` command. For example, to make a Linux build, run `dotnet publish -c Release -r linux-x64 -o publish`.

Make sure you copy all your content (sprites, sound effects, etc.) into the `publish` directory.

You can also compress the directory for a smaller file-size.

For a reference Powershell script, see [Ali the Android's publish.ps1 script](https://github.com/deengames/ali-the-android/blob/master/publish.ps1). We intend to provide a similar script in the future.

# Development

- To build `Puffin`, you need to install `MonoGame`.
- To build documentation, download `docfx`. Run it by entering `Puffin.Docs` and running `docfx docfx.json --serve`.
