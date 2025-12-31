using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using AiGame1.World;
using AiGame1.Graphics;
using AiGame1.Core;
using AiGame1.AI;

namespace AiGame1.Entities
{
    public class Player : GameEntity
    {
        private readonly Grid _grid;
        private readonly Camera _camera;
        private List<Vector2> _path;
        private MouseState _previousMouseState;

        private const float Speed = 100f; // Movement speed in pixels per second

        public Player(Grid grid, Camera camera, Vector2 startGridPos)
        {
            _grid = grid;
            _camera = camera;
            WorldPosition = Camera.IsometricProjection((int)startGridPos.X, (int)startGridPos.Y, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
            _path = new List<Vector2>();
            _previousMouseState = Mouse.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();
            Move(gameTime);
            UpdateBoundingBox(20, 20); // Corrected size to match visual diameter
        }

        private void HandleInput()
        {
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                // Convert mouse screen position to world position
                Vector2 mouseWorldPos = _camera.ScreenToWorld(currentMouseState.Position.ToVector2());

                // Convert world position to grid position
                Vector2 targetGridPos = Camera.WorldToGrid(mouseWorldPos, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
                Vector2 startGridPos = Camera.WorldToGrid(WorldPosition, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);

                // Find a path to the target
                if (targetGridPos.X >= 0 && targetGridPos.X < _grid.Width && targetGridPos.Y >= 0 && targetGridPos.Y < _grid.Height)
                {
                    List<Vector2> newPath = AStarPathfinder.FindPath(_grid, startGridPos, targetGridPos);
                    if (newPath != null)
                    {
                        _path = newPath;
                        if (_path.Count > 0)
                        {
                             // Remove the first node, which is the start position
                            _path.RemoveAt(0);
                        }
                    }
                }
            }
            _previousMouseState = currentMouseState;
        }

        private void Move(GameTime gameTime)
        {
            if (_path == null || _path.Count == 0)
            {
                // No active path, let existing velocity (e.g., from sliding) decay.
                Velocity *= 0.9f; // Apply friction
                if (Velocity.Length() < 1.0f)
                {
                    Velocity = Vector2.Zero;
                }
            }
            else
            {
                // Follow the path
                Vector2 targetGridPos = _path[0];
                Vector2 targetWorldPos = Camera.IsometricProjection((int)targetGridPos.X, (int)targetGridPos.Y, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);

                Vector2 direction = targetWorldPos - WorldPosition;
                if (direction.Length() > 1.0f)
                {
                    direction.Normalize();
                    Velocity = direction * Speed;
                }
                
                if (Vector2.Distance(WorldPosition, targetWorldPos) < 2.0f)
                {
                    WorldPosition = targetWorldPos; // Snap to target
                    _path.RemoveAt(0);
                    if (_path.Count == 0) Velocity = Vector2.Zero; // Stop when path is complete
                }
            }
            
            WorldPosition += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(SpriteRenderer renderer)
        {
            renderer.DrawCircle(WorldPosition, 10, Color.Green);
        }
    }
}
