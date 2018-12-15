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
            List<string> lines;
            using(var reader = new InputReader("test2.txt"))
            {
                lines = reader.GetLines().ToList();
            }
            
            Board board = null;
            for (int y = 0; y < lines.Count; y++)
            {
                var line = lines[y];

                if(y == 0) board = new Board(line.Length, lines.Count);

                for (int x = 0; x < line.Length; x++)
                {
                    board[x,y] = new Unit(x,y, line[x]);
                }
            }
            int turn = 0;
            bool print = true;
            for (; turn < 100000; turn++)
            {
                
                var units = board.GetUnitsInOrder(Unit.ELF, Unit.GOBLIN);
                if(print)
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
                    if(unit.IsDead) continue;

                    if(unit.TakeTurn(board))
                    {
                        noMoreEnemies = true;
                        break;
                    }
                }

                if(noMoreEnemies) break;

                if(print)
                {
                    board.Print();
                    foreach (var unit in units)
                    {
                        Console.WriteLine("{0}[{2},{3}]: HP={1}", unit.UnitType, unit.HitPoints, unit.X, unit.Y);
                    }
                }
            }

            int hp = board.GetUnitsInOrder(Unit.ELF, Unit.GOBLIN).Sum(x => x.HitPoints);
            Console.WriteLine("Part1: {0} full turns * {1} HP = {2}", turn, hp, turn*hp);
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

            private void MoveUnit(Board board, long x, long y)
            {
                board[this.X, this.Y] = new Unit(this.X, this.Y, Unit.EMPTY);
                board[x, y] = this;
                this.X = x;
                this.Y = y;
            }

            private Unit FindNextClosestSpace(Board board, IEnumerable<Unit> enemies)
            {
                var sortedEnemies = enemies.Select(x => new
                {
                    Enemy = x,
                    EmptySpaces = board.GetSquaresAdjacentToUnit(x, Unit.EMPTY).Select(y => new
                    {
                        Space = y,
                        ManhattenDistance = this.ManhattenDistance(y)
                    }).OrderBy(z => z.ManhattenDistance)

                }).Where(x => x.EmptySpaces.Any()).OrderBy(x => x.EmptySpaces.First().ManhattenDistance);

                long closestDistance = long.MaxValue;
                IEnumerable<IEnumerable<Unit>> minPaths = null;

                foreach (var enemy in sortedEnemies)
                {
                    if (enemy.EmptySpaces.First().ManhattenDistance > closestDistance) break;

                    var paths = board.GetShortestPaths(this, enemy.EmptySpaces.Select(x => x.Space).ToArray());
                    var minPath = paths.Select(x => new {
                        Paths = x.Paths,
                        Distance = x.Distance
                    }).OrderBy(x => x.Distance).FirstOrDefault();

                    // No path to this enemy
                    if(minPath == null) continue;

                    if (minPath.Distance < closestDistance)
                    {
                        minPaths = minPath.Paths;
                        closestDistance = minPath.Distance;
                    }
                }

                if(minPaths == null) return null;

                // All of the first steps will just be a distance of 1, so grab the first
                // one that in reading order.
                return minPaths.Select(x => x.Take(2).Last()).OrderBy(x => x).FirstOrDefault();
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
