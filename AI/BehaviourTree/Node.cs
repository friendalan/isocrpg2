using Microsoft.Xna.Framework;

namespace AiGame1.AI.BehaviourTree
{
    public abstract class Node
    {
        public abstract NodeStatus Execute(GameTime gameTime);
    }
}
