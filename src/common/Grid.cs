using System;
using System.Collections.Generic;
using System.Linq;

namespace common
{
    public class Grid<T> where T : Point
    {
        private T[,] _board;
        protected long _width, _height;

        public delegate T EmptyGenerator(long x, long y);

        protected EmptyGenerator _emptyGenerator;

        public Grid(long width, long height, EmptyGenerator emptyGen = null)
        {
            _board = new T[width, height];
            _width = width;
            _height = height;

            _emptyGenerator = emptyGen;
        }

        public void Print()
        {
            for (long y = 0; y < _height; y++)
            {
                for (long x = 0; x < _width; x++)
                {
                    Console.Write(_board[x,y].ToString());
                }
                Console.Write("\n");
            }
        }

        public virtual T this[long x, long y]
        {
            get
            {
                return _board[x, y];
            }
            set 
            {
                _board[value.X, value.Y] = _emptyGenerator(value.X, value.Y);
                _board[x, y] = value;
            }
        }

        public IEnumerable<T> GetAdjacentSquares(T point)
        {
            return GetAdjacentSquares(_board, point.X, point.Y);
        }
        
        private static IEnumerable<V> GetAdjacentSquares<V>(V[,] grid, long x, long y)
        {
            List<V> adjacents = new List<V>(4);
            
            if(y != 0)
            {
                var up = grid[x, y-1];
                adjacents.Add(up);
            }            

            if(x != 0)
            {
                var left = grid[x - 1, y];
                adjacents.Add(left);
            }
            
            if(grid.GetLength(0) > x + 1)
            {
                var right = grid[x + 1, y];
                adjacents.Add(right);
            }
            
            if(grid.GetLength(1) > y + 1)
            {
                var down = grid[x, y + 1];
                adjacents.Add(down);
            }

            return adjacents;
        }

        public IEnumerable<ShortestPathResult> GetShortestPaths(T start, params T[] endingPoints)
        {
            return DijkstrasAlgorithm(start, endingPoints);
        }

        public long[,,,] GetAllShortestPathLengths()
        {
            return FloydWarshallAlgorithm();
        }

        private long[,,,] FloydWarshallAlgorithm()
        {
            long[,,,] distances = InitializeFWAlgorithm();

            for (long kX = 0; kX < _width; kX++)
            {
                for (long kY = 0; kY < _height; kY++)
                {
                    for (long iX = 0; iX < _width; iX++)
                    {
                        for (long iY = 0; iY < _height; iY++)
                        {
                            for (long jX = 0; jX < _width; jX++)
                            {
                                for (long jY = 0; jY < _height; jY++)
                                {
                                    long distIToK = distances[iX,iY,kX,kY];
                                    long distKToJ = distances[kX,kY,jX,jY];

                                    // Don't evaluate "infinity" or we will overflow the long
                                    if(long.MaxValue == distIToK || long.MaxValue == distKToJ) continue;

                                    long distThruK = distIToK + distKToJ; 
                                    if(distances[iX,iY,jX,jY] > distThruK)
                                    {
                                        distances[iX,iY,jX,jY] = distThruK;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return distances;
        }

        private long[,,,] InitializeFWAlgorithm()
        {
            long[,,,] distances = new long[_width, _height, _width, _height];
            for (long x1 = 0; x1 < _width; x1++)
            {
                for (long y1 = 0; y1 < _height; y1++)
                {
                    for (long x2 = 0; x2 < _width; x2++)
                    {
                        for (long y2 = 0; y2 < _height; y2++)
                        {
                            long startingDistance = long.MaxValue;
                            if (x1 == x2 && y1 == y2) startingDistance = 0;
                            if (Point.ManhattenDistance(x1, x2, y1, y2) == 1)
                            {
                                startingDistance = this[x2, y2].Weight;
                            }
                            distances[x1, y1, x2, y2] = startingDistance;
                        }
                    }
                }
            }

            return distances;
        }

        public class ShortestPathResult
        {
            public long Distance { get; private set; }
            public IEnumerable<IEnumerable<T>> Paths {get; private set;}

            public T Start { get; private set; }
            public T End { get; private set; }

            internal ShortestPathResult(long distance, IEnumerable<IEnumerable<T>> paths, T start, T end)
            {
                Distance = distance;
                Paths = paths;
                Start = start;
                End = end;
            }
        }

        private List<ShortestPathResult> DijkstrasAlgorithm(T start, IEnumerable<T> ends)
        {
            PathInfo[,] pathInfos = new PathInfo[_width, _height];
            List<PathInfo> endsPaths = new List<PathInfo>(ends.Count());
            SortedSet<PathInfo> unvisitedNodes = new SortedSet<PathInfo>(new DistanceSorter(ends.FirstOrDefault()));
            for (long y = 0; y < _height; y++)
            {
                for (long x = 0; x < _width; x++)
                {
                    bool isSource = start.X == x && start.Y == y;
                    bool isDestination = ends.Any(b => b.X == x && b.Y == y);
                    long distance = isSource ? 0 : long.MaxValue;
                    
                    var pathInfo = new PathInfo()
                    {
                        Distance = distance,
                        Node = _board[x,y]
                    };
                    if(isDestination) endsPaths.Add(pathInfo);
                    
                    pathInfos[x, y] = pathInfo;
                    unvisitedNodes.Add(pathInfo);
                }
            }

            while (unvisitedNodes.Count > 0)
            {
                var current = unvisitedNodes.Min;
                unvisitedNodes.Remove(current);
                current.Visited = true;

                if(endsPaths.All(x => x.Visited) || current.Distance == long.MaxValue) break;

                foreach (var neighbor in GetAdjacentSquares(pathInfos, current.Node.X, current.Node.Y))
                {
                    // Unreachable node, so remove it.
                    if(neighbor.Node.Weight == long.MaxValue)
                    {
                        unvisitedNodes.Remove(neighbor);
                        neighbor.Visited = true;
                        continue;
                    }

                    long distance = current.Distance + neighbor.Node.Weight;
                    if(distance == neighbor.Distance)
                    {
                        neighbor.Prevs.Add(current);
                    }
                    if(distance < neighbor.Distance)
                    {
                        if(unvisitedNodes.Remove(neighbor))
                        {
                            // Need to remove and add it so that the sorting updates
                            neighbor.Distance = distance;
                            unvisitedNodes.Add(neighbor);
                        }
                        else
                        {
                            neighbor.Distance = distance;
                        }

                        neighbor.Prevs.Clear();
                        neighbor.Prevs.Add(current);
                    }
                }
            }

            List<ShortestPathResult> shortestPaths = new List<ShortestPathResult>();

            foreach (var ending in endsPaths)
            {
                if(!ending.Visited) continue;

                PathInfo currentNode = ending;
                List<Stack<T>> currentShortestPaths = new List<Stack<T>>();

                FollowPath(currentNode, currentShortestPaths);

                ShortestPathResult result = new ShortestPathResult(ending.Distance, currentShortestPaths,start, ending.Node);

                shortestPaths.Add(result);
            }

            return shortestPaths;
        }

        private void FollowPath(PathInfo current, List<Stack<T>> prevPaths, Stack<T> currentPath = null)
        {
            if(!prevPaths.Any())
            {
                prevPaths.Add(new Stack<T>());
            }

            if(currentPath == null) currentPath = prevPaths.First();

            currentPath.Push(current.Node);

            bool firstPrev = true;
            foreach (var prev in current.Prevs)
            {
                if(firstPrev)
                {
                    firstPrev = false;
                }
                else
                {
                    currentPath = new Stack<T>(currentPath.Reverse());
                    prevPaths.Add(currentPath);
                }
                
                FollowPath(prev, prevPaths, currentPath);
            }    
        }

        private class PathInfo
        {
            public long Distance { get; set; }
            public T Node { get; set; }

            public List<PathInfo> Prevs {get; set;} = new List<PathInfo>();

            public bool Visited {get; set;}
        }

        private class DistanceSorter : IComparer<PathInfo>
        {
            private T _endPoint;
            public DistanceSorter(T endPoint = null)
            {
                _endPoint = endPoint;
            }
            public int Compare(PathInfo x, PathInfo y)
            {
                int comparision = x.Distance.CompareTo(y.Distance);

                if(comparision != 0) return comparision;

                if(_endPoint != null)
                {
                    comparision = x.Node.ManhattenDistance(_endPoint).CompareTo(y.Node.ManhattenDistance(_endPoint));
                    if(comparision != 0) return comparision;
                }

                return x.Node.CompareTo(y.Node);
            }
        }
    }
}