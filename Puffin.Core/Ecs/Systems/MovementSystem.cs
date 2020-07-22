using System;
using System.Collections.Generic;
using Puffin.Core.Ecs.Components;

namespace Puffin.Core.Ecs.Systems
{
    class MovementSystem : ISystem
    {
        private readonly IList<Entity> entities = new List<Entity>();
        private readonly IList<Entity> collidables = new List<Entity>();
        private readonly Scene scene;

        public MovementSystem(Scene scene)
        {
            this.scene = scene;
        }

        public void OnAddEntity(Entity entity)
        {
            // This assumes all entities *want* to move. In the future, if this becomes a bottleneck, 
            // we can separate out or somehow "tag" moving entities, so that only they are processed.
            this.entities.Add(entity);

            if (entity.Get<CollisionComponent>() != null)
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
            // Separate out updating keyboard (intention to move) with collision resolution).
            // For cases where movement from one tile collides/resolves into another tile (eg.
            // standing in the top-left wall corner of the map, pressing up and left together,
            // top-tile collides/resolves into the left tile). Splitting this into two rounds
            // of resolution fixes this.
            //
            // We also want to just update the intention to move once, because the first round
            // of resolution modifies it to non-collide.
            var halfElapsed = TimeSpan.FromSeconds(elapsed.TotalSeconds / 2);
            foreach (var entity in this.entities)
            {
                // Get keyboard/intended movement
                entity.Get<FourWayMovementComponent>()?.OnUpdate();

                if (entity.VelocityX != 0)
                {
                    entity.IntendedMoveDeltaX += (float)(entity.VelocityX * elapsed.TotalSeconds);
                }
                if (entity.VelocityY != 0)
                {
                    entity.IntendedMoveDeltaY += (float)(entity.VelocityY * elapsed.TotalSeconds);
                }

                // Resolve collisions twice to stabilize multi-collisions.
                this.ProcessMovement(halfElapsed, entity);
                this.ProcessMovement(halfElapsed, entity);
            }

            foreach (var entity in this.entities)
            {
                entity.X += entity.IntendedMoveDeltaX;
                entity.Y += entity.IntendedMoveDeltaY;
                entity.IntendedMoveDeltaX = 0;
                entity.IntendedMoveDeltaY = 0;
            }
        }

        private void ProcessMovement(TimeSpan elapsed, Entity entity)
        {
            if (entity.IntendedMoveDeltaX != 0 || entity.IntendedMoveDeltaY != 0)
            {
                // If the entity has a collision component, we have to apply collision resolution.
                if (entity.Get<CollisionComponent>() != null)
                {
                    var entityCollision = entity.Get<CollisionComponent>();
                    // See if the entity collided with any solid tiles.
                    var tileMaps = this.scene.TileMaps;

                    foreach (var tileMap in tileMaps)
                    {
                        // Check the tile at the entity's position; if we're moving right/down, use the right/down edge of the entity instead
                        // of the left/up edge of the entity.

                        int targetTileX = (int)Math.Floor(
                            (entity.X + (entity.IntendedMoveDeltaX > 0 ? entityCollision.Width : 0) + entity.IntendedMoveDeltaX) / tileMap.TileWidth);

                        int targetTileY = (int)(Math.Floor(
                            entity.Y + (entity.IntendedMoveDeltaY > 0 ? entityCollision.Height : 0) + entity.IntendedMoveDeltaY) / tileMap.TileHeight);

                        var targetTile = tileMap.Get(targetTileX, targetTileY);

                        if (targetTile != null && tileMap.GetDefinition(targetTile).IsSolid)
                        {
                            var collideAgainst = new Entity()
                                .Move(targetTileX * tileMap.TileWidth, targetTileY * tileMap.TileHeight)
                                .Collide(tileMap.TileWidth, tileMap.TileHeight);
                            
                            resolveAabbCollision(entity, collideAgainst);
                        }
                    }

                    // Compare against collidable entities
                    foreach (var collidable in this.collidables)
                    {
                        if (collidable != entity && collidable.Get<CollisionComponent>() != null)
                        {
                            var collideAgainstComponent = collidable.Get<CollisionComponent>();
                            resolveAabbCollision(entity, collidable);
                        }
                    }
                }
            }
        }

        // Checks for AABB collisions between entity (moving) and collideAgainst (hopefully not moving).
        // The output is to modify the IntendedMoveX/IntendedMoveY on entity so that it will be just at the point
        // of collision (stop right at the collision).
        private static void resolveAabbCollision(Entity entity, Entity collideAgainst)
        {
            (var oldIntendedX, var oldIntendedY) = (entity.IntendedMoveDeltaX, entity.IntendedMoveDeltaY);

            var entityCollision = entity.Get<CollisionComponent>();
            var collideAgainstComponent = collideAgainst.Get<CollisionComponent>();

            if (isAabbCollision(entity.X + entity.IntendedMoveDeltaX + entityCollision.XOffset, entity.Y + entityCollision.YOffset + entity.IntendedMoveDeltaY, entityCollision.Width, entityCollision.Height,
                collideAgainst.X + collideAgainstComponent.XOffset, collideAgainst.Y + collideAgainstComponent.YOffset, collideAgainstComponent.Width, collideAgainstComponent.Height))
            {
                // Another entity occupies that space. Use separating axis theorem (SAT)
                // to see how much we can move, and then move accordingly, resolving at whichever
                // axis collides first by time (not whichever one is the smallest diff).
                (float xDistance, float yDistance) = CalculateAabbDistanceTo(entity, collideAgainst);
                (float xVelocity, float yVelocity) = (entity.VelocityX, entity.VelocityY);
                float xAxisTimeToCollide = xVelocity != 0 ? Math.Abs(xDistance / xVelocity) : 0;
                float yAxisTimeToCollide = yVelocity != 0 ? Math.Abs(yDistance / yVelocity) : 0;

                float shortestTime = 0;
                string collisionAxis = "";

                if (xVelocity != 0 && yVelocity == 0)
                {
                    // Colliison on X-axis only
                    shortestTime = xAxisTimeToCollide;
                    entity.IntendedMoveDeltaX = shortestTime * xVelocity;
                    collisionAxis = "X";
                }
                else if (xVelocity == 0 && yVelocity != 0)
                {
                    // Collision on Y-axis only
                    shortestTime = yAxisTimeToCollide;
                    entity.IntendedMoveDeltaY = shortestTime * yVelocity;
                    collisionAxis = "Y";
                }
                else
                {
                    // Collision on X and Y axis (eg. slide up against a wall)
                    shortestTime = Math.Min(Math.Abs(xAxisTimeToCollide), Math.Abs(yAxisTimeToCollide));
                    entity.IntendedMoveDeltaX = shortestTime * xVelocity;
                    entity.IntendedMoveDeltaY = shortestTime * yVelocity;
                    collisionAxis = shortestTime == Math.Abs(xAxisTimeToCollide) ? "X" : "Y";

                    if (entityCollision.SlideOnCollide)
                    {
                        // Setting oldIntendedX/oldIntendedY might put us directly inside another solid thing.
                        // No worries, we resolve collisions twice, so the second iteration will catch it.

                        // Resolved collision on the X-axis first
                        if (shortestTime == xAxisTimeToCollide)
                        {
                            // Slide vertically
                            entity.IntendedMoveDeltaX = 0;
                            // If we're in a corner, don't resolve incorrectly; move only if we're clear on the Y-axis.
                            // Fixes a bug where you  move a lot in the corner (left/right/left/right) and suddenly go through the wall.                     
                            if (!isAabbCollision(entity.X + entityCollision.XOffset, entity.Y + entityCollision.YOffset + oldIntendedY, entityCollision.Width, entityCollision.Height,
                                collideAgainst.X + collideAgainstComponent.XOffset, collideAgainst.Y + collideAgainstComponent.YOffset, collideAgainstComponent.Width, collideAgainstComponent.Height))
                                {
                                    entity.IntendedMoveDeltaY = oldIntendedY;
                                }
                        }
                        // Resolved collision on the Y-axis first
                        if (shortestTime == yAxisTimeToCollide)
                        {
                            // Slide horizontally
                            entity.IntendedMoveDeltaY = 0;
                            // If we're in a corner, don't resolve incorrectly; move only if we're clear on the X-axis.
                            // Fixes a bug where you  move a lot in the corner (left/right/left/right) and suddenly go through the wall.
                            if (!isAabbCollision(entity.X  + entityCollision.XOffset + oldIntendedX, entity.Y + entityCollision.YOffset, entityCollision.Width, entityCollision.Height,
                                collideAgainst.X + collideAgainstComponent.XOffset, collideAgainst.Y + collideAgainstComponent.YOffset, collideAgainstComponent.Width, collideAgainstComponent.Height))
                                {
                                    entity.IntendedMoveDeltaX = oldIntendedX;
                                }
                        }
                    }
                }

                // Post-collision callbacks
                entityCollision.onCollide?.Invoke(collideAgainst, collisionAxis);
                collideAgainstComponent.onCollide?.Invoke(entity, collisionAxis);
            }
        }

        // Assuming we have two AABBs, what's the actual distance between them?
        // eg. if `e1` is on the left of `e2`, we want `dx` to be `e2.left - e1.right`.
        private static (float, float) CalculateAabbDistanceTo(Entity e1, Entity e2)
        {
            var movingCollision = e1.Get<CollisionComponent>();
            var targetCollision = e2.Get<CollisionComponent>();

            float dx = 0;
            float dy = 0;

            if (e1.X + movingCollision.XOffset < e2.X  + targetCollision.XOffset)
            {
                dx = e2.X + targetCollision.XOffset - (e1.X + movingCollision.XOffset + movingCollision.Width);
            }
            else if (e1.X + movingCollision.XOffset > e2.X + targetCollision.XOffset)
            {
                dx = e1.X + movingCollision.XOffset - (e2.X + targetCollision.XOffset + targetCollision.Width);
            }
            
            if (e1.Y + movingCollision.YOffset < e2.Y + targetCollision.YOffset)
            {
                dy = e2.Y + targetCollision.YOffset - (e1.Y + movingCollision.YOffset + movingCollision.Height);
            }
            else if (e1.Y + movingCollision.YOffset > e2.Y + targetCollision.YOffset)
            {
                dy = e1.Y + movingCollision.YOffset - (e2.Y + targetCollision.YOffset + targetCollision.Height);
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