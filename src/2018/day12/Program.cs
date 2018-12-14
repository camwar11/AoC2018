using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day12
{
    class Program
    {
        static void Main(string[] args)
        {
            LinkedList<Pot> pots = new LinkedList<Pot>();
            Dictionary<string, char> growChart = new Dictionary<string, char>();  
            string initStateId = "initial state: ";
            using(var reader = new InputReader("input.txt"))
            {
                foreach (string line in reader.GetLines())
                {
                    if(line.StartsWith(initStateId))
                    {
                        int id = 0;
                        foreach (char initPot in line.Substring(initStateId.Length))
                        {
                            pots.AddLast(new Pot(initPot, id++));
                        }
                    }
                    else if(string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    else
                    {
                        var split = line.Split("=>").Select(x => x.Trim()).ToArray();

                        growChart.Add(split[0], split[1][0]);
                    }
                }
            }
                
            string generationLine = string.Empty;
            long numForLine = 0;
            long prevNumForLine = 0;


            for (int generation = 0; generation <= 1000; generation++)
            {
                LinkedListNode<Pot> currentPot = pots.First;

                generationLine = string.Empty;
                prevNumForLine = numForLine;
                numForLine = 0;

                while (currentPot != null)
                {
                    currentPot.Value.GrowNextGen();
                    generationLine += currentPot.Value.CurrentState;
                    numForLine += currentPot.Value.CurrentState == '#' ? currentPot.Value.Id : 0;
                    currentPot = currentPot.Next;
                }
                Console.WriteLine("{0}:{1}: {2}", generation, numForLine - prevNumForLine, generationLine.Trim('.'));

                if(generation == 20) Console.WriteLine("Part 1: {0}", numForLine);

                currentPot = pots.First;

                Pot.GetOrAddPrevious(pots, Pot.GetOrAddPrevious(pots, currentPot));
                
                bool added;
                var lastPot = pots.Last;
                Pot.GetOrAddNext(pots, Pot.GetOrAddNext(pots, lastPot, out added), out added);

                int numCreated = 0;

                currentPot = pots.First;

                while (currentPot != null)
                {
                    if(currentPot.Value.SetNextGeneration(pots, currentPot, growChart))
                    {
                        numCreated++;
                        if(numCreated >= 3) break;
                    }
                    currentPot = currentPot.Next;
                }

                
            }

            long diff =  numForLine - prevNumForLine;
            long fiveBill = (50000000000 - 1000) * diff + numForLine;
            Console.WriteLine("Part 2: Diff per run = {0}. Extrapolate to 5bill: {1}.", diff, fiveBill);
        }

        private class Pot
        {
            private const char PLANT = '#';
            private const char EMPTY = '.';

            public int Id;

            internal char CurrentState {get;set;}
            internal char? NextGen {get;set;}

            public Pot(char initState, int id)
            {
                CurrentState = initState;
                Id = id;
                NextGen = null;
            }

            internal bool SetNextGeneration(LinkedList<Pot> list, LinkedListNode<Pot> thisPot, Dictionary<string, char> growLog)
            {
                string potFrame = string.Empty;
                
                var prev = GetOrAddPrevious(list, thisPot);
                var prevPrev = GetOrAddPrevious(list, prev);

                bool created = false;

                var next = GetOrAddNext(list, thisPot, out created);
                var nextNext = GetOrAddNext(list, next, out created);

                potFrame = potFrame + prevPrev.Value.CurrentState + prev.Value.CurrentState + this.CurrentState +
                next.Value.CurrentState + next.Next.Value.CurrentState;

                char nextGenFromLog;

                if(!growLog.TryGetValue(potFrame, out nextGenFromLog))
                {
                    NextGen = EMPTY;
                }
                else
                {
                    NextGen = nextGenFromLog;
                }

                return created;
            }

            internal void GrowNextGen()
            {
                CurrentState = NextGen.HasValue ? NextGen.Value : CurrentState;
                NextGen = null;
            }

            internal static LinkedListNode<Pot> GetOrAddPrevious(LinkedList<Pot> list, LinkedListNode<Pot> thisPot)
            {
                if(thisPot.Previous == null)
                {
                    list.AddBefore(thisPot, new Pot(EMPTY, thisPot.Value.Id - 1));
                }

                return thisPot.Previous;
            }

            internal static LinkedListNode<Pot> GetOrAddNext(LinkedList<Pot> list, LinkedListNode<Pot> thisPot, out bool created)
            {
                if(thisPot.Next == null)
                {
                    list.AddAfter(thisPot, new Pot(EMPTY, thisPot.Value.Id + 1));
                    created = true;
                }
                else
                {
                    created = false;
                }

                return thisPot.Next;
            }
        }
    }
}
