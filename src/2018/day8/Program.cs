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
            Tree<int, LicenseFileData> tree = new Tree<int, LicenseFileData>();
            Node<int, LicenseFileData> currentNode = null;
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
                        currentNode = tree.CreateNode(null);
                        currentNode.Data = new LicenseFileData(tree, currentNode, item);
                        continue;
                    }

                    if(currentNode.Data.NeedsMetadataHeader)
                    {
                        currentNode.Data.SetMetadata(item);
                        continue;
                    }

                    if(currentNode.Data.NeedsChildren)
                    {
                        currentNode = currentNode.Data.AddChild(item);
                        continue;
                    }

                    if(currentNode.Data.NeedsMetadata)
                    {
                        currentNode.Data.AddMetadata(item);
                        continue;
                    }

                    // node is finished, go back to the parent.
                    currentNode = currentNode.Parent;
                    increment = 0;
                }
            }

            Console.WriteLine("Part 1: {0}", currentNode.Data.CountMetadata());
            Console.WriteLine("Part 2: {0}", currentNode.Data.GetNodeValue());
        }

        private class LicenseFileData
        {
            internal int RemainingChildren;
            internal int RemainingMetadata;

            internal List<int> Data = new List<int>();

            internal void AddMetadata(string metadata)
            {
                Data.Add(int.Parse(metadata));
                RemainingMetadata--;
            }

            internal Node<int, LicenseFileData> AddChild(string childNumOfChildren)
            {
                var newNode = _tree.CreateNode(null);
                LicenseFileData data = new LicenseFileData(_tree, newNode, childNumOfChildren);
                newNode.Data = data;
                _node.AddChild(newNode);
                RemainingChildren--;
                return newNode;
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
            private Tree<int, LicenseFileData> _tree;           
            private Node<int, LicenseFileData> _node; 

            internal LicenseFileData(Tree<int, LicenseFileData> tree, Node<int, LicenseFileData> node, string remainingChildren)
            {
                _tree = tree;
                _node = node;
                RemainingChildren = int.Parse(remainingChildren);
                NeedsMetadataHeader = true;

                _node.Data = this;
            }

            public int CountMetadata()
            {
                return Data.Sum() + _node.Children.Select(x => x.Data.CountMetadata()).Sum();
            }

            public int GetNodeValue()
            {
                if(!_node.Children.Any()) return Data.Sum();

                return Data.Select(x => {
                    int idx = x-1;
                    if(_node.Children.Count() <= idx) return 0;
                    return _node.Children.ElementAt(idx).Data.GetNodeValue();
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
