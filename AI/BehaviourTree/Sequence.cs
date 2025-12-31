using Microsoft.Xna.Framework;

namespace AiGame1.AI.BehaviourTree
{
    public class Sequence : CompositeNode
    {
        public Sequence(params Node[] children) : base(children) { }

        public override NodeStatus Execute(GameTime gameTime)
        {
            foreach (var child in Children)
            {
                switch (child.Execute(gameTime))
                {
                    case NodeStatus.Failure:
                        return NodeStatus.Failure;
                    case NodeStatus.Running:
                        return NodeStatus.Running;
                    case NodeStatus.Success:
                        continue;
                }
            }
            return NodeStatus.Success;
        }
    }
}
