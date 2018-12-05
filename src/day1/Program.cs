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
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            using (InputReader reader = new InputReader("input.txt"))
            {
                HashSet<int> foundValues = new HashSet<int>();
                int currentValue = 0;
                foundValues.Add(currentValue);
                
                var lines = reader.GetLines().Select(x => int.Parse(x)).ToArray();
                bool found = false;
                bool part1 = false;
                while(!found)
                {
                    foreach(var line in lines)
                    {
                        currentValue += line;
                        
                        if(!found)
                        {
                            if(foundValues.Contains(currentValue))
                            {
                                Console.WriteLine("Part 2: " + currentValue);
                                found = true;
                            }
                            else
                            {
                                foundValues.Add(currentValue);
                            }
                        }
                    }

                    if(!part1)
                    {
                        Console.WriteLine("Part 1: " + currentValue);
                        part1 = true;
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine("Elapsed = {0}", stopwatch.Elapsed);
        }
    }
}
