using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AiGame1.Core
{
    public class Camera
    {
        public Vector2 Position { get; set; }
        public float Zoom { get; set; }
        public Matrix TransformMatrix { get; private set; }

        private readonly int _viewportWidth;
        private readonly int _viewportHeight;

        public Camera(int viewportWidth, int viewportHeight)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            Position = Vector2.Zero; // Camera starts at (0,0) world coordinates
            Zoom = 1.0f; // No zoom initially
            UpdateTransformMatrix();
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            float speed = 200f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.Left))
                Position -= new Vector2(speed, 0);
            if (keyboardState.IsKeyDown(Keys.Right))
                Position += new Vector2(speed, 0);
            if (keyboardState.IsKeyDown(Keys.Up))
                Position -= new Vector2(0, speed);
            if (keyboardState.IsKeyDown(Keys.Down))
                Position += new Vector2(0, speed);

            UpdateTransformMatrix();
        }

        private void UpdateTransformMatrix()
        {
            TransformMatrix =
                Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(_viewportWidth * 0.5f, _viewportHeight * 0.5f, 0)); // Center the view
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TransformMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TransformMatrix));
        }

        // Helper for isometric projection (grid coordinates to world coordinates)
        public static Vector2 IsometricProjection(int gridX, int gridY, int tileWidth, int tileHeight)
        {
            float worldX = (gridX - gridY) * (tileWidth / 2f);
            float worldY = (gridX + gridY) * (tileHeight / 2f);
            return new Vector2(worldX, worldY);
        }

        // Helper for converting world coordinates back to grid coordinates
        public static Vector2 WorldToGrid(Vector2 worldPosition, int tileWidth, int tileHeight)
        {
            float tileWidthHalf = tileWidth / 2f;
            float tileHeightHalf = tileHeight / 2f;

            float gridX = (worldPosition.X / tileWidthHalf + worldPosition.Y / tileHeightHalf) / 2;
            float gridY = (worldPosition.Y / tileHeightHalf - worldPosition.X / tileWidthHalf) / 2;

            return new Vector2((int)System.Math.Round(gridX), (int)System.Math.Round(gridY));
        }
    }
}
