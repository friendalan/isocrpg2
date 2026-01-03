using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using AiGame1.Entities;
using AiGame1.World;
using AiGame1.Graphics; // For TilemapRenderer constants

namespace AiGame1.Collision
{
    public class CollisionManager
    {
        private readonly SpatialHash _spatialHash;
        private readonly Grid _grid;

        public CollisionManager(Grid grid, int spatialHashCellSize)
        {
            _grid = grid;
            _spatialHash = new SpatialHash(spatialHashCellSize);
        }

        public void DetectAndResolveCollisions(List<GameEntity> entities, GameTime gameTime)
        {
            // Update spatial hash with new entity positions
            _spatialHash.Clear();
            foreach (var entity in entities)
            {
                _spatialHash.Add(entity);
            }

            // Check and resolve collisions for each entity
            foreach (var entity in entities)
            {
                //CheckAndResolveWallCollisions(entity);
                //CheckAndResolveEntityCollisions(entity);

                CheckCircleCircleCollision(entity);
                CheckCircleWallCollision(entity);
            }
        }

        private void CheckCircleCircleCollision(GameEntity entity) {
            List<GameEntity> nearby = _spatialHash.GetNearby(entity);
            foreach (var other in nearby)
            {
                if (entity == other) continue; // Don't check against self
 
                var dx = entity.WorldPosition.X - other.WorldPosition.X;
                var dy = entity.WorldPosition.Y - other.WorldPosition.Y;
                var dist = System.Math.Sqrt(dx*dx + dy*dy);
                if (dist < entity.ColliderRadius+other.ColliderRadius) 
                {
                    var overlap = (entity.ColliderRadius+other.ColliderRadius) - dist;
                    var pushX = (dx / dist) * overlap / 2;
                    var pushY = (dy / dist) * overlap / 2;

                    entity.WorldPosition = new Vector2((float)(entity.WorldPosition.X + pushX), (float)(entity.WorldPosition.Y + pushY));
                    other.WorldPosition = new Vector2((float)(other.WorldPosition.X - pushX), (float)(other.WorldPosition.Y - pushY));
                }
            }
        }

        private bool CirclesTouching(GameEntity entity, GameEntity other)
        {
            var dx = entity.WorldPosition.X - other.WorldPosition.X;
            var dy = entity.WorldPosition.Y - other.WorldPosition.Y;
            var dist = System.Math.Sqrt(dx*dx + dy*dy) -1; //minus 1 to ensure small overlap
            return dist < entity.ColliderRadius+other.ColliderRadius; //+1
        }

        private void CheckCircleWallCollision(GameEntity entity) {
            Rectangle entityBounds = entity.BoundingBox;

            // Get the grid coordinates the entity might be touching
            Vector2 minGridPos = Core.Camera.WorldToGrid(new Vector2(entityBounds.Left, entityBounds.Top), TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
            Vector2 maxGridPos = Core.Camera.WorldToGrid(new Vector2(entityBounds.Right, entityBounds.Bottom), TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);

            for (int x = (int)minGridPos.X - 1; x <= (int)maxGridPos.X + 1; x++)
            {
                for (int y = (int)minGridPos.Y - 1; y <= (int)maxGridPos.Y + 1; y++)
                {
                    if (x >= 0 && x < _grid.Width && y >= 0 && y < _grid.Height && _grid.Cells[x, y].Type == TileType.Wall)
                    {
                        Vector2 wallWorldPos = Core.Camera.IsometricProjection(x, y, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
                        Rectangle wallBounds = new Rectangle((int)(wallWorldPos.X - TilemapRenderer.TileWidth / 2), (int)(wallWorldPos.Y - TilemapRenderer.TileHeight / 2), TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);

                        if (entityBounds.Intersects(wallBounds))
                        {
                            // Find the closest point on the rectangle to the circle
                            var closestX = Math.Max(wallWorldPos.X-TilemapRenderer.TileWidth/2, Math.Min(entity.WorldPosition.X, wallWorldPos.X+TilemapRenderer.TileWidth/2));
                            var closestY = Math.Max(wallWorldPos.Y-TilemapRenderer.TileHeight/2, Math.Min(entity.WorldPosition.Y, wallWorldPos.Y+TilemapRenderer.TileHeight/2));

                            // Calculate the distance between the circle center and the closest point
                            var dx = entity.WorldPosition.X - closestX;
                            var dy = entity.WorldPosition.Y - closestY;
                            var distance = Math.Sqrt(dx*dx + dy*dy);

                            // If the distance is less than the circle radius then collision
                            if (distance < entity.ColliderRadius) {
                                // Resolve collision by pushing the circle away
                                if (distance > 0) {
                                    var pushX = (dx / distance) * (entity.ColliderRadius - distance);
                                    var pushY = (dy / distance) * (entity.ColliderRadius - distance);

                                    entity.WorldPosition = new Vector2((float)(entity.WorldPosition.X + pushX), (float)(entity.WorldPosition.Y + pushY));

                                }
                            }
                        }
                    }
                }
            }
        }

 //====== Obsolete =======================================

        private void CheckAndResolveWallCollisions(GameEntity entity)
        {
            Rectangle entityBounds = entity.BoundingBox;

            // Get the grid coordinates the entity might be touching
            Vector2 minGridPos = Core.Camera.WorldToGrid(new Vector2(entityBounds.Left, entityBounds.Top), TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
            Vector2 maxGridPos = Core.Camera.WorldToGrid(new Vector2(entityBounds.Right, entityBounds.Bottom), TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);

            for (int x = (int)minGridPos.X - 1; x <= (int)maxGridPos.X + 1; x++)
            {
                for (int y = (int)minGridPos.Y - 1; y <= (int)maxGridPos.Y + 1; y++)
                {
                    if (x >= 0 && x < _grid.Width && y >= 0 && y < _grid.Height && _grid.Cells[x, y].Type == TileType.Wall)
                    {
                        Vector2 wallWorldPos = Core.Camera.IsometricProjection(x, y, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
                        Rectangle wallBounds = new Rectangle((int)(wallWorldPos.X - TilemapRenderer.TileWidth / 2), (int)(wallWorldPos.Y - TilemapRenderer.TileHeight / 2), TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);

                        if (entityBounds.Intersects(wallBounds))
                        {
                            ResolveCollision(entity, wallBounds);
                        }
                    }
                }
            }
        }

        private void CheckAndResolveEntityCollisions(GameEntity entity)
        {
            List<GameEntity> nearby = _spatialHash.GetNearby(entity);
            foreach (var other in nearby)
            {
                if (entity == other) continue; // Don't check against self

                if (entity.BoundingBox.Intersects(other.BoundingBox))
                {
                    ResolveCollision(entity, other.BoundingBox);
                }
            }
        }
        
        private void ResolveCollision(GameEntity entity, Rectangle obstacleBounds)
        {
            Rectangle entityBounds = entity.BoundingBox;
            Vector2 penetration = CalculatePenetration(entityBounds, obstacleBounds);

            // Move the entity out of the collision
            entity.WorldPosition -= penetration;

            // --- Sliding Logic ---
            if (entity.Velocity == Vector2.Zero) return;
            
            // Calculate collision normal
            Vector2 normal = -penetration;
            if (normal != Vector2.Zero)
            {
                normal.Normalize();
            }

            // Project velocity onto the normal to get the perpendicular component
            Vector2 perpendicularVelocity = Vector2.Dot(entity.Velocity, normal) * normal;

            // Subtract the perpendicular component from the original velocity to get the sliding component
            Vector2 slidingVelocity = entity.Velocity - perpendicularVelocity;

            entity.Velocity = slidingVelocity;
        }

        private Vector2 CalculatePenetration(Rectangle rectA, Rectangle rectB)
        {
            Rectangle intersection = Rectangle.Intersect(rectA, rectB);
            Vector2 penetration;

            if (intersection.Width < intersection.Height)
            {
                float dx = rectA.Center.X < rectB.Center.X ? -intersection.Width : intersection.Width;
                penetration = new Vector2(dx, 0);
            }
            else
            {
                float dy = rectA.Center.Y < rectB.Center.Y ? -intersection.Height : intersection.Height;
                penetration = new Vector2(0, dy);
            }

            return penetration;
        }


        
/*
        private void ResolveCircleCircleCollision(GameEntity entity, GameEntity other) {
            entityPositionData := g.EcsManager.GetEntityPositionComponent(entity.Id)
            otherPositionData := g.EcsManager.GetEntityPositionComponent(other.Id)
            entityColliderData := g.EcsManager.GetEntityColliderComponent(entity.Id)
            otherColliderData := g.EcsManager.GetEntityColliderComponent(other.Id)
            dx := entityPositionData.Position.X - otherPositionData.Position.X
            dy := entityPositionData.Position.Y - otherPositionData.Position.Y
            dist := math.Sqrt(dx*dx + dy*dy)
            if dist < entityColliderData.Radius+otherColliderData.Radius {
                overlap := (entityColliderData.Radius + otherColliderData.Radius) - dist
                pushX := (dx / dist) * overlap / 2
                pushY := (dy / dist) * overlap / 2
                entityPositionData.Position.X += pushX
                entityPositionData.Position.Y += pushY
                otherPositionData.Position.X -= pushX
                otherPositionData.Position.Y -= pushY
            }
        }
*/
    }
}
