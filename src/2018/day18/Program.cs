using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using common;

namespace day18
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines;
            string inputFile = "input.txt";
            int size = inputFile == "test.txt" ? 10 : 50;
            bool print = false;

            using(var reader = new InputReader(inputFile))
            {
                lines = reader.GetLines().ToList();
            }

            Forest forest = new Forest(size, size);
            int x = 0;
            int y = 0;
            foreach (var line in lines)
            {
                foreach (var acre in line)
                {
                    forest[x,y] = new Acre(x, y, acre);
                    x++;
                }
                y++;
                x = 0;
            }

            long lumberYards = 0;
            long trees = 0;

            for (int minute = 1; minute <= 1000000000; minute++)
            {
                lumberYards = 0;
                trees = 0;
                
                if(print)
                {
                    File.AppendAllText("output.txt", string.Format("\n\nForest minute {0}\n{1}", minute, forest.Print(false)));
                }

                foreach (var acre in forest.Points())
                {
                    acre.RunTurn(forest.GetAdjacentSquares(acre, true));
                }

                foreach (var acre in forest.Points())
                {
                    acre.Generate();
                    if(acre.GroundType == Acre.LUMBERYARD) lumberYards++;
                    else if(acre.GroundType == Acre.TREES) trees++;
                }

                if(minute == 10)
                {
                    Console.WriteLine("Part 1: {0}x{1}={2}", lumberYards, trees, lumberYards * trees);
                }
                // Found part 2 by just outputting the values and looking for repetition, then
                // figuring out what the value would be at 1000000000
                File.AppendAllText("output.txt", lumberYards * trees + "\n");
            }

            Console.WriteLine("Part 2: {0}x{1}={2}", lumberYards, trees, lumberYards * trees);
        }

        private class Forest : Grid<Acre>
        {
            public Forest(long width, long height) : base(width, height, null, 0, 0)
            {
            }
        }

        private class Acre : Point
        {
            public const char OPEN = '.';
            public const char TREES = '|';
            public const char LUMBERYARD = '#';


            public char GroundType {get; set;}
            private char _nextGroundType;
            public Acre(long x, long y, char groundType) : base(x, y)
            {
                GroundType = groundType;
            }

            public void RunTurn(IEnumerable<Acre> adjacentAcres)
            {
                switch (GroundType)
                {
                    case TREES:
                    _nextGroundType = adjacentAcres.Count(x => x.GroundType == LUMBERYARD) >= 3 ? LUMBERYARD : GroundType;
                    break;
                    case OPEN:
                    _nextGroundType = adjacentAcres.Count(x => x.GroundType == TREES) >= 3 ? TREES : GroundType;
                    break;
                    case LUMBERYARD:
                    _nextGroundType = adjacentAcres.Any(x => x.GroundType == TREES) &&
                        adjacentAcres.Any(x => x.GroundType == LUMBERYARD) ? LUMBERYARD : OPEN;
                    break;
                }
            }

            public void Generate()
            {
                GroundType = _nextGroundType;
            }

            public override string ToString()
            {
                return ""+GroundType;
            }
        }
    }
}
