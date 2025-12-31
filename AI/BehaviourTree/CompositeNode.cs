using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AiGame1.AI.BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        protected readonly List<Node> Children = new List<Node>();

        public CompositeNode(params Node[] children)
        {
            Children.AddRange(children);
        }
    }
}
