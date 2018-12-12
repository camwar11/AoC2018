using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day6
{
    class Program
    {
        static void Main(string[] args)
        {
            List<AreaPoint> points = new List<AreaPoint>();
            int top = int.MaxValue;
            int left = int.MaxValue;
            int bottom = int.MinValue;
            int right = int.MinValue;
            const int PART_TWO_MAX_DIST = 10000;
            int part2Size = 0;
            using(var reader = new InputReader("input.txt"))
            {
                foreach (string line in reader.GetLines())
                {
                    var split = line.Split(',').Select(x => int.Parse(x.Trim())).ToArray();
                    
                    if(split[0] < left) left = split[0];
                    if(split[0] > right) right = split[0];
                    if(split[1] < top) top = split[1];
                    if(split[1] > bottom) bottom = split[1];
                    
                    
                    points.Add(new AreaPoint(split[0], split[1]));
                }
            }

            for (int y = top; y <= bottom; y++)
            {
                // Assume infinites if they hit the outside of the box of coordinates
                //Console.Write("\n");
                bool isXInfinite = y == top || y == bottom;
                for (int x = left; x <= right; x++)
                {
                    AreaPoint closestPoint = null;
                    bool hasTie = false;
                    long closestDistance = int.MaxValue;
                    // Probably a better way than brute forcing all the points...
                    // but let's just do this for now.
                    long totalPointDistance = 0;
                    foreach (var point in points)
                    {
                        long distance = point.ManhattenDistance(x, y);
                        totalPointDistance += distance;
                        if(distance == closestDistance && closestPoint != point)
                        {
                            hasTie = true;
                        }

                        if(distance < closestDistance)
                        {
                            hasTie = false;
                            closestPoint = point;
                            closestDistance = distance;
                        }
                    }

                    if(totalPointDistance < PART_TWO_MAX_DIST) part2Size++;

                    if(hasTie) 
                    {
                        //Console.Write(".");
                        continue;
                    }

                    if(closestPoint != null)
                    {
                        //Console.Write(closestPoint.Id);

                        if(isXInfinite || x == left || x == right)
                        {
                            closestPoint.Infinite = true;
                            continue;
                        }

                        closestPoint.Area++;
                    }
                }
            }
            
            points.Sort(new PointComparer());
            var largestPoint = points.Last();

            Console.WriteLine("Part 1: id={0}, area={1}", largestPoint.Id, largestPoint.Area );
            Console.WriteLine("Part 2: {0}", part2Size);
        }

        private class PointComparer : IComparer<AreaPoint>
        {
            public int Compare(AreaPoint x, AreaPoint y)
            {
                if(x.Infinite && !y.Infinite) return -1;
                if(!x.Infinite && y.Infinite) return 1;
                if(x.Area == y.Area) return x.Id.CompareTo(y.Id);
                return x.Area.CompareTo(y.Area);
            }
        }

        private class AreaPoint : Point
        {
            internal AreaPoint(string x, string y) : this(int.Parse(x), int.Parse(y))
            {
            }

            internal AreaPoint(int x, int y) : base(x, y)
            {
                Infinite = false;
                Area = 0;
            }

            internal bool Infinite {get; set;}

            internal int Area {get; set;}
        }
    }
}
