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

                HashSet<char> usedChars = new HashSet<char>();
                int smallestPolymer = int.MaxValue; 
                char problemChar = char.MinValue;
                char removedChar = default(char);

                while(true)
                {
                    Stack<char> result = new Stack<char>();
                    foreach (char nextChar in polymer.Where(x => char.ToLower(x) != removedChar))
                    {
                        char prev;
                        if(!result.TryPeek(out prev) || !AreOppositePolarity(prev, nextChar))
                        {
                            result.Push(nextChar);
                            continue;
                        }

                        result.Pop();
                    }

                    if(problemChar == char.MinValue) Console.WriteLine("Part1: {0}", result.Count);

                    if(smallestPolymer > result.Count)
                    {
                        smallestPolymer = result.Count;
                        problemChar = removedChar;
                    }

                    var unusedChar = result.FirstOrDefault(x => !usedChars.Contains(char.ToLower(x)));

                    if(unusedChar == default(char)) break;

                    usedChars.Add(char.ToLower(unusedChar));
                    removedChar = char.ToLower(unusedChar);
                }
                Console.WriteLine("Part2: {0}, {1}", problemChar, smallestPolymer);
            }
        }

        static bool AreOppositePolarity(char one, char two)
        {
            return one != two && char.ToLower(one) == char.ToLower(two);
        }
    }
}
