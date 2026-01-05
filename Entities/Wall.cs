using Microsoft.Xna.Framework;
using System.Collections.Generic;
using AiGame1.World;
using AiGame1.Graphics;
using AiGame1.Core;

namespace AiGame1.Entities
{
    public class Wall : GameEntity
    {
        private readonly Grid _grid;
        //private readonly Camera _camera;

        public Wall(Grid grid, Vector2 gridPos, int CollisionRadius, int width, int height, bool IsIsometric)
        {
            _grid = grid;
            if (IsIsometric)
            {
                WorldPosition = Camera.IsometricProjection((int)gridPos.X, (int)gridPos.Y, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
            }
            else
            {
                WorldPosition = Camera.TopDownProjection((int)gridPos.X, (int)gridPos.Y, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
            }
            ColliderRadius = CollisionRadius;
            UpdateBoundingBox(width, height);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteRenderer renderer)
        {
            
        }
    }


}