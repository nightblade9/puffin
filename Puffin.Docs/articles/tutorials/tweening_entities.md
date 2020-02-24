# Tweening Entities

Puffin supports modest tweens: you can tween entity positions from one/start position to an end position over a specified duration. You can also specify an optional callback to invoke when the tween completes (good for chanining or doing things when the tween completes).

You can access the tweening API via the `Scene` class (in your instances that subclass `Scene`) or via the `Entity` itself.

## Tweening an Entity

The sample below moves the player from `(100, 200)` to `(200, 300)` over a second, and moves the pet from `(800, 800)` to `(200, 300)` over two seconds.

```csharp
override public void Ready()
{
    base.Ready();
    var playerReached = false;
    var petMoving = true;

    this.Add(new Entity().Sprite("player.png").Tween(new Tuple<int, int>(100, 200), new Tuple<int, int>(200, 300), 1, () => playerReached = true));
    this.Add(new Entity().Sprite("rabbit.png").Tween(new Tuple<int, int>(800, 800), new Tuple<int, int>(200, 300), 2, () => petMoving = false));
}
```

Some caveats and points to keep in mind:

- Tweens include `Start` function that's called automatically on start
- Tweens include a `Stop` function. Stopping aborts immediately - it doesn't move the entity to the end position.
- Every entity can have only one tween at a time (adding a new one stops/aborts the previous one).
- Even if the game slows down and updates slowly/infrequently, the tween won't over-shoot; it'll finish with the entity in the end position.
- The entity moves at a steady, linear velocity; Puffin doesn't support ease functions.