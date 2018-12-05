using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day2
{
    class Program
    {
        const int CHARS_BEFORE_A = 97;
        const int LETTERS_IN_ALPHABET = 26;
        
        static void Main(string[] args)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            
            using (InputReader reader = new InputReader("input.txt"))
            {
                int twoLetters = 0;
                int threeLetters = 0;

                List<string> prevLines = new List<string>();

                bool foundPart2 = false;
                foreach (string line in reader.GetLines())
                {
                    EvaluatePart1(ref twoLetters, ref threeLetters, line);
                    if(!foundPart2) 
                        foundPart2 = EvaluatePart2(prevLines, line);
                }

                Console.WriteLine("Part1: " + twoLetters * threeLetters);
            }

            Console.WriteLine("Elapsed: {0}", stopwatch.Elapsed);
        }

        private static void EvaluatePart1(ref int twoLetters, ref int threeLetters, string line)
        {
            int[] letters = new int[LETTERS_IN_ALPHABET];

            int twoLettersForLine = 0;
            int threeLettersForLine = 0;
            foreach (char character in line)
            {
                int numberOfChars = ++letters[character - CHARS_BEFORE_A];
                if (numberOfChars == 2)
                {
                    twoLettersForLine++;
                }
                else if (numberOfChars == 3)
                {
                    twoLettersForLine--;
                    threeLettersForLine++;
                }
                else if (numberOfChars > 3)
                {
                    threeLettersForLine--;
                }
            }

            if (twoLettersForLine > 0) twoLetters++;
            if (threeLettersForLine > 0) threeLetters++;
        }

        private static bool EvaluatePart2(List<string> prevLines, string line)
        {
            foreach (string prevLine in prevLines)
            {
                string sameChars = string.Empty;
                int differences = 0;

                for (int i = 0; i < prevLine.Length; i++)
                {
                    if (line[i] == prevLine[i])
                    {
                        sameChars += line[i];
                    }
                    else
                    {
                        if (++differences > 1)
                        {
                            break;
                        }
                    }
                }

                if (differences <= 1)
                {
                    Console.WriteLine("Part2: {0}", sameChars);
                    return true;
                }
            }

            prevLines.Add(line);
            return false;
        }
    }
}
