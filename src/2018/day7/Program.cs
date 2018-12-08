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
            Dictionary<char, Node> nodes = ReadNodes();

            Part1(nodes);

            nodes = ReadNodes();

            Part2(nodes);
        }

        private static void Part1(Dictionary<char, Node> nodes)
        {
            SortedSet<Node> roots = new SortedSet<Node>(nodes.Values.Select(x => x.Root.NextNode()));
            SortedSet<Node> bestValue = new SortedSet<Node>(roots.Select(x => x.NextNode()).Where(x => x != null));

            Console.Write("Part 1: ");
            do
            {
                var next = bestValue.Min;
                next.WasRun = true;
                Console.Write(next.ID);
                bestValue = new SortedSet<Node>(roots.Select(x => x.NextNode()).Where(x => x != null));
            } while (bestValue.Any());

            Console.WriteLine();
        }

        private static void Part2(Dictionary<char, Node> nodes)
        {
            SortedSet<Node> roots = new SortedSet<Node>(nodes.Values.Select(x => x.Root.NextNode()));
            SortedSet<Node> bestValue = new SortedSet<Node>(roots.Select(x => x.NextNode()).Where(x => x != null));
            const int WORKERS = 5;
            int[] workerTimes = new int[WORKERS];
            Node[] workItems = new Node[WORKERS];

            Console.Write("Part 2: ");
            for (int currentTime = 0; true; currentTime++)
            {
                for (int worker = 0; worker < WORKERS; worker++)
                {
                    if(workerTimes[worker] <= currentTime)
                    {
                        var finishedWork = workItems[worker];
                        if(finishedWork != null)
                        {
                            Console.Write(finishedWork.ID);
                            finishedWork.FinishTask();
                            workItems[worker] = null;
                        }

                        bestValue = new SortedSet<Node>(roots.Select(x => x.NextNode()).Where(x => x != null));

                        if(!bestValue.Any()) continue;

                        var nextTask = bestValue.Min;
                        int endTime = nextTask.StartTask(currentTime);
                        workerTimes[worker] = endTime;
                        workItems[worker] = nextTask;
                    }
                }
                
                if(!workItems.Any(x => x != null))
                {
                    Console.WriteLine(currentTime);
                    break;
                } 
            }

            Console.WriteLine();
        }

        private static Dictionary<char, Node> ReadNodes()
        {
            Dictionary<char, Node> nodes = new Dictionary<char, Node>();
            const int FIRST_IDX = 5;
            const int SECOND_IDX = 36;
            using (var reader = new InputReader("input.txt"))
            {
                foreach (string line in reader.GetLines())
                {
                    char firstStep = line[FIRST_IDX];
                    char secondStep = line[SECOND_IDX];

                    Node firstNode;
                    if (!nodes.TryGetValue(firstStep, out firstNode))
                    {
                        firstNode = new Node(firstStep);
                        nodes.Add(firstStep, firstNode);
                    }

                    Node secondNode;
                    if (!nodes.TryGetValue(secondStep, out secondNode))
                    {
                        secondNode = new Node(secondStep);
                        nodes.Add(secondStep, secondNode);
                    }

                    firstNode.AddChild(secondNode);
                }
            }

            return nodes;
        }

        private class Node : IComparable<Node>
        {
            private const int TASK_LENGTH = 60;
            private const int CHARS_BEFORE_A = 64;
            internal char ID;
            internal bool WasRun;
            internal bool IsBeingWorked;
            private SortedSet<Node> _children;
            private int doneTime = int.MaxValue;

            internal List<Node> Parents;

            internal Node Root 
            {
                get
                {
                    if(Parents.FirstOrDefault() == null) return this;
                    return Parents.FirstOrDefault().Root;
                }
            }

            private int TaskLength 
            {
                get 
                {
                    return TASK_LENGTH + (int) ID - CHARS_BEFORE_A;
                }
            }

            internal Node(char ID)
            {
                this.ID = ID;
                WasRun = false;
                _children = new SortedSet<Node>();
                Parents = new List<Node>();
                IsBeingWorked = false;
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
                if(IsBeingWorked) return null;

                if(this.Parents.Any(x => !x.WasRun || x.IsBeingWorked)) return null;

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

            internal int StartTask(int currentTime)
            {
                IsBeingWorked = true;
                return doneTime = currentTime + TaskLength;
            }

            internal void FinishTask()
            {
                WasRun = true;
                IsBeingWorked = false;
            }
        }
    }
}
