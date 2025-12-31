using Microsoft.Xna.Framework;
using System.Collections.Generic;
using AiGame1.AI;
using AiGame1.AI.BehaviourTree;
using AiGame1.Graphics;
using AiGame1.World;
using AiGame1.Core;

namespace AiGame1.Entities
{
    public class Enemy : GameEntity
    {
        private readonly Grid _grid;
        private readonly Node _root;
        private List<Vector2> _path;

        private const float Speed = 80f; // Slower than player
        private const float ChaseRange = 250f;
        private const float AttackRange = 35f;

        public Enemy(Vector2 startGridPos, Player player, Grid grid)
        {
            _grid = grid;
            _path = new List<Vector2>();

            WorldPosition = Camera.IsometricProjection((int)startGridPos.X, (int)startGridPos.Y, TilemapRenderer.TileWidth, TilemapRenderer.TileHeight);

            _root = BuildBehaviourTree(player);
        }

        private Node BuildBehaviourTree(Player player)
        {
            return new Selector(
                // Attack sequence
                new Sequence(
                    new IsPlayerInRange(this, player, AttackRange),
                    new Attack(this, player, AttackRange)
                ),
                // Chase sequence
                new Sequence(
                    new IsPlayerInRange(this, player, ChaseRange),
                    new Chase(this, player)
                ),
                // Wander is the default action
                new Wander(this, _grid)
            );
        }

        public override void Update(GameTime gameTime)
        {
            _root.Execute(gameTime);
            Move(gameTime);
            UpdateBoundingBox(20, 20);
        }

        private void Move(GameTime gameTime)
        {
            if (_path == null || _path.Count == 0)
            {
                Velocity *= 0.9f;
                if (Velocity.Length() < 1.0f)
                {
                    Velocity = Vector2.Zero;
                }
            }
            else
            {
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
                    WorldPosition = targetWorldPos;
                    _path.RemoveAt(0);
                    if (_path.Count == 0) Velocity = Vector2.Zero;
                }
            }
            
            WorldPosition += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void SetPath(Vector2 startGridPos, Vector2 endGridPos)
        {
            List<Vector2> newPath = AStarPathfinder.FindPath(_grid, startGridPos, endGridPos);
            if (newPath != null)
            {
                _path = newPath;
                if (_path.Count > 0)
                {
                    _path.RemoveAt(0); // Remove the start node
                }
            }
        }

        public bool HasPath()
        {
            return _path != null && _path.Count > 0;
        }

        public override void Draw(SpriteRenderer renderer)
        {
            renderer.DrawCircle(WorldPosition, 10, Color.Red);
        }
    }
}
