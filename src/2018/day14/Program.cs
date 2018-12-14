using System;
using System.Collections.Generic;
using System.Linq;

namespace day14
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, string> part1Tests = new Dictionary<int, string>()
            {
                {9, "5158916779"},
                {5, "0124515891"},
                {18, "9251071085"},
                {2018, "5941429882"}
            };

            foreach (var test in part1Tests)
            {
                string result = GetPart1Result(test.Key);

                if(result == test.Value)
                {
                    Console.WriteLine("Pt 1 Test {0} passed.", test.Key);
                }
                else
                {
                    Console.WriteLine("Pt 1 Test {0} failed with value {1} != expected {2}.", test.Key, result, test.Value);
                }
            }

            Console.WriteLine("Part 1: {0}", GetPart1Result(030121));

            Dictionary<string, int> part2Tests = new Dictionary<string, int>()
            {
                {"51589", 9},
                {"01245", 5},
                {"92510", 18},
                {"594142", 2018 }
            };

            foreach (var test in part2Tests)
            {
                int result = GetPart2Result(test.Key);

                if(result == test.Value)
                {
                    Console.WriteLine("Pt 2 Test {0} passed.", test.Key);
                }
                else
                {
                    Console.WriteLine("Pt 2 Test {0} failed with value {1} != expected {2}.", test.Key, result, test.Value);
                }
            }

            Console.WriteLine("Part 2: {0}", GetPart2Result("030121"));
        }

        private static Dictionary<char, int> _charToInt = new Dictionary<char, int>()
        {
            {'0', 0},
            {'1', 1},
            {'2', 2},
            {'3', 3},
            {'4', 4},
            {'5', 5},
            {'6', 6},
            {'7', 7},
            {'8', 8},
            {'9', 9}
        };

        private static string GetPart1Result(int numRecipes)
        {
            List<int> scores = new List<int>(numRecipes + 10){3, 7};

            int elf1Idx = 0;
            int elf2Idx = 1;

            for (int i = 2; i < numRecipes + 10; i++)
            {
                int elf1Recipe = scores[elf1Idx];
                int elf2Recipe = scores[elf2Idx];

                foreach (var score in (elf1Recipe + elf2Recipe).ToString())
                {
                    scores.Add(_charToInt[score]);
                }

                elf1Idx = (elf1Idx + 1 + elf1Recipe) % scores.Count;
                elf2Idx = (elf2Idx + 1 + elf2Recipe) % scores.Count;
            }

            //Console.WriteLine(string.Join("", scores));

            return string.Join("", scores.Skip(numRecipes).Take(10));
        }

        private static int GetPart2Result(string recipeSequence)
        {
            List<int> scores = new List<int>(1000000){3, 7};

            List<int> sequenceToCheck = recipeSequence.Select(x => _charToInt[x]).ToList();
            int sequenceLength = sequenceToCheck.Count;
            int sequenceIdx = 0;

            int elf1Idx = 0;
            int elf2Idx = 1;

            Stack<int> previousScores = new Stack<int>(sequenceToCheck.Count);
            
            int i = 2;
            while (true)
            {
                int elf1Recipe = scores[elf1Idx];
                int elf2Recipe = scores[elf2Idx];

                foreach (var score in (elf1Recipe + elf2Recipe).ToString())
                {
                    i++;
                    var scoreAsInt = _charToInt[score];
                    scores.Add(scoreAsInt);
                    if(FoundSequence(sequenceToCheck, previousScores, ref sequenceIdx, scoreAsInt))
                    {
                        return i - sequenceLength;
                    }
                }

                elf1Idx = (elf1Idx + 1 + elf1Recipe) % scores.Count;
                elf2Idx = (elf2Idx + 1 + elf2Recipe) % scores.Count;
            }
        }

        private static bool FoundSequence(List<int> sequenceToCheck, Stack<int> stack, ref int sequenceIdx, int newNumber)
        {
            if(sequenceToCheck[sequenceIdx++] == newNumber)
            {
                stack.Push(newNumber);
                return sequenceIdx == sequenceToCheck.Count;
            }
            
            sequenceIdx = 0;
            stack.Clear();
            
            if(sequenceToCheck[sequenceIdx] == newNumber)
            {
                sequenceIdx++;
                stack.Push(newNumber);
            }
            
            return false;
        }
    }
}
