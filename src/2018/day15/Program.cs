using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day15
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Tuple<int, int, string>> tests = new List<Tuple<int, int, string>>();
            tests.Add(new Tuple<int, int, string>(47, 590, "test1.txt"));
            tests.Add(new Tuple<int, int, string>(37, 982, "test2.txt"));
            tests.Add(new Tuple<int, int, string>(46, 859, "test3.txt"));
            tests.Add(new Tuple<int, int, string>(35, 793, "test4.txt"));
            tests.Add(new Tuple<int, int, string>(54, 536, "test5.txt"));
            tests.Add(new Tuple<int, int, string>(20, 937, "test6.txt"));

            foreach (var test in tests)
            {
                List<string> lines;
                using (var reader = new InputReader(test.Item3))
                {
                    lines = reader.GetLines().ToList();
                }

                bool print = false;//test.Item3 == "test4.txt" || test.Item3 == "test5.txt";

                int turn, hp;
                RunCombat(lines, print, out turn, out hp);
                Console.WriteLine("Part1: {0} full turns * {1} HP = {2}", turn, hp, turn * hp);
                if(turn == test.Item1 && hp == test.Item2)
                {
                    Console.WriteLine("Test Passes: " + test.Item3);
                }
                else
                {
                    Console.WriteLine("Test Fails: {0}. Expect {1}*{2}, found {3}*{4}", test.Item3, test.Item1, test.Item2, turn, hp);
                }
            }

            List<string> realLines;
            using (var reader = new InputReader("input.txt"))
            {
                realLines = reader.GetLines().ToList();
            }

            int realTurn, realHp;
            RunCombat(realLines, false, out realTurn, out realHp);
            Console.WriteLine("Part1: {0} full turns * {1} HP = {2}", realTurn, realHp, realTurn * realHp);
        }

        private static void RunCombat(List<string> lines, bool print, out int turn, out int hp)
        {
            Board board = null;
            for (int y = 0; y < lines.Count; y++)
            {
                var line = lines[y];

                if (y == 0) board = new Board(line.Length, lines.Count);

                for (int x = 0; x < line.Length; x++)
                {
                    board[x, y] = new Unit(x, y, line[x]);
                }
            }
            turn = 0;
            for (; turn < 100000; turn++)
            {
                var units = board.GetUnitsInOrder(Unit.ELF, Unit.GOBLIN);
                if (print)
                {
                    Console.WriteLine("Turn " + turn + "--------------------------");
                    board.Print();
                    foreach (var unit in units)
                    {
                        Console.WriteLine("{0}[{2},{3}]: HP={1}", unit.UnitType, unit.HitPoints, unit.X, unit.Y);
                    }
                }

                bool noMoreEnemies = false;
                foreach (var unit in units)
                {
                    if (unit.IsDead) continue;

                    if (unit.TakeTurn(board))
                    {
                        noMoreEnemies = true;
                        break;
                    }
                }

                if (noMoreEnemies) break;

                if (print)
                {
                    board.Print();
                    foreach (var unit in units)
                    {
                        Console.WriteLine("{0}[{2},{3}]: HP={1}", unit.UnitType, unit.HitPoints, unit.X, unit.Y);
                    }
                }
            }

            hp = board.GetUnitsInOrder(Unit.ELF, Unit.GOBLIN).Sum(x => x.HitPoints);
        }

        private class Unit : Point
        {
            public const char ELF = 'E';
            public const char GOBLIN = 'G';
            public const char WALL = '#';
            public const char EMPTY = '.';

            public override string ToString()
            {
                return ""+UnitType;
            }

            private char _unitType;
            public char UnitType 
            { 
                get
                {
                    if(IsDead && _unitType != WALL) return EMPTY;
                    return _unitType;
                } 
                set
                {
                    _unitType = value;
                }
            }
            public Unit(long x, long y, char type) : base(x, y)
            {
                UnitType = type;

                switch (type)
                {
                    case ELF:
                    case GOBLIN:
                        HitPoints = 200;
                        AttackPower = 3;
                        break;
                }
            }

            public bool IsDead
            {
                get
                {
                    return HitPoints <= 0;
                }
            }
            
            public int HitPoints { get; private set; }
            public int AttackPower { get; private set; }
            public override long Weight 
            { 
                get
                {
                    if(this.UnitType == EMPTY) return 1;
                    return long.MaxValue;
                }
            }

            public bool AttemptAttack(Board board)
            {
                var adjacentEnemy = board.GetSquaresAdjacentToUnit(this, GetEnemy())
                    .OrderBy(x => x.HitPoints)
                    .ThenBy(x => x).FirstOrDefault();

                if(adjacentEnemy == null) return false;

                adjacentEnemy.HitPoints -= this.AttackPower;
                
                return true;
            }

            public char GetEnemy()
            {
                return this.UnitType == ELF ? GOBLIN : ELF;
            }

            internal bool TakeTurn(Board board)
            {
                var enemies = board.GetUnitsInOrder(GetEnemy());
                if (!enemies.Any()) return true;

                if (AttemptAttack(board))
                {
                    return false;
                }

                Unit closestSpace = FindNextClosestSpace(board, enemies);

                if (closestSpace == null)
                {
                    return false;
                }

                MoveUnit(board, closestSpace.X, closestSpace.Y);

                AttemptAttack(board);

                return false;
            }

            private Unit FindNextClosestSpaceDeux(Board board, IEnumerable<Unit> enemies, long[,,,] allPathLengths)
            {
                long shortestLength = long.MaxValue;
                Unit bestMove = null;
                foreach (var possibleMove in board.GetSquaresAdjacentToUnit(this, Unit.EMPTY))
                {
                    foreach (var enemy in enemies)
                    {
                        foreach (var emptySpace in board.GetSquaresAdjacentToUnit(enemy, Unit.EMPTY))
                        {
                            var length = allPathLengths[possibleMove.X,possibleMove.Y,emptySpace.X,emptySpace.Y];

                            if(length<shortestLength)
                            {
                                shortestLength = length;
                                bestMove = possibleMove;
                            }
                        }
                    }
                }
                

                return bestMove;
            }

            private void MoveUnit(Board board, long x, long y)
            {
                board[this.X, this.Y] = new Unit(this.X, this.Y, Unit.EMPTY);
                board[x, y] = this;
                this.X = x;
                this.Y = y;
            }

            private Unit FindNextClosestSpace(Board board, IEnumerable<Unit> enemies)
            {
                long closestDistance = long.MaxValue;
                Unit closestPath = null;

                foreach (var possibleSpace in board.GetSquaresAdjacentToUnit(this, Unit.EMPTY))
                {
                    foreach (var enemy in enemies)
                    {
                        foreach (var spaceNextToEnemy in board.GetSquaresAdjacentToUnit(enemy, Unit.EMPTY))
                        {
                            var paths = board.GetShortestPaths(true, possibleSpace, spaceNextToEnemy);
                            var minPath = paths.FirstOrDefault();

                            // No path to this enemy
                            if(minPath == null) continue;

                            if (minPath.Distance < closestDistance)
                            {
                                closestPath = possibleSpace;
                                closestDistance = minPath.Distance;
                            }   
                        }                                            
                    }
                }

                return closestPath;
            }
        }

        private class Board : Grid<Unit>
        {
            private static readonly EmptyGenerator _generator = (long x, long y) => new Unit(x, y, Unit.EMPTY);

            public Board(long width, long height) : base(width, height, _generator)
            {
            }

            public IEnumerable<Unit> GetUnitsInOrder(params char[] types)
            {
                List<Unit> units = new List<Unit>();
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        Unit unit = this[x,y];
                        if(types.Contains(unit.UnitType))
                        {
                            units.Add(unit);
                        }
                    }
                }

                return units;
            }

            public IEnumerable<Unit> GetSquaresAdjacentToUnit(Unit unit, char unitType)
            {
                return GetAdjacentSquares(unit).Where(x => x.UnitType == unitType);
            }
        }
    }
}
