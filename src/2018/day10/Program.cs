using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using common;

namespace day10
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Vector> points = new List<Vector>();
            using (var reader = new InputReader("input.txt"))
            {
                foreach (var line in reader.GetLines())
                {
                    var split = line.Split("velocity");
                    var position = ParseXY(split[0]);
                    var velocity = ParseXY(split[1]);

                    points.Add(new Vector(position[0], position[1], velocity[0], velocity[1]));
                }
            }
            IEnumerable<Vector> newPoints = points;

            if(File.Exists("picture.txt")) File.Delete("picture.txt");

            long bestMaxX = long.MinValue;
            long bestMaxY = long.MinValue;
            long bestMinX = long.MaxValue;
            long bestMinY = long.MaxValue;
            IEnumerable<Vector> bestPicture = null;
            long runsWithoutBestPicture = 0;
            long runs = 1;
            long bestRun = 0;
            while (runsWithoutBestPicture < 5000)
            {
                long maxX = long.MinValue;
                long maxY = long.MinValue;
                long minX = long.MaxValue;
                long minY = long.MaxValue;

                var xSort = new SortedSet<Vector>(new VectorComparer(true));
                var ySort = new SortedSet<Vector>(new VectorComparer(false));

                foreach (var point in newPoints)
                {
                    var movedPoint = point.Move();
                    xSort.Add(movedPoint);
                    ySort.Add(movedPoint);

                    if (movedPoint.X > maxX) maxX = movedPoint.X;
                    if (movedPoint.X < minX) minX = movedPoint.X;
                    if (movedPoint.Y > maxY) maxY = movedPoint.Y;
                    if (movedPoint.Y < minY) minY = movedPoint.Y;

                }

                if (bestMinY == long.MaxValue || Math.Abs((bestMaxY - bestMinY)) > Math.Abs((maxY - minY)))
                {
                    runsWithoutBestPicture = 0;
                    bestRun = runs;
                    bestPicture = ySort;
                    bestMinX = minX;
                    bestMinY = minY;
                    bestMaxX = maxX;
                    bestMaxY = maxY;
                }

                newPoints = ySort;
                runsWithoutBestPicture++;
                runs++;
            }

            PrintMessage(bestMinX, bestMinY, bestMaxX, bestMaxY, bestPicture, bestRun);
        }

        private static long minYDiff = int.MaxValue;
        private static void PrintMessage(long minX, long minY, long maxX, long maxY, IEnumerable<Vector> picture, long run)
        {
            string fileName = "picture.txt";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Run " + run);
            sb.AppendLine("");
            const long padding = 0;
            for (long y = minY - padding; y <= maxY + padding; y++)
            {
                var yPoints = new Queue<Vector>(picture.SkipWhile(x => x.Y < y).TakeWhile(x => x.Y == y));
                long? nextX = GetNextX(yPoints);

                for (long x = minX - padding; x <= maxX + padding; x++)
                {
                    if (x == nextX)
                    {
                        sb.Append("X");
                        nextX = GetNextX(yPoints);
                        continue;
                    }
                    sb.Append(".");
                }
                sb.Append("\n");
            }

            File.AppendAllText(fileName, sb.ToString());
        }

        private static long? GetNextX(Queue<Vector> yPoints)
        {
            long? nextX = yPoints.Any() ? yPoints.Dequeue()?.X : null;

            if(nextX == null) return null;

            while(yPoints.Any() && yPoints.Peek().X == nextX.Value)
            {
                nextX = yPoints.Dequeue().X;
            }

            return nextX;
        }

        private static void FindOrphans(SortedSet<Vector> sortedVectors, HashSet<Vector> orphans, HashSet<Vector> processed)
        {
            Vector prev = null;
            bool prevWasOrphan = true;

            foreach (Vector current in sortedVectors)
            {
                if (prev == null)
                {
                    prev = current;
                    continue;
                }

                if (prev.ManhattenDistance(current) <= 1)
                {
                    processed.Add(prev);
                    processed.Add(current);
                    orphans.Remove(prev);
                    orphans.Remove(current);

                    prevWasOrphan = false;
                    prev = current;
                    continue;
                }

                if (prevWasOrphan && !processed.Contains(prev)) orphans.Add(prev);

                prevWasOrphan = true;
                prev = current;
            }

            if (prevWasOrphan && !processed.Contains(prev)) orphans.Add(prev);
        }

        private static string[] ParseXY(string coords)
        {
            var start = coords.IndexOf("<") + 1;
            var end = coords.IndexOf(">");

            return coords.Substring(start, end-start).Split(",").Select(x => x.Trim()).ToArray();
        }

        private class VectorComparer : IComparer<Vector>
        {
            private bool _sortByX;
            public VectorComparer(bool sortByX)
            {
                _sortByX = sortByX;
            }
            public int Compare(Vector x, Vector y)
            {
                int firstValue = _sortByX ? x.X.CompareTo(y.X) : x.Y.CompareTo(y.Y);
                int secondValue = _sortByX ? x.Y.CompareTo(y.Y) : x.X.CompareTo(y.X);

                if(firstValue != 0) return firstValue;

                if(secondValue != 0) return secondValue;

                return 1;
            }
        }
    }
}
