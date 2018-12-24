using System;
using System.Collections.Generic;
using System.Linq;
using common;
using day16;

namespace day19
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = new List<string>();
            using(var reader = new InputReader("input.txt"))
            {
                lines = reader.GetLines().ToList();
            }

            var cpu = new CPU(new int[6]);

            int instructionPointerRegister = 0;
            bool first = true;
            List<CPU.Instruction> instructions = new List<CPU.Instruction>();
            foreach (string line in lines)
            {
                if(first)
                {
                    first = false;
                    instructionPointerRegister = int.Parse(line.Substring(4,1));
                    continue;
                }

                var split = line.Split(" ");
                var instruction = new CPU.Instruction()
                {
                    OpCode = Enum.Parse<CPU.OpCode>(split[0]),
                    InputA = int.Parse(split[1]),
                    InputB = int.Parse(split[2]),
                    OutputC = int.Parse(split[3]),
                };

                instructions.Add(instruction);
            }

            long cycles;
            var result = cpu.PerformInstructionSet(instructions, instructionPointerRegister, out cycles);

            Console.WriteLine("Part 1: " + result[0]);

            cpu = new CPU(new int[]{1,0,0,0,0,0});
            
            result = cpu.PerformInstructionSet(instructions, instructionPointerRegister, out cycles);

            // After letting this run for an hour, I watched it and realized it was trying
            // to do a sum of the factors of the big number, so I put it in Wolfram Alpha and got 27578880
            Console.WriteLine("Part 2: " + result[0]);


        }
    }
}
