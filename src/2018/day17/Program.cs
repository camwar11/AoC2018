using System;
using common;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace day17
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines;
            using(var reader = new InputReader("input.txt"))
            {
                lines = reader.GetLines().ToList();
            }

            SortedSet<ScanNode> nodes = new SortedSet<ScanNode>();
            
            int maxX = 0, maxY = 0, minX = int.MaxValue, minY = int.MaxValue;
            foreach (var line in lines)
            {
                var xAndY = line.Split(", ");
                int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                foreach (var xOrY in xAndY)
                {
                    var moreSplitting = xOrY.Split('=');
                    string letter = moreSplitting[0];
                    var numberSplit = moreSplitting[1].Split("..");
                    int firstNumber = int.Parse(numberSplit[0]);
                    int secondNumber = numberSplit.Length > 1 ? int.Parse(numberSplit[1]) : firstNumber;

                    if(letter == "x")
                    {
                        x1 = firstNumber;
                        x2 = secondNumber;
                    }
                    else
                    {
                        y1 = firstNumber;
                        y2 = secondNumber;
                    }
                }
                for (int x = x1; x <= x2; x++)
                {
                    for (int y = y1; y <= y2; y++)
                    {
                        if(x > maxX) maxX = x;
                        if(x < minX) minX = x;
                        if(y > maxY) maxY = y;
                        if(y < minY) minY = y;

                        nodes.Add(new ScanNode(x, y, ScanNode.CLAY));
                    }
                }
            }
            // Enlaging Xs by 1 should be enough to count everything.
            maxX++;
            minX--;
            Ground grid = new Ground(maxX, maxY+1, (long x, long y) => new ScanNode(x, y, ScanNode.SAND), minX, minY);

            foreach (ScanNode node in nodes)
            {
                grid[node.X, node.Y] = node;
            }

            long nonFlowingCount;

            Console.WriteLine("Part 1: " + RunSim(grid, 500, true, out nonFlowingCount));
            Console.WriteLine("Part 2: " + nonFlowingCount);
        }

        private static long RunSim(Ground ground, int waterXCoord, bool print, out long nonFlowingCount)
        {
            ground.AddWater(waterXCoord);
            return ground.WaterCount(print, out nonFlowingCount);
        }

        private class ScanNode : Point
        {
            public const char CLAY = '#';
            public const char SAND = '.';
            public const char WATER = '+';
            public const char FLOWING_WATER = '|';


            public char GroundType { get; set; }
            public ScanNode(long x, long y, char type) : base(x, y)
            {
                GroundType = type;
            }

            public bool IsSolid 
            { 
                get
                {
                    return GroundType == CLAY || GroundType == WATER;
                }
            }
        }

        private class Ground : Grid<ScanNode>
        {
            public Ground(long width, long height, EmptyGenerator emptyGen, long minX = 0, long minY = 0) : base(width, height, emptyGen, minX, minY)
            {
            }

            internal long WaterCount(bool print, out long nonFlowingCount)
            {
                long count = 0;
                nonFlowingCount = 0;
                StringBuilder builder = new StringBuilder();
                for (long y= _minY; y < _height; y++)
                {
                    for (long x = _minX; x < _width; x++)
                    {
                        var node = this[x, y];
                        if(node.GroundType == ScanNode.FLOWING_WATER || 
                           node.GroundType == ScanNode.WATER)
                        {
                            if(node.GroundType == ScanNode.WATER) nonFlowingCount++;

                            count++;   
                        }
                        //if(print) Console.Write(node.GroundType);
                        if(print) builder.Append(node.GroundType);
                    }   
                    //if(print) Console.WriteLine();
                    if(print) builder.AppendLine();
                }

                if(print) File.WriteAllText("output.txt", builder.ToString());

                return count;
            }

            internal void AddWater(long waterXCoord)
            {
                long x = waterXCoord;
                long y = _minY;
                DropWater(x, y);
            }

            private bool DropWater(long x, long y)
            {
                this[x,y].GroundType = ScanNode.FLOWING_WATER;

                if(y+1 == _height)
                {
                    return false;
                }

                var nextSquare = this[x, y+1];
                if(nextSquare.GroundType == ScanNode.FLOWING_WATER) return false;
                if(!nextSquare.IsSolid)
                {
                    if(!DropWater(nextSquare.X, nextSquare.Y))
                    {
                        return false;
                    }
                }

                List<ScanNode> leftNodes = new List<ScanNode>();
                List<ScanNode> rightNodes = new List<ScanNode>();
                bool wallLeft = HasWall(x, y, LEFT, leftNodes);
                bool wallRight = HasWall(x, y, RIGHT, rightNodes);

                if(wallLeft && wallRight)
                {  
                    this[x,y].GroundType = ScanNode.WATER;
                    foreach (var node in leftNodes.Concat(rightNodes))
                    {
                        node.GroundType = ScanNode.WATER;
                    }

                    return true;
                }
                else if(wallLeft)
                {
                    foreach (var node in leftNodes.Concat(rightNodes))
                    {
                        node.GroundType = ScanNode.FLOWING_WATER;
                    }

                    var rightMostNode = rightNodes.LastOrDefault();
                    if(rightMostNode == null) return false;
                    return DropWater(rightMostNode.X, rightMostNode.Y);
                }
                else if(wallRight)
                {
                    foreach (var node in leftNodes.Concat(rightNodes))
                    {
                        node.GroundType = ScanNode.FLOWING_WATER;
                    }

                    var leftMostNode = leftNodes.LastOrDefault();
                    if(leftMostNode == null) return false;
                    return DropWater(leftMostNode.X, leftMostNode.Y);
                }
                else
                {
                    foreach (var node in leftNodes.Concat(rightNodes))
                    {
                        node.GroundType = ScanNode.FLOWING_WATER;
                    }

                    var rightMostNode = rightNodes.LastOrDefault();
                    var leftMostNode = leftNodes.LastOrDefault();
                    bool returnVal = false;
                    if(leftMostNode != null)
                    {
                        returnVal |= DropWater(leftMostNode.X, leftMostNode.Y);
                    }
                    if(rightMostNode != null)
                    {
                        returnVal |= DropWater(rightMostNode.X, rightMostNode.Y);
                    }
                    return returnVal;
                }
            }

            private bool IsSolidUnderneath(long x, long y)
            {
                if(y+1 == _height)
                {
                    return false;
                }

                return this[x, y+1].IsSolid;
            }

            private long LEFT = -1;
            private long RIGHT = 1;
            private bool HasWall(long x, long y, long changeX, List<ScanNode> nodes)
            {
                if(!IsSolidUnderneath(x, y))
                {
                    return false;
                }

                long newX = x + changeX;
                if(newX < _minX || newX > _width)
                {
                    return false;
                }

                ScanNode nextNode = this[newX, y];
                if(nextNode.GroundType == ScanNode.CLAY)
                {
                    return true;
                }

                nodes.Add(nextNode);

                return HasWall(newX, y, changeX, nodes);
            }
        }
    }
}
