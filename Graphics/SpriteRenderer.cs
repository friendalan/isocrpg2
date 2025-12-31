using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AiGame1.Graphics
{
    public class SpriteRenderer
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly GraphicsDevice _graphicsDevice;
        private Texture2D _circleTexture;

        public SpriteRenderer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
            CreateCircleTexture(16); // Create a default circle texture
        }

        private void CreateCircleTexture(int radius)
        {
            _circleTexture = new Texture2D(_graphicsDevice, radius * 2, radius * 2);
            Color[] data = new Color[_circleTexture.Width * _circleTexture.Height];

            for (int x = 0; x < _circleTexture.Width; x++)
            {
                for (int y = 0; y < _circleTexture.Height; y++)
                {
                    Vector2 center = new Vector2(radius, radius);
                    Vector2 current = new Vector2(x, y);
                    if (Vector2.Distance(center, current) < radius)
                    {
                        data[x + y * _circleTexture.Width] = Color.White;
                    }
                    else
                    {
                        data[x + y * _circleTexture.Width] = Color.Transparent;
                    }
                }
            }
            _circleTexture.SetData(data);
        }

        // Draw a circle for entities
        public void DrawCircle(Vector2 position, int radius, Color color)
        {
            // Position for drawing assumes the position is the center of the circle
            // The texture is a square, so origin is center. Scale adjusts to desired radius.
            Vector2 origin = new Vector2(_circleTexture.Width / 2, _circleTexture.Height / 2);
            _spriteBatch.Draw(_circleTexture, position, null, color, 0f, origin, (float)radius / (_circleTexture.Width / 2), SpriteEffects.None, 0f);
        }

        // Overload to draw an actual sprite later
        public void DrawSprite(Texture2D texture, Vector2 position, Color color, float rotation = 0f, Vector2 origin = default(Vector2), float scale = 1f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            _spriteBatch.Draw(texture, position, null, color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
