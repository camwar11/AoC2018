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
                string[,] fabric = new string[MIN_SIZE, MIN_SIZE];

                int overlaps = 0;
                HashSet<string> claimsWithNoOverlaps = new HashSet<string>();
                foreach (var claim in reader.GetLines())
                {
                    string[] claimSplit = claim.Split('@');

                    string[] claimInfoSplit = claimSplit[CLAIM_INFO].Split(':');
                    string claimId = claimSplit[CLAIM_ID];

                    int[] coordsSplit = claimInfoSplit[COORDS].Split(',').Select(x => int.Parse(x)).ToArray();
                    int[] sizeSplit = claimInfoSplit[SIZE].Split('x').Select(x => int.Parse(x)).ToArray();

                    int currentOverlaps = overlaps;
                    for (int y = 0; y < sizeSplit[HEIGHT]; y++)
                    {
                        for (int x = 0; x < sizeSplit[WIDTH]; x++)
                        {
                            string overlappingClaim = fabric[coordsSplit[ROW] + y, coordsSplit[COL] + x];
                            if(overlappingClaim == null)
                            {
                                fabric[coordsSplit[ROW] + y, coordsSplit[COL] + x] = claimId;
                            }
                            else
                            {
                                claimsWithNoOverlaps.Remove(overlappingClaim);
                                overlaps++;
                            }
                        }
                    }

                    if(currentOverlaps == overlaps) claimsWithNoOverlaps.Add(claimId);
                }

                //fabric.Display();

                Console.WriteLine("Part1: {0}", overlaps);
                Console.WriteLine("Part2: {0}", claimsWithNoOverlaps.FirstOrDefault());
            }
        }
    }
}
