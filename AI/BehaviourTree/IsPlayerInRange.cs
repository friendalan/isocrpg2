using Microsoft.Xna.Framework;
using AiGame1.Entities;

namespace AiGame1.AI.BehaviourTree
{
    public class IsPlayerInRange : Node
    {
        private readonly Enemy _enemy;
        private readonly Player _player;
        private readonly float _range;

        public IsPlayerInRange(Enemy enemy, Player player, float range)
        {
            _enemy = enemy;
            _player = player;
            _range = range;
        }

        public override NodeStatus Execute(GameTime gameTime)
        {
            if (Vector2.Distance(_enemy.WorldPosition, _player.WorldPosition) <= _range)
            {
                return NodeStatus.Success;
            }
            return NodeStatus.Failure;
        }
    }
}
