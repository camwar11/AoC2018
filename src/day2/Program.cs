using System;
using System.Linq;
using common;

namespace day2
{
    class Program
    {
        static void Main(string[] args)
        {
            using(InputReader reader = new InputReader("input.txt"))
            {
                int twoLetters = 0;
                int threeLetters = 0;

                foreach (string line in reader.GetLines())
                {
                    int[] letters = new int[26];
                    int twoLettersForLine = 0;
                    int threeLettersForLine = 0;
                    foreach(char character in line)
                    {
                        int numberOfChars = ++letters[character%26];
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
                }

                Console.WriteLine("Part1: " + twoLetters * threeLetters);
            }
        }
    }
}
