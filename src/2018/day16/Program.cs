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

            Tuple<int,int> result = RunCPU(lines, true);
            Console.WriteLine("Part 1 Result = {0}", result.Item1);
            Console.WriteLine("Part 2 Result = {0}", result.Item2);
        }

        private static Tuple<int,int> RunCPU(List<string> lines, bool runPart2)
        {
            int linesLeftInSet = 3;
            List<int[]> linesInSet = new List<int[]>();
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

            int part2 = !runPart2 ? 0 : cpu.Part2(linesInSet);

            return new Tuple<int, int>(part1Results, part2);
        }

        private static int[] ParseInstruction(string line)
        {
            return line.Split(' ').Select(x => int.Parse(x)).ToArray();
        }

        private static int[] ParseRegisters(string line)
        {
            int startIdx = line.IndexOf('[') + 1;
            int endIdx = line.IndexOf(']', startIdx);
            return line.Substring(startIdx, endIdx-startIdx).Split(", ").Select(x => int.Parse(x)).ToArray();
        }

        private class CPU
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

            private delegate int GetValue(int input, int[] registers);

            private static GetValue GetImmediate = (int input, int[] registers) =>
            {
                return input;
            };

            private static GetValue GetRegister = (int input, int[] registers) =>
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

            private delegate int RunOp(int inputA, int inputB);
            private static int Add(int inputA, int inputB)
            {
                return inputA + inputB;
            }
            private static int And(int inputA, int inputB)
            {
                return inputA & inputB;
            }
            private static int Or(int inputA, int inputB)
            {
                return inputA | inputB;
            }
            private static int Assignment(int inputA, int inputB)
            {
                return inputA;
            }
            private static int Multiply(int inputA, int inputB)
            {
                return inputA * inputB;
            }
            private static int Equal(int inputA, int inputB)
            {
                if(inputA == inputB) return 1;
                return 0;
            }
            private static int GreaterThan(int inputA, int inputB)
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

            private int[] _registers;
            private Dictionary<int, HashSet<OpCode>> _opCodeMap;

            public CPU(int[] initialRegisters = null)
            {
                if(initialRegisters == null)
                {
                    _registers = new int[4];
                }
                else
                {
                    _registers = initialRegisters;
                }
                _opCodeMap = new Dictionary<int, HashSet<OpCode>>();
                // for (int i = 0; i < 16; i++)
                // {
                //     _opCodeMap[i] = new HashSet<OpCode>(Enum.GetValues(typeof(OpCode)).OfType<OpCode>());
                // }
            }

            public int[] PerformInstruction(OpCode opCode, int inputA, int inputB, int outputC)
            {
                var getter = _opcodeValueGetters[opCode];
                int value = _opcodePerformOp[opCode](getter[0](inputA, _registers), getter[1](inputB, _registers));

                _registers[outputC] = value;

                return _registers;
            }

            private void PerformInstruction(int opCodeId, int inputA, int inputB, int outputC)
            {
                OpCode opCode = _opCodeMap[opCodeId].First();
                var getter = _opcodeValueGetters[opCode];
                int value = _opcodePerformOp[opCode](getter[0](inputA, _registers), getter[1](inputB, _registers));

                _registers[outputC] = value;
            }

            public int Part2(List<int[]> list)
            {
                _registers = list[0].ToArray();

                foreach (var instruction in list)
                {
                    PerformInstruction(instruction[0], instruction[1], instruction[2], instruction[3]);
                }

                return _registers[0];
            }

            public int Part1(List<int[]> list)
            {
                int matches = 0;
                int opCodeNumber = list[1][0];
                
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
}
