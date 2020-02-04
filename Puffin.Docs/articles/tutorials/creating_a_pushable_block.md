# Tutorial: Creating a Pushable Block

In this tutorial, we create a relatively simple pushable block; the player constantly walks against the block to move it. You can use these as a basis for block puzzles, or to add a touch more interactivity to your scenes. We use a simple approach where we apply the player's velocity to the block to move it when we see a player/block collision.

Puffin supports movement via the `.FourWayMovement` extension. The `FourWayMovementComponent` component listens to keyboard events and applies an appropriate velocity to the player. The collision system also supports callbacks, which trigger after resolving the collision (by moving the moving entity so it stops just at the point of collision). Combining these two, we simply add a collision callback which applies the player's velocity to the block.

## Code

Summary:
- Create a movable, collidable player
- Create a simple block entity
- Add a collision handler that moves the block in the pushed direction
- Add an update handler that resets the block velocity to zero

In the collision handler, we look at what axis the collision resolved on (X or Y). If the player collided on the X-axis, the block gets the player's X-velocity; if the player collided on the Y-axis, the block gets the player's Y-velocity.

```csharp
public CoreGameScene()
{
    var player = new Entity().Colour(0xFFFFFF, 32, 32)
        .Move(100, 100)
        .FourWayMovement(100)
        .Collide(32, 32, true);
    
    this.Add(player);

    var pushable = new Entity().Colour(0x0000AA, 32, 32)
        .Move(50, 50);
    
    pushable.Collide(32, 32, (e, collisionAxis) => {
        if (e == player)
        {
            var vx = collisionAxis == "X" ? (int)player.VelocityX : 0;
            var vy = collisionAxis == "Y" ? (int)player.VelocityY : 0;
            pushable.Velocity(vx, vy);
        }
    }).OnUpdate((elapsed) => {
        pushable.Velocity(0, 0);
    });

    this.Add(pushable);
}
```

## Creating a Sliding Block

If you wanted to create a block that the player can "push away" and that slowly slides to a stop, you can achieve that with a couple of small changes:

- Set the block's velocity to some multiple of the player's velocity (eg. 2x). Increase this to push the block farther.
- In update, instead of setting the block velocity to zero, multiply it by a constant like `0.95f` (95%). Increase this to make the block slide farther.

Replace the collide/update callbacks with this:

```csharp
pushable.Collide(32, 32, (e, axis) => {
    if (e == player)
    {
        var vx = axis == "X" ? (int)player.VelocityX * 2 : 0;
        var vy = axis == "Y" ? (int)player.VelocityY * 2 : 0;
        pushable.Velocity(vx, vy);
    }
}).OnUpdate((elapsed) => {
    pushable.VelocityX *= 0.95f;
    pushable.VelocityY *= 0.95f;
});
```