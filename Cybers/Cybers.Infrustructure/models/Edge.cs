using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.models
{
    /// <summary>
    /// Represents an edge between nodes.
    /// </summary>
    public class Edge<T> where T : IEquatable<T>
    {
        public T FromNode;
        public T ToNode;

        /// <summary>
        /// Constructs an edge between two nodes.
        /// </summary>
        /// <param name="n1">The first node.</param>
        /// <param name="n2">The second node.</param>
        public Edge(T n1, T n2)
        {
            FromNode = n1;
            ToNode = n2;
        }

        /// <summary>
        /// True iff the two nodes of the edge are the same.
        /// </summary>
        public bool SelfLoop => FromNode.Equals(ToNode);
    }
}
