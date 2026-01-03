using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using AiGame1.Core;
using AiGame1.Graphics;
using AiGame1.World;
using AiGame1.Entities;
using AiGame1.Collision;

namespace AiGame1;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Camera _camera;
    private Grid _grid;
    private TilemapRenderer _tilemapRenderer;
    private SpriteRenderer _spriteRenderer;
    private CollisionManager _collisionManager;

    private Texture2D _floorTexture;
    private Texture2D _wallTexture;
    private Texture2D _playerTexture; 
    private Texture2D _enemyTexture; 

    private Player _player;
    private List<Enemy> _enemies;

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
        _collisionManager = new CollisionManager(_grid, TilemapRenderer.TileWidth);

        _player = new Player(_grid, _camera, new Vector2(5, 5), 10);
        _enemies = new List<Enemy>
        {
            new Enemy(new Vector2(10, 10), _player, _grid, 10),
            new Enemy(new Vector2(15, 8), _player, _grid, 10)
        };

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

        // Update all entities (which applies their velocity)
        _player.Update(gameTime);
        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime);
        }

        // Create a list of all entities for the collision manager
        var allEntities = new List<GameEntity> { _player };
        allEntities.AddRange(_enemies);

        // Detect and resolve collisions
        _collisionManager.DetectAndResolveCollisions(allEntities, gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(transformMatrix: _camera.TransformMatrix);

        _tilemapRenderer.Draw(_camera);

        _player.Draw(_spriteRenderer);
        foreach (var enemy in _enemies)
        {
            enemy.Draw(_spriteRenderer);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

