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
            string line = null;
            using(var reader = new InputReader("input.txt"))
            {
                line = reader.GetNextLine();
            }

            int players = int.Parse(line.Substring(0, line.IndexOf("players") - 1));

            int start = line.IndexOf("worth") + 5;
            int length = line.IndexOf("points") - 1 - start;
            int lastMarblePoints = int.Parse(line.Substring(start, length));

            int currentMarble = 0;
            int nextMarble = 0;
            int currentPlayer = 0;
            int[] scores = new int[players];
            int highScore = 0;
            int pointsFromLastMarble = 0;
            List<int> circle = new List<int>();
            circle.Add(nextMarble++); // Marble 0 starts in the circle

            while(nextMarble <= lastMarblePoints * 100)
            {
                if(nextMarble == lastMarblePoints)
                {
                    Console.WriteLine("Part 1: {0}", highScore);
                }

                if(nextMarble % 23 == 0)
                {
                    //scoring
                    currentMarble = GetCircularIdx(circle, currentMarble, -7);
                    int removedMarble = circle[currentMarble];
                    circle.RemoveAt(currentMarble); // Should collapse so we dont need to move the pointer again.
                    pointsFromLastMarble = removedMarble + nextMarble;
                    scores[currentPlayer] += pointsFromLastMarble;
                    if(scores[currentPlayer] > highScore) highScore = scores[currentPlayer];
                    currentPlayer = (currentPlayer + 1) % players;
                    nextMarble++;
                    continue;
                }

                currentMarble = GetCircularIdx(circle, currentMarble, 2);
                circle.Insert(currentMarble, nextMarble);
                nextMarble++;
                currentPlayer = (currentPlayer + 1) % players;
            }

            
            Console.WriteLine("Part 2: {0}", highScore);
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
