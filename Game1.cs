using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AiGame1.Core;
using AiGame1.Graphics;
using AiGame1.World;

namespace AiGame1;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Camera _camera;
    private Grid _grid;
    private TilemapRenderer _tilemapRenderer;
    private SpriteRenderer _spriteRenderer;

    private Texture2D _floorTexture;
    private Texture2D _wallTexture;
    private Texture2D _playerTexture; 
    private Texture2D _enemyTexture; 

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        _camera = new Camera(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _grid = new Grid(20, 20); // Example grid size

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _floorTexture = Content.Load<Texture2D>("assets/floor");
        _wallTexture = Content.Load<Texture2D>("assets/wall");
        _playerTexture = Content.Load<Texture2D>("assets/player");
        _enemyTexture = Content.Load<Texture2D>("assets/enemy");

        _tilemapRenderer = new TilemapRenderer(_spriteBatch, _grid, _floorTexture, _wallTexture);
        _spriteRenderer = new SpriteRenderer(_spriteBatch, GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _camera.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(transformMatrix: _camera.TransformMatrix);

        _tilemapRenderer.Draw(_camera);

        // Draw placeholder circles for player and enemy
        // Convert grid coordinates to world coordinates for drawing
        Vector2 playerGridPos = new Vector2(5, 5);
        Vector2 playerWorldPos = Camera.IsometricProjection((int)playerGridPos.X, (int)playerGridPos.Y, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
        _spriteRenderer.DrawCircle(playerWorldPos, 10, Color.Green); // Player in green

        Vector2 enemyGridPos = new Vector2(10, 10);
        Vector2 enemyWorldPos = Camera.IsometricProjection((int)enemyGridPos.X, (int)enemyGridPos.Y, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);
        _spriteRenderer.DrawCircle(enemyWorldPos, 10, Color.Red); // Enemy in red


        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

