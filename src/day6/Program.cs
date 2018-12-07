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
            List<Point> points = new List<Point>();
            int top = int.MaxValue;
            int left = int.MaxValue;
            int bottom = int.MinValue;
            int right = int.MinValue;
            using(var reader = new InputReader("input.txt"))
            {
                foreach (string line in reader.GetLines())
                {
                    var split = line.Split(',').Select(x => int.Parse(x.Trim())).ToArray();
                    
                    if(split[0] < left) left = split[0];
                    if(split[0] > right) right = split[0];
                    if(split[1] < top) top = split[1];
                    if(split[1] > bottom) bottom = split[1];
                    
                    
                    points.Add(new Point(split[0], split[1]));
                }
            }

            for (int y = top; y <= bottom; y++)
            {
                // Assume infinites if they hit the outside of the box of coordinates
                Console.Write("\n");
                bool isXInfinite = y == top || y == bottom;
                for (int x = left; x <= right; x++)
                {
                    Point closestPoint = null;
                    bool hasTie = false;
                    int closestDistance = int.MaxValue;
                    // Probably a better way than brute forcing all the points...
                    // but let's just do this for now.
                    foreach (var point in points)
                    {
                        int distance = Distance(point, x, y);
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

                    if(hasTie) 
                    {
                        Console.Write(".");
                        continue;
                    }

                    if(closestPoint != null)
                    {
                        Console.Write(closestPoint.Id);

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
        }

        private class PointComparer : IComparer<Point>
        {
            public int Compare(Point x, Point y)
            {
                if(x.Infinite && !y.Infinite) return -1;
                if(!x.Infinite && y.Infinite) return 1;
                if(x.Area == y.Area) return x.Id.CompareTo(y.Id);
                return x.Area.CompareTo(y.Area);
            }
        }

        private static int Distance(int x1, int x2, int y1, int y2)
        {
            return Math.Abs(x1-x2) + Math.Abs(y1-y2);
        }

        private static int Distance(Point one, Point two)
        {
            return Distance(one.X, two.X, one.Y, two.Y);
        }

        private static int Distance(Point one, int x2, int y2)
        {
            return Distance(one.X, x2, one.Y, y2);
        }

        private class Point
        {
            public static int nextId = 0;
            internal Point(string x, string y) : this(int.Parse(x), int.Parse(y))
            {
            }

            internal Point(int x, int y)
            {
                X = x;
                Y = y;
                Id = nextId++;
                Infinite = false;
                Area = 0;
            }
            internal int X {get; private set;}
            internal int Y {get; private set;}

            internal int Id {get; private set;}

            internal bool Infinite {get; set;}

            internal int Area {get; set;}
        }
    }
}
