using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day5
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var reader = new InputReader("input.txt"))
            {
                string polymer = reader.GetNextLine();

                Stack<char> result = new Stack<char>();

                foreach (char nextChar in polymer)
                {
                    char prev;
                    if(!result.TryPeek(out prev) || !AreOppositePolarity(prev, nextChar))
                    {
                        result.Push(nextChar);
                        continue;
                    }

                    result.Pop();
                }

                Console.WriteLine("Part1: {0}", result.Count);
            }
        }

        static bool AreOppositePolarity(char one, char two)
        {
            return one != two && char.ToLower(one) == char.ToLower(two);
        }
    }
}
