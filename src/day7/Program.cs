using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day7
{
    class Program
    {
        static void Main(string[] args)
        {
            const int FIRST_IDX = 5;
            const int SECOND_IDX = 36;
            Dictionary<char, Node> nodes = new Dictionary<char, Node>();
            using(var reader = new InputReader("input.txt"))
            {
                foreach (string line in reader.GetLines())
                {
                    char firstStep = line[FIRST_IDX];
                    char secondStep = line[SECOND_IDX];

                    Node firstNode;
                    if(!nodes.TryGetValue(firstStep, out firstNode))
                    {
                        firstNode = new Node(firstStep);
                        nodes.Add(firstStep, firstNode);
                    }

                    Node secondNode;
                    if(!nodes.TryGetValue(secondStep, out secondNode))
                    {
                        secondNode = new Node(secondStep);
                        nodes.Add(secondStep, secondNode);
                    }

                    firstNode.AddChild(secondNode);
                }
            }

            SortedSet<Node> roots = new SortedSet<Node>(nodes.Values.Select(x => x.Root.NextNode()));
            SortedSet<Node> bestValue = new SortedSet<Node>(roots.Select(x => x.NextNode()).Where(x => x != null));
            
            do
            {
                var next = bestValue.Min;
                next.WasRun = true;
                Console.Write(next.ID);
                bestValue = new SortedSet<Node>(roots.Select(x => x.NextNode()).Where(x => x != null));
            } while(bestValue.Any());
        }

        private class Node : IComparable<Node>
        {
            internal char ID;
            internal bool WasRun;
            private SortedSet<Node> _children;

            internal List<Node> Parents;

            internal Node Root 
            {
                get
                {
                    if(Parents.FirstOrDefault() == null) return this;
                    return Parents.FirstOrDefault().Root;
                }
            }

            internal Node(char ID)
            {
                this.ID = ID;
                WasRun = false;
                _children = new SortedSet<Node>();
                Parents = new List<Node>();
            }

            public int CompareTo(Node other)
            {
                return this.ID.CompareTo(other.ID);
            }

            internal void AddChild(Node child)
            {
                _children.Add(child);
                child.Parents.Add(this);
            }

            internal Node NextNode()
            {
                if(this.Parents.Any(x => !x.WasRun)) return null;

                if(!WasRun)
                {
                    return this;
                }

                SortedSet<Node> potentialNodes = new SortedSet<Node>();

                foreach (var child in _children)
                {
                    var childNext = child.NextNode();
                    if(childNext != null) potentialNodes.Add(childNext);
                }

                if(!potentialNodes.Any()) return null;

                return potentialNodes.Min;
            }
        }
    }
}
