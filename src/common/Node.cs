using System;
using System.Collections.Generic;
using System.Linq;

namespace common
{
    public class Node<T, V>
    {
        private List<Node<T, V>> _children;

        public T ID { get; private set; }
        public V Data { get; set; }
        public Node<T, V> Parent { get; private set; }

        protected Tree<T,V> _tree;

        protected internal Node(Tree<T,V> tree, T ID, V data)
        {
            _tree = tree;
            this.ID = ID;
            Data = data;
            _children = new List<Node<T, V>>();
            Parent = null;
        }

        public void AddChild(Node<T, V> child)
        {
            _children.Add(child);
            child.Parent = this;
        }

        public IEnumerable<Node<T,V>> Children
        {
            get {return _children;}
        }

        public IEnumerable<Node<T,V>> Decendents
        {
            get {return _children.Concat(_children.SelectMany(x => x.Decendents));}
        }
    }
}