using Microsoft.Xna.Framework;
using AiGame1.Graphics;

namespace AiGame1.Entities
{
    public abstract class GameEntity
    {
        public Vector2 WorldPosition { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle BoundingBox { get; protected set; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteRenderer renderer);

        // Helper to update the bounding box based on position
        protected void UpdateBoundingBox(int width, int height)
        {
            BoundingBox = new Rectangle(
                (int)(WorldPosition.X - width / 2),
                (int)(WorldPosition.Y - height / 2),
                width,
                height
            );
        }
    }
}
