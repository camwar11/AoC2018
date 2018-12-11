using System;

namespace common
{
    public class Point
    {
        public static int nextId = 0;
        public Point(string x, string y) : this(int.Parse(x), int.Parse(y))
        {
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
            Id = nextId++;
        }
        public int X {get; private set;}
        public int Y {get; private set;}

        public int Id {get; private set;}

        public static int ManhattenDistance(int x1, int x2, int y1, int y2)
        {
            return Math.Abs(x1-x2) + Math.Abs(y1-y2);
        }

        public static int ManhattenDistance(Point one, Point two)
        {
            return ManhattenDistance(one.X, two.X, one.Y, two.Y);
        }

        public static int ManhattenDistance(Point one, int x2, int y2)
        {
            return ManhattenDistance(one.X, x2, one.Y, y2);
        }

        public int ManhattenDistance(Point other)
        {
            return Point.ManhattenDistance(this, other);
        }

        public int ManhattenDistance(int otherX, int otherY)
        {
            return Point.ManhattenDistance(this, otherX, otherY);
        }
    }
}