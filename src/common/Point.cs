using System;

namespace common
{
    public class Point
    {
        public static long nextId = 0;
        public Point(string x, string y) : this(long.Parse(x), long.Parse(y))
        {
        }

        public Point(long x, long y)
        {
            X = x;
            Y = y;
            Id = nextId++;
        }
        public long X {get; private set;}
        public long Y {get; private set;}

        public long Id {get; private set;}

        public static long ManhattenDistance(long x1, long x2, long y1, long y2)
        {
            return Math.Abs(x1-x2) + Math.Abs(y1-y2);
        }

        public static long ManhattenDistance(Point one, Point two)
        {
            return ManhattenDistance(one.X, two.X, one.Y, two.Y);
        }

        public static long ManhattenDistance(Point one, long x2, long y2)
        {
            return ManhattenDistance(one.X, x2, one.Y, y2);
        }

        public long ManhattenDistance(Point other)
        {
            return Point.ManhattenDistance(this, other);
        }

        public long ManhattenDistance(long otherX, long otherY)
        {
            return Point.ManhattenDistance(this, otherX, otherY);
        }
    }
}