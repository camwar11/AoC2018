using System;
using common;

namespace day11
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTests();

            string input;
            using(var reader = new InputReader("input.txt"))
            {
                input = reader.GetNextLine();
            }

            int serialNum = int.Parse(input);
            int size = 300;
            int partOneSize = 3;

            FuelCell[,] fuelCells = new FuelCell[size,size]; 

            var builder = new System.Text.StringBuilder();
            for (int x = 1; x <= size; x++)
            {
                for (int y = 1; y <= size; y++)
                {
                    fuelCells[x - 1,y - 1] = new FuelCell(x, y, serialNum);
                }
            }

            long mostPower = 0;
            FuelCell mostPowerCell = null;
            int mostPowerSquareSize = 0;
            long part1MostPower = 0;
            FuelCell part1MostPowerCell = null;

            for(int square = 1; square <= size; square++)
            {
                for (int x = 0; x <= size - square; x++)
                {
                    for (int y = 0; y <= size - square; y++)
                    {
                        long totalPower = 0;
                        for (int i = 0; i < square; i++)
                        {
                            for (int j = 0; j < square; j++)
                            {
                                totalPower += fuelCells[x + i, y + j].Power();
                            }
                        }

                        if(square == partOneSize && totalPower > part1MostPower)
                        {
                            part1MostPower = totalPower;
                            part1MostPowerCell = fuelCells[x,y];
                        }

                        if(totalPower > mostPower)
                        {
                            mostPower = totalPower;
                            mostPowerCell = fuelCells[x, y];
                            mostPowerSquareSize = square;
                        }           
                    }
                }
            }

            Console.WriteLine("Part 1: {0},{1}", part1MostPowerCell.X, part1MostPowerCell.Y);
            Console.WriteLine("Part 2: {0},{1},{2}", mostPowerCell.X, mostPowerCell.Y, mostPowerSquareSize);
        }

        static void RunTests()
        {
            if(new FuelCell(3,5,8).Power() != 4) throw new Exception("Test 1 failed.");
            if(new FuelCell(122,79,57).Power() != -5) throw new Exception("Test 2 failed.");
            if(new FuelCell(217,196,39).Power() != 0) throw new Exception("Test 3 failed.");
            if(new FuelCell(101,153,71).Power() != 4) throw new Exception("Test 4 failed.");
        }
    }

    class FuelCell : Point
    {
        private int _serialNum;
        public FuelCell(long x, long y, int serialNum) : base(x, y)
        {
            _serialNum = serialNum;
        }
        private long _power = long.MinValue;

        public long Power()
        {
            if(_power != long.MinValue) return _power;

            long power = RackId() * Y;

            power += _serialNum;
            power *= RackId();
            var powString = power.ToString();
            power = powString.Length < 3 ? 0 : int.Parse(powString.Substring(powString.Length - 3, 1));
            power -= 5;
            _power = power;

            return _power;
        }

        public long RackId()
        {
            return X + 10;
        }

    }
}
