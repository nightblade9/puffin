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
                if (entity.GetIfHas<CollisionComponent>() != null)
                {
                    var myCollision = entity.GetIfHas<CollisionComponent>();
                    // Compare against tilemaps
                    var tileMaps = Scene.LatestInstance.TileMaps;

                    foreach (var tileMap in tileMaps)
                    {
                        var targetX = (int)((entity.X + movementComponent.IntendedMoveDeltaX) /  tileMap.TileWidth);
                        var targetY = (int)((entity.Y + movementComponent.IntendedMoveDeltaY) /  tileMap.TileHeight);
                        var tile = tileMap.Get(targetX, targetY);
                        if (tile != null)
                        {
                            var definition = tileMap.GetDefinition(tile);
                            if (definition.IsSolid && isAabbCollision(
                                entity.X, entity.Y, myCollision.Width, myCollision.Height,
                                targetX * tileMap.TileWidth, targetY * tileMap.TileHeight, tileMap.TileWidth, tileMap.TileHeight))
                            {
                                // Can't move there according to one tilemap.
                                // TODO: detect the collision and allow sliding instead of nixing out both movements.
                                movementComponent.IntendedMoveDeltaX = 0;
                                movementComponent.IntendedMoveDeltaY = 0;
                                return;
                            }
                        }
                    }

                    // Compare against collidable entities
                    foreach (var collidable in this.collidables)
                    {
                        if (collidable != entity && collidable.GetIfHas<CollisionComponent>() != null)
                        {
                            var c2 = collidable.GetIfHas<CollisionComponent>();
                            if (isAabbCollision(entity.X + movementComponent.IntendedMoveDeltaX, entity.Y + movementComponent.IntendedMoveDeltaY, myCollision.Width, myCollision.Height,
                                collidable.X, collidable.Y, c2.Width, c2.Height))
                            {
                                // Another entity occupies that space. Use separating axis theorem (SAT)
                                // to see how much we can move, and then move accordingly, resolving at whichever
                                // axis collides first (not whichever one is the smallest diff).
                                (float dx, float dy) = CalculateDxDy(entity, collidable);
                                float vx = (float)(movementComponent.IntendedMoveDeltaX / elapsed.TotalSeconds);
                                float vy = (float)(movementComponent.IntendedMoveDeltaY / elapsed.TotalSeconds);
                                float tx = vx != 0 ? Math.Abs(dx / vx) : 0;
                                float ty = vy != 0 ? Math.Abs(dy / vy) : 0;

                                float time;
                                if (vx != 0 && vy == 0)
                                {
                                    // X-axis collision
                                    time = tx;
                                    entity.X += time * vx;
                                    movementComponent.IntendedMoveDeltaX = 0;
                                }
                                else if (vx == 0 && vy != 0)
                                {
                                    time = ty;
                                    entity.Y += time * vy;
                                    movementComponent.IntendedMoveDeltaY = 0;
                                }
                                else
                                {
                                    time = Math.Min(Math.Abs(tx), Math.Abs(ty));
                                    entity.X += time * vx;
                                    entity.Y += time * vy;

                                    movementComponent.IntendedMoveDeltaX = 0;
                                    movementComponent.IntendedMoveDeltaY = 0;
                                }

                                Console.WriteLine($"elapsed={elapsed.TotalSeconds}; dx={dx} dy={dy} vx={vx} vy={vy} tx={tx} ty={ty}; time is {time}; resolve along {(time == tx ? "x" : "y")}-axis");

                                return;
                            }
                        }
                    }
                }

                entity.X += movementComponent.IntendedMoveDeltaX;
                entity.Y += movementComponent.IntendedMoveDeltaY;
                // BUG: everything was clear but we moved to far and now overlap something.

                movementComponent.IntendedMoveDeltaX = 0;
                movementComponent.IntendedMoveDeltaY = 0;
            }
        }

        private static (float, float) CalculateDxDy(Entity moving, Entity target)
        {
            var movingCollision = moving.GetIfHas<CollisionComponent>();
            var targetCollision = target.GetIfHas<CollisionComponent>();

            float dx = 0;
            float dy = 0;

            if (moving.X < target.X)
            {
                dx = target.X - (moving.X + movingCollision.Width);
            }
            else if (moving.X > target.X)
            {
                dx = moving.X - (target.X + targetCollision.Width);
            }

            
            if (moving.Y < target.Y)
            {
                dy = target.Y - (moving.Y + movingCollision.Height);
            }
            else if (moving.Y > target.Y)
            {
                dy = moving.Y - (target.Y + targetCollision.Height);
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