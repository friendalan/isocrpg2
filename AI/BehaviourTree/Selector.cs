using Microsoft.Xna.Framework;

namespace AiGame1.AI.BehaviourTree
{
    public class Selector : CompositeNode
    {
        public Selector(params Node[] children) : base(children) { }

        public override NodeStatus Execute(GameTime gameTime)
        {
            foreach (var child in Children)
            {
                switch (child.Execute(gameTime))
                {
                    case NodeStatus.Success:
                        return NodeStatus.Success;
                    case NodeStatus.Running:
                        return NodeStatus.Running;
                    case NodeStatus.Failure:
                        continue;
                }
            }
            return NodeStatus.Failure;
        }
    }
}
