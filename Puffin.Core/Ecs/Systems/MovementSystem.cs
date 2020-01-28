using System;
using System.Collections.Generic;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs.Systems
{
    class MovementSystem : ISystem
    {
        private IList<Entity> entities = new List<Entity>();
        private IList<Entity> collidables = new List<Entity>();

        public void OnAddEntity(Entity entity)
        {
            if (entity.GetIfHas<FourWayMovementComponent>() != null)
            {
                this.entities.Add(entity);
            }

            if (entity.GetIfHas<CollisionComponent>() != null)
            {
                this.collidables.Add(entity);
            }
        }

        public void OnRemoveEntity(Entity entity)
        {
            this.entities.Remove(entity);
        }

        public void OnUpdate(TimeSpan elapsed)
        {
            foreach (var entity in this.entities)
            {
                this.ProcessMovement(elapsed, entity);
            }
        }

        private void ProcessMovement(TimeSpan elapsed, Entity entity)
        {
            var movementComponent = entity.GetIfHas<FourWayMovementComponent>();
                
            movementComponent.OnUpdate(elapsed);

            if (movementComponent.IntendedMoveDeltaX != 0 || movementComponent.IntendedMoveDeltaY != 0)
            {
                // If the entity has a collision component, we have to apply collision resolution.
                if (entity.GetIfHas<CollisionComponent>() != null)
                {
                    var entityCollision = entity.GetIfHas<CollisionComponent>();
                    // See if the entity collided with any solid tiles.
                    var tileMaps = Scene.LatestInstance.TileMaps;

                    foreach (var tileMap in tileMaps)
                    {
                        int targetTileX = (int)Math.Floor((entity.X + movementComponent.IntendedMoveDeltaX) / tileMap.TileWidth);
                        int targetTileY = (int)(Math.Floor(entity.Y + movementComponent.IntendedMoveDeltaY) / tileMap.TileHeight);
                        var targetTile = tileMap.Get(targetTileX, targetTileY);

                        if (targetTile != null && tileMap.GetDefinition(targetTile).IsSolid)
                        {
                            var collideAgainst = new Entity()
                                .Move(targetTileX * tileMap.TileWidth, targetTileY * tileMap.TileHeight)
                                .Collide(tileMap.TileWidth, tileMap.TileHeight);

                            resolveAabbCollision(entity, collideAgainst, elapsed.TotalSeconds);
                        }
                    }

                    // Compare against collidable entities
                    foreach (var collidable in this.collidables)
                    {
                        if (collidable != entity && collidable.GetIfHas<CollisionComponent>() != null)
                        {
                            var collideAgainstComponent = collidable.GetIfHas<CollisionComponent>();
                            resolveAabbCollision(entity, collidable, elapsed.TotalSeconds);
                        }
                    }
                }

                entity.X += movementComponent.IntendedMoveDeltaX;
                entity.Y += movementComponent.IntendedMoveDeltaY;
                movementComponent.IntendedMoveDeltaX = 0;
                movementComponent.IntendedMoveDeltaY = 0;
            }
        }

        private static void resolveAabbCollision(Entity entity, Entity collideAgainst, double elapsedSeconds)
        {
            ////////////////////// TODO: just nerf IntendedMove, don't actually move.
            
            var movementComponent = entity.GetIfHas<FourWayMovementComponent>();
            var entityCollision = entity.GetIfHas<CollisionComponent>();
            var collideAgainstComponent = collideAgainst.GetIfHas<CollisionComponent>();

            if (isAabbCollision(entity.X + movementComponent.IntendedMoveDeltaX, entity.Y + movementComponent.IntendedMoveDeltaY, entityCollision.Width, entityCollision.Height,
                collideAgainst.X, collideAgainst.Y, collideAgainstComponent.Width, collideAgainstComponent.Height))
            {
                // Another entity occupies that space. Use separating axis theorem (SAT)
                // to see how much we can move, and then move accordingly, resolving at whichever
                // axis collides first by time (not whichever one is the smallest diff).
                (float xDistance, float yDistance) = CalculateAabbDistanceTo(entity, collideAgainst);
                float xVelocity = (float)(movementComponent.IntendedMoveDeltaX / elapsedSeconds);
                float yVelocity = (float)(movementComponent.IntendedMoveDeltaY / elapsedSeconds);
                float xAxisTimeToCollide = xVelocity != 0 ? Math.Abs(xDistance / xVelocity) : 0;
                float yAxisTimeToCollide = yVelocity != 0 ? Math.Abs(yDistance / yVelocity) : 0;

                float shortestTime = 0;

                if (xVelocity != 0 && yVelocity == 0)
                {
                    // Colliison on X-axis only
                    shortestTime = xAxisTimeToCollide;
                    entity.X += shortestTime * xVelocity;
                    movementComponent.IntendedMoveDeltaX = 0;
                }
                else if (xVelocity == 0 && yVelocity != 0)
                {
                    // Collision on Y-axis only
                    shortestTime = yAxisTimeToCollide;
                    entity.Y += shortestTime * yVelocity;
                    movementComponent.IntendedMoveDeltaY = 0;
                }
                else
                {
                    // Collision on X and Y axis (eg. slide up against a wall)
                    shortestTime = Math.Min(Math.Abs(xAxisTimeToCollide), Math.Abs(yAxisTimeToCollide));
                    entity.X += shortestTime * xVelocity;
                    entity.Y += shortestTime * yVelocity;

                    if (movementComponent.SlideOnCollide)
                    {
                        // Resolved collision on the X-axis first
                        if (shortestTime == xAxisTimeToCollide)
                        {
                            // Slide vertically
                            entity.Y  += movementComponent.IntendedMoveDeltaY;
                        }
                        // Resolved collision on the Y-axis first
                        else if (shortestTime == yAxisTimeToCollide)
                        {
                            // Slide horizontally
                            entity.X += movementComponent.IntendedMoveDeltaX;
                        }
                    }

                    movementComponent.IntendedMoveDeltaX = 0;
                    movementComponent.IntendedMoveDeltaY = 0;
                }
            }
        }

        // Assuming we have two AABBs, what's the actual distance between them?
        // eg. if `e1` is on the left of `e2`, we want `dx` to be `e2.left - e1.right`.
        private static (float, float) CalculateAabbDistanceTo(Entity e1, Entity e2)
        {
            var movingCollision = e1.GetIfHas<CollisionComponent>();
            var targetCollision = e2.GetIfHas<CollisionComponent>();

            float dx = 0;
            float dy = 0;

            if (e1.X < e2.X)
            {
                dx = e2.X - (e1.X + movingCollision.Width);
            }
            else if (e1.X > e2.X)
            {
                dx = e1.X - (e2.X + targetCollision.Width);
            }
            
            if (e1.Y < e2.Y)
            {
                dy = e2.Y - (e1.Y + movingCollision.Height);
            }
            else if (e1.Y > e2.Y)
            {
                dy = e1.Y - (e2.Y + targetCollision.Height);
            }
            
            return (dx, dy);
        }

        private static bool isAabbCollision(float x1, float y1, int w1, int h1, float x2, float y2, int w2, int h2)
        {
            // Adapted from https://tutorialedge.net/gamedev/aabb-collision-detection-tutorial/
            return x1 < x2 + w2 &&
                x1 + w1 > x2 &&
                y1 < y2 + h2 &&
                y1 + h1 > y2;
        }
    }
}