using Microsoft.Xna.Framework;
using AiGame1.Graphics;

namespace AiGame1.Entities
{
    public class Enemy : GameEntity
    {
        public Enemy(Vector2 startGridPos)
        {
            // Set initial world position based on grid position
            WorldPosition = Core.Camera.IsometricProjection((int)startGridPos.X, (int)startGridPos.Y, Graphics.TilemapRenderer.TileWidth, Graphics.TilemapRenderer.TileHeight);
        }

        public override void Update(GameTime gameTime)
        {
            // AI and movement logic will be added here in later steps
            UpdateBoundingBox(10, 10); // Example size
        }

        public override void Draw(SpriteRenderer renderer)
        {
            renderer.DrawCircle(WorldPosition, 10, Color.Red);
        }
    }
}
