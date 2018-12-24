using System;
using System.Collections.Generic;
using System.Linq;
using common;
using day16;

namespace day21
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

            var cpu = new CPU(new long[6]);

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

            long cycles = long.MaxValue;
            long smallestCycles = 10000000000;
            int bestRegisterValue = 0;

            for (int regZero = 1000000; regZero >= 0; regZero--)
            {
                cpu = new CPU(new long[]{regZero,0,0,0,0,0}); 
                cpu.PerformInstructionSet(instructions, instructionPointerRegister, out cycles, smallestCycles, false);
                if(cycles <= smallestCycles)
                {
                    smallestCycles = cycles;
                    bestRegisterValue = regZero;
                }
            }

            Console.WriteLine("Part 1: {0}, {1}", bestRegisterValue, smallestCycles);
        }
    }

    internal class TestProgram
    {
        private int reg0, reg1, reg2, reg3, reg5;

        void RealStart()
        {
            reg1 = 0;
            E();
        }

        void End()
        {

        }

        void E()
        {
            reg2 = reg1 | 0b1_0000_0000_0000_0000;
            reg1 = 0b1000_0101_0010_0011_0110_1011;
            D();
        }
        void D()
        {
            reg5 = reg2 & 0b1111_1111;
            reg1 += reg5;
            reg1 = reg1 & 0b1111_1111_1111_1111_1111_1111;
            reg1 = reg1 * 65899;
            reg1 = reg1 & 0b1111_1111_1111_1111_1111_1111;
            // Biggest theoretical reg1 is 16711317
            // but to keep reg2 < 0b1_0000_0000, reg2 and reg5 would have to start at 1111_1111
            // so 
            if(0b1_0000_0000 > reg2)
            {
                reg5 = 1;
                F();
            }
            else
            {
                reg5 = 0;
                A();
            }
        }
        void F()
        {
            if(reg0 == reg1)
            {
                reg5 = 1;
                End();
            }
            else
            {
                reg5 = 0;
                E();
            }
        }
        void A()
        {
            reg5 = 0;
            C();
        }
        void C()
        {
            reg3 = reg5 + 1;
            reg3 *= 256;
            if(reg3 > reg2)
            {
                reg3 = 1;
                G();
            }
            else
            {
                reg3 = 0;
                B();
            }
        }
        void B()
        {
            reg5 += 1;
            C();
        }
        void G()
        {
            reg2 = reg5;
            D();
        }
    }
}
