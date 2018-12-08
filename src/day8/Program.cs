using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day8
{
    class Program
    {
        static void Main(string[] args)
        {
            Node currentNode = null;
            using(var reader = new InputReader("input.txt"))
            {
                string input = reader.GetNextLine();
                var inputItems = input.Split(' ');
                int increment = 1;

                for (int i = 0; i < inputItems.Length; i+=increment)
                {
                    increment = 1;
                    string item = inputItems[i];
                    if(currentNode == null)
                    {
                        currentNode = new Node(item);
                        continue;
                    }

                    if(currentNode.NeedsMetadataHeader)
                    {
                        currentNode.SetMetadata(item);
                        continue;
                    }

                    if(currentNode.NeedsChildren)
                    {
                        currentNode = currentNode.AddChild(item);
                        continue;
                    }

                    if(currentNode.NeedsMetadata)
                    {
                        currentNode.AddMetadata(item);
                        continue;
                    }

                    // node is finished, go back to the parent.
                    currentNode = currentNode.Parent;
                    increment = 0;
                }
            }

            Console.WriteLine("Part 1: {0}", currentNode.CountMetadata());
            Console.WriteLine("Part 2: {0}", currentNode.GetNodeValue());
        }

        private class Node
        {
            private static int nextId = 0;
            internal int ID;
            private List<Node> Children = new List<Node>();
            private List<int> Metadata = new List<int>();

            internal int RemainingChildren;
            internal int RemainingMetadata;

            internal void AddMetadata(string metadata)
            {
                Metadata.Add(int.Parse(metadata));
                RemainingMetadata--;
            }

            internal Node AddChild(string childNumOfChildren)
            {
                Node newChild = new Node(childNumOfChildren);
                newChild.Parent = this;
                Children.Add(newChild);
                RemainingChildren--;
                return newChild;
            }

            public bool NeedsMetadata 
            { 
                get
                {
                    return RemainingMetadata > 0;
                }   
            }

            public bool NeedsChildren 
            { 
                get
                {
                    return RemainingChildren > 0;
                }   
            }
            public bool NeedsMetadataHeader { get; internal set; }
            public Node Parent { get; private set; }

            internal Node(string remainingChildren)
            {
                ID = nextId++;
                RemainingChildren = int.Parse(remainingChildren);
                NeedsMetadataHeader = true;
                Parent = null;
            }

            public int CountMetadata()
            {
                return Metadata.Sum() + Children.Select(x => x.CountMetadata()).Sum();
            }

            public int GetNodeValue()
            {
                if(!Children.Any()) return Metadata.Sum();

                return Metadata.Select(x => {
                    int idx = x-1;
                    if(Children.Count <= idx) return 0;
                    return Children[idx].GetNodeValue();
                }).Sum();
            }

            internal void SetMetadata(string item)
            {
                RemainingMetadata = int.Parse(item);
                NeedsMetadataHeader = false;
            }
        }
    }
}
