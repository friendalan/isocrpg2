using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AiGame1.World; // To access Grid and TileType
using AiGame1.Core; // To access Camera.IsometricProjection

namespace AiGame1.Graphics
{
    public class TilemapRenderer
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly Grid _grid;
        private readonly Texture2D _floorTexture;
        private readonly Texture2D _wallTexture;

        private readonly Texture2D _floor2D;
        private readonly Texture2D _wall2D;

        // Assuming isometric tiles are typically diamond-shaped, e.g., 64x32
        public static readonly int TileWidth = 64; 
        public static readonly int TileHeight = 32;

        public TilemapRenderer(SpriteBatch spriteBatch, Grid grid, Texture2D floorTexture, Texture2D wallTexture
                                , Texture2D floor2D, Texture2D wall2D)
        {
            _spriteBatch = spriteBatch;
            _grid = grid;
            _floorTexture = floorTexture;
            _wallTexture = wallTexture;
            _floor2D = floor2D;
            _wall2D = wall2D;
        }

        public void Draw(Camera camera, bool isIsometricView)
        {
            if (isIsometricView)
            {
                DrawIsometric(camera);
            }
            else
            {
                DrawTopDown(camera);
            }
        }

        public void DrawIsometric(Camera camera)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    Cell cell = _grid.Cells[x, y];
                    Texture2D textureToDraw = null;

                    if (cell.Type == TileType.Floor)
                    {
                        textureToDraw = _floorTexture;
                    }
                    else if (cell.Type == TileType.Wall)
                    {
                        textureToDraw = _wallTexture;
                    }

                    if (textureToDraw != null)
                    {
                        // Calculate world position for the tile based on grid coordinates
                        Vector2 worldPosition = Camera.IsometricProjection(x, y, TileWidth, TileHeight);
                        
                        // Adjust world position for drawing texture:
                        // The IsometricProjection returns the center of the tile's footprint.
                        // To draw the texture correctly, we need to offset it by half its width and height
                        // to position its top-left corner for drawing.
                        Vector2 drawPosition = worldPosition - new Vector2(textureToDraw.Width / 2, textureToDraw.Height / 2);

                        _spriteBatch.Draw(textureToDraw, drawPosition, Color.White);
                    }
                }
            }
        }


        public void DrawTopDown(Camera camera)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    Cell cell = _grid.Cells[x, y];
                    Texture2D textureToDraw = null;

                    if (cell.Type == TileType.Floor)
                    {
                        textureToDraw = _floor2D;
                    }
                    else if (cell.Type == TileType.Wall)
                    {
                        textureToDraw = _wall2D;
                    }

                    if (textureToDraw != null)
                    {
                        // Calculate world position for the tile based on grid coordinates
                        Vector2 worldPosition = Camera.TopDownProjection(x, y, TileWidth, TileHeight);
                        
                        // Adjust world position for drawing texture:
                        // The IsometricProjection returns the center of the tile's footprint.
                        // To draw the texture correctly, we need to offset it by half its width and height
                        // to position its top-left corner for drawing.
                        //Vector2 drawPosition = worldPosition - new Vector2(textureToDraw.Width / 2, textureToDraw.Height / 2);

                        _spriteBatch.Draw(textureToDraw, worldPosition, Color.White);
                    }
                }
            }
        }

    }
}
