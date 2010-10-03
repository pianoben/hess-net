using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HessNet.Hessian
{
    public class HessianNode
    {
        private readonly Collection<HessianNode> nodes;

        public Collection<HessianNode> Nodes
        {
            get { return nodes; }
        }

        public HessianNode()
            : this(null)
        {

        }

        public HessianNode(IList<HessianNode> nodes)
        {
            this.nodes = (nodes as Collection<HessianNode>) ?? new Collection<HessianNode>(nodes);
        }
    }
}
