using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybers.Infrustructure.models
{
    /// <summary>
    /// Represents an undirected graph.
    /// </summary>
    public class Graph<T> where T : IEquatable<T>
    {

        private readonly Dictionary<T, Dictionary<T, bool>> _adjacencyMatrix;
        private int _numEdges;
        private int _numVertices;

        public Graph()
        {
            _adjacencyMatrix = new Dictionary<T, Dictionary<T, bool>>();
        }

        public Graph(Graph<T> g)
        {
            _adjacencyMatrix = new Dictionary<T, Dictionary<T, bool>>();
            foreach (var ilist in g._adjacencyMatrix)
            {
                _adjacencyMatrix[ilist.Key] = new Dictionary<T, bool>(ilist.Value);
            }
            _numEdges = g._numEdges;
        }

        /// <summary>
        /// Adds a vertex to the graph.
        /// </summary>
        /// <param name="vertex">The vertex to add.</param>
        public void AddVertex(T vertex)
        {
            EnsureIncidenceList(vertex);
            _numVertices++;
        }

        /// <summary>
        /// Adds to the weight between node1 and node2 (edges are assumed to start with weight 0).
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        public void AddEdge(T vertex1, T vertex2)
        {
            if (!IsAdjacentVertices(vertex1, vertex2) && !IsAdjacentVertices(vertex2, vertex1))
            {
                AddDirectedEdge(vertex1, vertex2);
                if (!vertex1.Equals(vertex2))
                {
                    AddDirectedEdge(vertex2, vertex1);
                }
                _numEdges += 1;
            }

        }

        private void AddDirectedEdge(T vertex1, T vertex2)
        {
            var ilist = EnsureIncidenceList(vertex1);
            ilist[vertex2] = true;
        }

        private Dictionary<T, bool> EnsureIncidenceList(T vertex)
        {
            Dictionary<T, bool> ilist;
            if (!_adjacencyMatrix.TryGetValue(vertex, out ilist))
            {
                ilist = _adjacencyMatrix[vertex] = new Dictionary<T, bool>();
            }
            return ilist;
        }

        /// <summary>
        /// The number of edges in the graph.
        /// </summary>
        public int NumberOfEdges => _numEdges;

        public int NumberOfVertices => _numVertices;

        public List<T> AdjacentVertices(T vertex)
        {
            _adjacencyMatrix.TryGetValue(vertex, out Dictionary<T, bool> ilist);
            return ilist?.Keys.ToList();
        }

        public bool IsAdjacentVertices(T vertex1, T vertex2)
        {
            Dictionary<T, bool> ilist;
            bool isAdjacent;
            return _adjacencyMatrix.TryGetValue(vertex1, out ilist) && ilist.TryGetValue(vertex2, out isAdjacent) && isAdjacent;
        }

        /// <summary>
        /// Computes the degree of a vertex, as the number of incident edges.
        /// </summary>
        /// <param name="vertex">The vertex whose edges' should be counted.</param>
        /// <returns>The degree of the vertex.</returns>
        public double Degree(T vertex)
        {
            _adjacencyMatrix.TryGetValue(vertex, out Dictionary<T, bool> ilist);
            return ilist?.Where(pair => pair.Value).Count() ?? 0;
        }

        /// <summary>
        /// An iterator for the nodes in the graph.
        /// </summary>
        public IEnumerable<T> Vertices => _adjacencyMatrix.Keys;

        /// <summary>
        /// An iterator for the edges in the graph.
        /// </summary>
        public IEnumerable<Edge<T>> Edges
        {
            get
            {
                foreach (var entry1 in _adjacencyMatrix)
                {
                    foreach (var entry2 in entry1.Value)
                    {
                        yield return new Edge<T>(entry1.Key, entry2.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of incident edges for a node.
        /// </summary>
        /// <param name="node">The node from which the returned edges will be incident.</param>
        /// <returns>An enumeration of incident edges.</returns>
        public IEnumerable<Edge<T>> IncidentEdges(T node)
        {
            Dictionary<T, bool> incidence;
            if (_adjacencyMatrix.TryGetValue(node, out incidence))
            {
                foreach (var entry in incidence)
                {
                    yield return new Edge<T>(node, entry.Key);
                }
            }
        }
    }
}
