using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day1
{
    class Program
    {
        static void Main(string[] args)
        {
            using(InputReader reader = new InputReader("input.txt"))
            {
                var lines = reader.GetLines().Select(x => int.Parse(x)).ToArray();
                Console.WriteLine("Part 1: " + lines.Sum());

                HashSet<int> foundValues = new HashSet<int>();
                int currentValue = 0;
                foundValues.Add(currentValue);

                while(true)
                {
                    if(lines.Any((x => {
                        currentValue += x;
                        bool found = foundValues.Contains(currentValue);
                        foundValues.Add(currentValue);
                        return found;
                    })))
                    {
                        Console.WriteLine("Part 2: " + currentValue);
                        break;
                    }
                }
            }
        }
    }
}
