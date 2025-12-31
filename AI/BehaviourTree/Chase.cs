using Microsoft.Xna.Framework;
using AiGame1.Entities;

namespace AiGame1.AI.BehaviourTree
{
    public class Chase : Node
    {
        private readonly Enemy _enemy;
        private readonly Player _player;

        public Chase(Enemy enemy, Player player)
        {
            _enemy = enemy;
            _player = player;
        }

        public override NodeStatus Execute(GameTime gameTime)
        {
            Vector2 enemyGridPos = Core.Camera.WorldToGrid(_enemy.WorldPosition, Graphics.TilemapRenderer.TileWidth, Graphics.TilemapRenderer.TileHeight);
            Vector2 playerGridPos = Core.Camera.WorldToGrid(_player.WorldPosition, Graphics.TilemapRenderer.TileWidth, Graphics.TilemapRenderer.TileHeight);

            // Set a path to the player's current position
            // This will be recalculated each time Chase is executed, leading to pursuit behavior.
            _enemy.SetPath(enemyGridPos, playerGridPos);
            
            // The Chase action itself just sets the path. The enemy's movement logic handles the rest.
            // We return Running because chasing is an ongoing process.
            return NodeStatus.Running;
        }
    }
}
