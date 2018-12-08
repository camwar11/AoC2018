using System;
using System.Linq;

namespace common
{
    public class Tree<T, V>
    {
        public Node<T, V> Root { get; set; }

        public Func<T> IDGenerator { get; set; }

        public Tree(Func<T> IDGenerator = null)
        {
            this.IDGenerator = IDGenerator;
        }

        public Node<T,V> CreateNode(T id, V data)
        {
            return new Node<T,V>(this, id, data);
        }

        public Node<T,V> CreateNode(V data)
        {
            return CreateNode(GetNextId(), data);
        }

        public T GetNextId()
        {
            return IDGenerator == null ? default(T) : IDGenerator();
        }
    }
}