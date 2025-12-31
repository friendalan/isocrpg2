using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using AiGame1.Entities;
using AiGame1.World;

namespace AiGame1.AI.BehaviourTree
{
    public class Wander : Node
    {
        private readonly Enemy _enemy;
        private readonly Grid _grid;

        public Wander(Enemy enemy, Grid grid)
        {
            _enemy = enemy;
            _grid = grid;
        }

        public override NodeStatus Execute(GameTime gameTime)
        {
            // If enemy already has a path, it's already wandering.
            if (_enemy.HasPath())
            {
                return NodeStatus.Running;
            }

            // Find a random walkable tile to wander to.
            Random random = new Random();
            int searchRadius = 5; // How far to look for a wander target
            Vector2 currentGridPos = Core.Camera.WorldToGrid(_enemy.WorldPosition, Graphics.TilemapRenderer.TileWidth, Graphics.TilemapRenderer.TileHeight);

            for (int i = 0; i < 10; i++) // Try 10 times to find a valid spot
            {
                int x = (int)currentGridPos.X + random.Next(-searchRadius, searchRadius + 1);
                int y = (int)currentGridPos.Y + random.Next(-searchRadius, searchRadius + 1);

                if (x >= 0 && x < _grid.Width && y >= 0 && y < _grid.Height && _grid.Cells[x, y].IsWalkable)
                {
                    _enemy.SetPath(currentGridPos, new Vector2(x, y));
                    return NodeStatus.Running;
                }
            }
            
            // Failed to find a wander spot
            return NodeStatus.Failure;
        }
    }
}
