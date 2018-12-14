using System;
using System.Collections.Generic;
using System.Linq;
using common;

namespace day13
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid grid = new Grid();
            SortedSet<Cart> cartsInOrder = new SortedSet<Cart>();
            using(var reader = new InputReader("input.txt"))
            {
                int y = 0;
                foreach (var line in reader.GetLines())
                {
                    int x = 0;
                    foreach (char character in line)
                    {
                        Cart cart = null;
                        Track track = null;
                        if(Cart.IsCart(character))
                        {
                            track = new Track(Cart.GetStartingCharUnderCart(character));
                            cart = new Cart(x, y, character, track);
                            cartsInOrder.Add(cart);
                            track.CurrentCart = cart;
                        }
                        else
                        {
                            track = new Track(character);
                        }

                        grid.Add(x, y, track);
                        x++;
                    }
                    y++;
                }
            }

            bool crash = false;
            int iteration = 0;
            while(!crash)
            {
                List<Cart> cartsToProcess = cartsInOrder.ToList();
                cartsInOrder = new SortedSet<Cart>();
                grid.Print(iteration.ToString());
                foreach (var cart in cartsToProcess)
                {
                    if(cart.Move(grid))
                    {
                        Console.WriteLine("Part 1: {0},{1}", cart.X, cart.Y);
                        crash = true;
                        break;
                    }

                    cartsInOrder.Add(cart);
                }

                iteration++;
            }
        }

        private class Cart : Point, IComparable<Cart>
        {
            public char CurrentDirection {get; private set;}
            private int _currentTurn;
            private Track _currentTrack;

            private enum Turn
            {
                Left = -1,
                Right = 1,
                Straight = 0
            }

            public const char UP = '^';
            public const char DOWN = 'v';
            public const char LEFT = '<';
            public const char RIGHT = '>';

            private static readonly List<char> _directions = new List<char>(){UP, RIGHT, DOWN, LEFT};

            private static readonly Turn[] _turns = new Turn[] { Turn.Left, Turn.Straight, Turn.Right};

            public Cart(long x, long y, char currentDirection, Track currentTrack) : base(x, y)
            {
                CurrentDirection = currentDirection;
                _currentTurn = 0;
                _currentTrack = currentTrack;
            }

            public static bool IsCart(char input)
            {
                return input == UP || input == DOWN || input == LEFT || input == RIGHT;
            }

            public static char GetStartingCharUnderCart(char input)
            {
                if(input == UP || input == DOWN) return Track.STRAIGHT_VERT;
                return Track.STRAIGHT_HORZ;
            }

            public int CompareTo(Cart other)
            {
                int xComp = this.X.CompareTo(other.X);
                if(xComp != 0) return xComp;

                int yComp = this.Y.CompareTo(other.Y);
                if(yComp != 0) return yComp;

                return 1;
            }

            public bool Move(Grid grid)
            {
                int xDelta = 0;
                int yDelta = 0;

                switch (this.CurrentDirection)
                {
                    case UP:
                        yDelta = -1;
                        break;
                    case DOWN:
                        yDelta = 1;
                        break;
                    case LEFT:
                        xDelta = -1;
                        break;
                    case RIGHT:
                        xDelta = 1;
                        break;
                }

                X = X + xDelta;
                Y = Y + yDelta;

                var track = grid[(int)X, (int)Y];

                if(track.CurrentCart != null)
                {
                    return true;
                }

                track.CurrentCart = this;
                this._currentTrack.CurrentCart = null;
                this._currentTrack = track;

                PerformTurn();

                return false;
            }

            private void PerformTurn()
            {
                if(_currentTrack.TrackType == Track.INTERSECTION)
                {
                    int directionIdx = _directions.IndexOf(CurrentDirection);

                    int newIdx = ((directionIdx + (int)_turns[_currentTurn]) + _directions.Count) % _directions.Count;

                    _currentTurn = (_currentTurn + 1) % _turns.Length;

                    CurrentDirection = _directions[newIdx];
                }
                else if (_currentTrack.TrackType == Track.CURVE_1)
                {
                    // Curve 1 is \, which means right turns down, up turns left, left turns up, and down turns right
                    if(CurrentDirection == RIGHT) CurrentDirection = DOWN;
                    else if(CurrentDirection == DOWN) CurrentDirection = RIGHT;
                    else if(CurrentDirection == UP) CurrentDirection = LEFT;
                    else if(CurrentDirection == LEFT) CurrentDirection = UP;
                }
                else if (_currentTrack.TrackType == Track.CURVE_2)
                {
                    // Curve 1 is /, which means right turns up, up turns right, left turns down, and down turns left
                    if(CurrentDirection == RIGHT) CurrentDirection = UP;
                    else if(CurrentDirection == DOWN) CurrentDirection = LEFT;
                    else if(CurrentDirection == UP) CurrentDirection = RIGHT;
                    else if(CurrentDirection == LEFT) CurrentDirection = DOWN;
                }
                
            }
        }

        private class Grid
        {
            private List<List<Track>> _grid = new List<List<Track>>();

            public Track this[int x, int y] 
            {
                get
                {
                    return _grid[y][x];
                }
                set
                {
                    _grid[y][x] = value;
                }
            }

            public void Add(int x, int y, Track value)
            {
                PadOutListToSize(_grid, y, () => new List<Track>());
                
                List<Track> xRow = _grid[y];

                PadOutListToSize(xRow, x, () => new Track(' '));

                xRow[x] = value;
            }

            private void PadOutListToSize<T>(List<T> list, int size, Func<T> valueCreator)
            {
                if(list.Count <= size)
                {
                    for (int i = list.Count; i <= size; i++)
                    {
                        list.Add(valueCreator());
                    }
                }
            }

            public void Print(string turn)
            {
                Console.WriteLine("Turn {0} ----------------------------------------", turn);
                foreach (var row in _grid)
                {
                    foreach (var column in row)
                    {
                        char value = column.TrackType;
                        if(column.CurrentCart != null) value = column.CurrentCart.CurrentDirection;

                        Console.Write(value);
                    }
                    Console.Write("\n");
                }
            }
        }

        private class Track
        {
            public const char STRAIGHT_HORZ = '-';
            public const char STRAIGHT_VERT = '-';
            public const char CURVE_1 = '\\';
            public const char CURVE_2 = '/';
            public const char INTERSECTION = '+';

            public const char BLANK = ' ';

            public char TrackType {get; private set;}
            public Cart CurrentCart {get; set;}
            public Track(char type, Cart cart = null)
            {
                TrackType = type;
                CurrentCart = cart;
            }


        }
    }
}
