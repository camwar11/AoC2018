using System;
using System.Collections.Generic;
using common;

namespace day9
{
    class Program
    {
        static void Main(string[] args)
        {
            string part1Answer = null;
            string part2Answer = null;
            IEnumerable<string> lines = null;
            using(var reader = new InputReader("input.txt"))
            {
                lines = reader.GetLines();
            
            
                foreach (string line in lines)
                {
                    int players = int.Parse(line.Substring(0, line.IndexOf("players") - 1));

                    int start = line.IndexOf("worth") + 5;
                    int length = line.IndexOf("points") - 1 - start;
                    int lastMarblePoints = int.Parse(line.Substring(start, length));

                    int nextMarble = 0;
                    int currentPlayer = 0;
                    long[] scores = new long[players];
                    long highScore = 0;
                    int pointsFromLastMarble = 0;
                    LinkedList<int> circle = new LinkedList<int>();
                    circle.AddFirst(nextMarble++); // Marble 0 starts in the circle
                    LinkedListNode<int> currentMarble = circle.First;

                    while(nextMarble <= lastMarblePoints * 100)
                    {
                        if(nextMarble == lastMarblePoints + 1)
                        {
                            Console.WriteLine("Part 1: {0}", highScore);
                        }

                        if(nextMarble % 23 == 0)
                        {
                            //scoring
                            currentMarble = GetCircularNode(circle, currentMarble, -7);
                            int removedMarble = currentMarble.Value;
                            var next = currentMarble.Next;
                            circle.Remove(currentMarble);
                            currentMarble = next;
                            pointsFromLastMarble = removedMarble + nextMarble;
                            scores[currentPlayer] += pointsFromLastMarble;
                            if(scores[currentPlayer] > highScore) highScore = scores[currentPlayer];
                            currentPlayer = (currentPlayer + 1) % players;
                            nextMarble++;
                            continue;
                        }

                        currentMarble = GetCircularNode(circle, currentMarble, 1);
                        currentMarble = circle.AddAfter(currentMarble, nextMarble);
                        nextMarble++;
                        currentPlayer = (currentPlayer + 1) % players;
                    }

                    
                    Console.WriteLine("Part 2: {0}", highScore);
                }
            }
        }

        private static LinkedListNode<int> GetCircularNode(LinkedList<int> circle, LinkedListNode<int> currentMarble, int v)
        {
            int incremeter = -1;
            if(v <= 0) incremeter = 1; 
            while(v != 0)
            {   
                if(v > 0)
                {
                    currentMarble = currentMarble.Next;
                    if(currentMarble == null) currentMarble = circle.First;
                }
                else
                {
                    currentMarble = currentMarble.Previous;
                    if(currentMarble == null) currentMarble = circle.Last;
                }

                v += incremeter;
            }

            return currentMarble;
        }

        private static int GetCircularIdx(List<int> circle, int current, int steps)
        {
            int newIdx = current + steps;

            if(newIdx < 0)
            {
                return circle.Count + newIdx;
            }
            else if(newIdx >= circle.Count)
            {
                return newIdx - circle.Count;
            }
            else
            {
                return newIdx;
            }
        }
    }
}
