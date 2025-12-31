using Microsoft.Xna.Framework;
using AiGame1.Entities;

namespace AiGame1.AI.BehaviourTree
{
    public class Attack : Node
    {
        private readonly Enemy _enemy;
        private readonly Player _player;
        private readonly float _attackRange;

        public Attack(Enemy enemy, Player player, float attackRange)
        {
            _enemy = enemy;
            _player = player;
            _attackRange = attackRange;
        }

        public override NodeStatus Execute(GameTime gameTime)
        {
            if (Vector2.Distance(_enemy.WorldPosition, _player.WorldPosition) <= _attackRange)
            {
                // In a real game, this would trigger an attack animation, deal damage, etc.
                // For now, we'll just log to the console or have it return success.
                System.Console.WriteLine("Enemy attacks!");
                return NodeStatus.Success;
            }
            return NodeStatus.Failure;
        }
    }
}
