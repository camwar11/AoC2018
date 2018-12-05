using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day3
{
    class Program
    {
        const int MIN_SIZE = 1050;
        const int CLAIM_ID = 0;
        const int CLAIM_INFO = 1;
        const int COORDS = 0;
        const int SIZE = 1;
        const int ROW = 1;
        const int COL = 0;
        const int WIDTH = 0;
        const int HEIGHT = 1;

        static void Main(string[] args)
        {
            using(var reader = new InputReader("input.txt"))
            {
                int[,] fabric = new int[MIN_SIZE, MIN_SIZE];

                int overlaps = 0;
                foreach (var claim in reader.GetLines())
                {
                    string[] claimSplit = claim.Split('@');

                    string[] claimInfoSplit = claimSplit[CLAIM_INFO].Split(':');

                    int[] coordsSplit = claimInfoSplit[COORDS].Split(',').Select(x => int.Parse(x)).ToArray();
                    int[] sizeSplit = claimInfoSplit[SIZE].Split('x').Select(x => int.Parse(x)).ToArray();

                    for (int y = 0; y < sizeSplit[HEIGHT]; y++)
                    {
                        for (int x = 0; x < sizeSplit[WIDTH]; x++)
                        {
                            if(++fabric[coordsSplit[ROW] + y, coordsSplit[COL] + x] == 2)
                            {
                                overlaps++;
                            }
                        }
                    }
                }

                //fabric.Display();

                Console.WriteLine("Part1: {0}", overlaps);
            }
        }
    }
}
