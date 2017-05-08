using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;
using QuickGraph;
using QuickGraph.Algorithms.KernighanLinAlgoritm;

namespace ILouvainLibrary
{
    public class ILouvain
    {
        private UndirectedGraph<User, Edge<User>> _graph;

        public ILouvain(UndirectedGraph<User, Edge<User>> graph)
        {
            _graph = graph;
        }

        private double CalculateEuclideanDistance(User u1, User u2)
        {
            double distance = 0;
            foreach (var key in u1.Attributes.Keys)
            {
                var val1 = u1.Attributes[key];
                var val2 = u2.Attributes[key];
                distance += Math.Pow(val1 - val2, 2);
            }
            return distance;
        }

        private User CalculateCenterOfGravity(IEnumerable<User> verticies)
        {
            var attr = verticies.First().Attributes.Keys;
            var gUser = new User();
            var size = verticies.Count();
           
            foreach (var key in attr)
            {
                long sum = verticies.Sum(vertex => vertex.Attributes[key]);
                gUser.Attributes[key] = sum/size;
            }

            return gUser;
        }

        private double CalculateInertia(IEnumerable<User> verticies)
        {
            var gUser = CalculateCenterOfGravity(verticies);
            return verticies.Sum(vertex => CalculateEuclideanDistance(vertex, gUser));
        }

        private double CalculateInertia(IEnumerable<User> verticies, User v)
            => verticies.Sum(vertex => CalculateEuclideanDistance(vertex, v));
       
    }
}
