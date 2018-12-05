using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            const int CHARS_BEFORE_A = 97;
            const int LETTERS_IN_ALPHABET = 26;
            using (InputReader reader = new InputReader("input.txt"))
            {
                int twoLetters = 0;
                int threeLetters = 0;

                List<string> prevLines = new List<string>();

                foreach (string line in reader.GetLines())
                {
                    int[] letters = new int[LETTERS_IN_ALPHABET];
                    
                    int twoLettersForLine = 0;
                    int threeLettersForLine = 0;
                    foreach(char character in line)
                    {
                        int numberOfChars = ++letters[character - CHARS_BEFORE_A];
                        if(numberOfChars == 2)
                        {
                            twoLettersForLine++;
                        }
                        else if(numberOfChars == 3)
                        {
                            twoLettersForLine--;
                            threeLettersForLine++;
                        }
                        else if(numberOfChars > 3)
                        {
                            threeLettersForLine--;
                        }
                    }

                    if(twoLettersForLine > 0) twoLetters++;
                    if(threeLettersForLine > 0) threeLetters++;

                    foreach (string prevLine in prevLines)
                    {
                        string sameChars = string.Empty;
                        int differences = 0;

                        for (int i = 0; i < prevLine.Length; i++)
                        {                            
                            if(line[i] == prevLine[i])
                            {
                                sameChars += line[i];
                            }
                            else
                            {
                                if(++differences > 1)
                                {
                                    break;
                                }
                            }
                        }

                        if(differences <= 1)
                        {
                            Console.WriteLine("Part2: {0}", sameChars);
                            break;
                        }
                    }

                    prevLines.Add(line);
                }

                Console.WriteLine("Part1: " + twoLetters * threeLetters);
            }

            Console.WriteLine("Elapsed: {0}", stopwatch.Elapsed);
        }
    }
}
