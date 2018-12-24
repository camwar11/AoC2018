using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day16
{
    class Program
    {
        static void Main(string[] args)
        {

            var test = new List<string>()
            {
                "Before: [3, 2, 1, 1]",
                "9 2 1 2",
                "After:  [3, 2, 2, 1]"
            };

            int testResult = RunCPU(test, false).Item1;

            if(testResult == 1)
            {
                Console.WriteLine("Part 1 test passes.");
            }
            else
            {
                Console.WriteLine("Part 1 test failed " + testResult);
            }

            List<string> lines;
            using (var reader = new InputReader("input.txt"))
            {
                lines = reader.GetLines().ToList();
            }

            var result = RunCPU(lines, true);
            Console.WriteLine("Part 1 Result = {0}", result.Item1);
            Console.WriteLine("Part 2 Result = {0}", result.Item2);
        }

        private static Tuple<int,long> RunCPU(List<string> lines, bool runPart2)
        {
            int linesLeftInSet = 3;
            List<long[]> linesInSet = new List<long[]>();
            var cpu = new CPU();
            int threshHold = 3;
            int part1Results = 0;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    linesInSet.Clear();
                    continue;
                }

                if (line.StartsWith("Before:"))
                {
                    linesLeftInSet = 3;
                    linesInSet.Add(ParseRegisters(line));
                }
                else if (line.StartsWith("After:"))
                {
                    linesLeftInSet = 0;
                    linesInSet.Add(ParseRegisters(line));

                    if (cpu.Part1(linesInSet) >= threshHold) part1Results++;
                }
                else if (linesLeftInSet > 0)
                {
                    linesInSet.Add(ParseInstruction(line));
                }
                else
                {
                    linesInSet.Add(ParseInstruction(line));
                }
            }

            long part2 = !runPart2 ? 0 : cpu.Part2(linesInSet);

            return new Tuple<int, long>(part1Results, part2);
        }

        private static long[] ParseInstruction(string line)
        {
            return line.Split(' ').Select(x => long.Parse(x)).ToArray();
        }

        private static long[] ParseRegisters(string line)
        {
            int startIdx = line.IndexOf('[') + 1;
            int endIdx = line.IndexOf(']', startIdx);
            return line.Substring(startIdx, endIdx-startIdx).Split(", ").Select(x => long.Parse(x)).ToArray();
        }

        
    }

    public class CPU
        {
            public enum OpCode
            {
                addr,
                addi,
                mulr,
                muli,
                banr,
                bani,
                borr,
                bori,
                setr,
                seti,
                gtir,
                gtri,
                gtrr,
                eqir,
                eqri,
                eqrr
            }

            private delegate long GetValue(long input, long[] registers);

            private static GetValue GetImmediate = (long input, long[] registers) =>
            {
                return input;
            };

            private static GetValue GetRegister = (long input, long[] registers) =>
            {
                return registers[input];
            };

            private static GetValue[] ImmediateImmediate = new GetValue[]{GetImmediate, GetImmediate};
            private static GetValue[] ImmediateRegister = new GetValue[]{GetImmediate, GetRegister};
            private static GetValue[] RegisterImmediate = new GetValue[]{GetRegister, GetImmediate};
            private static GetValue[] RegisterRegister = new GetValue[]{GetRegister, GetRegister};

            private static Dictionary<OpCode, GetValue[]> _opcodeValueGetters = new Dictionary<OpCode, GetValue[]>()
            {
                {OpCode.addi, RegisterImmediate},
                {OpCode.addr, RegisterRegister},
                {OpCode.bani, RegisterImmediate},
                {OpCode.banr, RegisterRegister},
                {OpCode.bori, RegisterImmediate},
                {OpCode.borr, RegisterRegister},
                {OpCode.eqir, ImmediateRegister},
                {OpCode.eqri, RegisterImmediate},
                {OpCode.eqrr, RegisterRegister},
                {OpCode.gtir, ImmediateRegister},
                {OpCode.gtri, RegisterImmediate},
                {OpCode.gtrr, RegisterRegister},
                {OpCode.muli, RegisterImmediate},
                {OpCode.mulr, RegisterRegister},
                {OpCode.seti, ImmediateImmediate},
                {OpCode.setr, RegisterRegister}
            };

            private delegate long RunOp(long inputA, long inputB);
            private static long Add(long inputA, long inputB)
            {
                return inputA + inputB;
            }
            private static long And(long inputA, long inputB)
            {
                return inputA & inputB;
            }
            private static long Or(long inputA, long inputB)
            {
                return inputA | inputB;
            }
            private static long Assignment(long inputA, long inputB)
            {
                return inputA;
            }
            private static long Multiply(long inputA, long inputB)
            {
                return inputA * inputB;
            }
            private static long Equal(long inputA, long inputB)
            {
                if(inputA == inputB) return 1;
                return 0;
            }
            private static long GreaterThan(long inputA, long inputB)
            {
                if(inputA > inputB) return 1;
                return 0;
            }

            private static Dictionary<OpCode, RunOp> _opcodePerformOp = new Dictionary<OpCode, RunOp>()
            {
                {OpCode.addi, Add},
                {OpCode.addr, Add},
                {OpCode.bani, And},
                {OpCode.banr, And},
                {OpCode.bori, Or},
                {OpCode.borr, Or},
                {OpCode.eqir, Equal},
                {OpCode.eqri, Equal},
                {OpCode.eqrr, Equal},
                {OpCode.gtir, GreaterThan},
                {OpCode.gtri, GreaterThan},
                {OpCode.gtrr, GreaterThan},
                {OpCode.muli, Multiply},
                {OpCode.mulr, Multiply},
                {OpCode.seti, Assignment},
                {OpCode.setr, Assignment}
            };

            private long[] _registers;
            private Dictionary<int, HashSet<OpCode>> _opCodeMap;

            public CPU(long[] initialRegisters = null)
            {
                if(initialRegisters == null)
                {
                    _registers = new long[4];
                }
                else
                {
                    _registers = initialRegisters;
                }
                _opCodeMap = new Dictionary<int, HashSet<OpCode>>();
            }

            public class Instruction
            {
                public OpCode OpCode;
                public long InputA;
                public long InputB;
                public long OutputC;
            }

            private long minReg1 = long.MaxValue;
            public long[] PerformInstructionSet(List<Instruction> instructions, int instructionPointerRegister, out long cycles, long prevLowestCycles = long.MaxValue, bool print = false)
            {
                long instructionPointer = 0;
                cycles = 0;
                while(instructionPointer < instructions.Count && cycles < prevLowestCycles)
                {
                    if(instructionPointer == 28)
                    {
                        if(_registers[1] < minReg1) minReg1 = _registers[1];
                    }
                    cycles++;
                    var instruction = instructions[(int)instructionPointer];
                    string firstRegisters = print ? string.Join(", ", _registers) : null;
                    
                    PerformInstruction(instruction.OpCode, instruction.InputA, instruction.InputB, instruction.OutputC);
                    if(print)
                    {
                        Console.WriteLine("ip={0} [{1}] {2} {3} {4} {5} {6}", instructionPointer, firstRegisters, 
                        instruction.OpCode, instruction.InputA, instruction.InputB, instruction.OutputC,
                        string.Join(", ", _registers));
                    }
                    instructionPointer = _registers[instructionPointerRegister];
                    if(++instructionPointer == instructions.Count) break;
                    _registers[instructionPointerRegister] = instructionPointer;
                }
                
                return _registers;
            }

            public long[] PerformInstruction(Instruction instruction)
            {
                return PerformInstruction(instruction.OpCode, instruction.InputA, instruction.InputB, instruction.OutputC);
            }

            public long[] PerformInstruction(OpCode opCode, long inputA, long inputB, long outputC)
            {
                var getter = _opcodeValueGetters[opCode];
                long value = _opcodePerformOp[opCode](getter[0](inputA, _registers), getter[1](inputB, _registers));

                _registers[outputC] = value;

                return _registers;
            }

            private void PerformInstruction(int opCodeId, long inputA, long inputB, long outputC)
            {
                OpCode opCode = _opCodeMap[opCodeId].First();
                var getter = _opcodeValueGetters[opCode];
                long value = _opcodePerformOp[opCode](getter[0](inputA, _registers), getter[1](inputB, _registers));

                _registers[outputC] = value;
            }

            public long Part2(List<long[]> list)
            {
                _registers = list[0].ToArray();

                foreach (var instruction in list)
                {
                    PerformInstruction((int)instruction[0], instruction[1], instruction[2], instruction[3]);
                }

                return _registers[0];
            }

            public long Part1(List<long[]> list)
            {
                int matches = 0;
                int opCodeNumber = (int)list[1][0];
                
                HashSet<OpCode> thisRunPossibleOpCodes = new HashSet<OpCode>();
                foreach (var opCode in _opcodePerformOp.Keys)
                {
                    _registers = list[0].ToArray();
                    var result = PerformInstruction(opCode, list[1][1], list[1][2], list[1][3]);
                    if(result.SequenceEqual(list[2]))
                    {
                        thisRunPossibleOpCodes.Add(opCode);
                        matches++;
                    }
                }

                HashSet<OpCode> possibleOpCodes;
                if(!_opCodeMap.TryGetValue(opCodeNumber, out possibleOpCodes))
                {
                    _opCodeMap[opCodeNumber] = thisRunPossibleOpCodes;
                }
                else
                {
                    var thisCodeValues =_opCodeMap[opCodeNumber];
                    thisCodeValues.RemoveWhere(x => !thisRunPossibleOpCodes.Contains(x));
                    if(thisCodeValues.Count == 1)
                    {
                        foreach (var otherCodeValues in _opCodeMap.Values.Where(x => x != thisCodeValues))
                        {
                            otherCodeValues.Remove(thisCodeValues.First());
                        }
                    }
                }

                return matches;
            }
        }
}
