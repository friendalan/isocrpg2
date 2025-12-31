using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
                CheckAndResolveWallCollisions(entity, gameTime);
                CheckAndResolveEntityCollisions(entity, gameTime);
            }
        }
        
        private void CheckAndResolveWallCollisions(GameEntity entity, GameTime gameTime)
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
                            ResolveCollision(entity, wallBounds, gameTime);
                        }
                    }
                }
            }
        }

        private void CheckAndResolveEntityCollisions(GameEntity entity, GameTime gameTime)
        {
            List<GameEntity> nearby = _spatialHash.GetNearby(entity);
            foreach (var other in nearby)
            {
                if (entity == other) continue; // Don't check against self

                if (entity.BoundingBox.Intersects(other.BoundingBox))
                {
                    // For now, entity-entity collision is simpler: just stop
                    // A full sliding resolution is more complex.
                    ResolveCollision(entity, other.BoundingBox, gameTime);
                }
            }
        }
        
        private void ResolveCollision(GameEntity entity, Rectangle obstacleBounds, GameTime gameTime)
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

            // Re-apply the new velocity for the remainder of the frame to complete the slide
            // This is a simplification; a more robust solution would integrate time remaining.
            entity.WorldPosition += entity.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
    }
}
