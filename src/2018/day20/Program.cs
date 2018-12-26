using System;
using System.Collections.Generic;
using System.Text;
using common;

namespace day20
{
    class Program
    {
        static void Main(string[] args)
        {
            string regex;
            using(var reader = new InputReader("test.txt"))
            {
                regex = reader.GetNextLine();
            }
            
            // Strip off the ^ and $
            regex = regex.Substring(1, regex.Length - 2);

            var _base = new NorthPoleBase();

            var currentRoom = new Room(0, 0);
            _base[0, 0] = currentRoom;

            int index = 0;
            _base.BuildMap(regex, ref index, currentRoom);

            _base.Print();
            var test = 0;
        }

        private class NorthPoleBase : CartesianPlane<Room>
        {
            internal void BuildMap(string regex, ref int currentIdx, Room currentRoom)
            {
                while(regex.Length > currentIdx)
                {
                    char direction = regex[currentIdx];
                    currentIdx++;
                    switch (direction)
                    {
                        case '(':
                            Room newRoom = currentRoom;
                            BuildMap(regex, ref currentIdx, newRoom);
                        break;
                        case ')':
                        case '|':
                            return;
                        default:
                            currentRoom = HandleMove(direction, currentRoom);
                        break;
                    }
                }
            }

            private Room HandleMove(char direction, Room currentRoom)
            {
                long x = currentRoom.X;
                long y = currentRoom.Y;
                Action<Room> doorAdder = (door) => {};
                switch (direction)
                {
                    case 'N':
                        doorAdder = currentRoom.AddNorthDoor;
                        y += 1;
                    break;
                    case 'S':
                        doorAdder = currentRoom.AddSouthDoor;
                        y += -1;
                    break;
                    case 'E':
                        doorAdder = currentRoom.AddEastDoor;
                        x += 1;
                    break;
                    case 'W':
                        doorAdder = currentRoom.AddWestDoor;
                        x += -1;
                    break;
                }

                var newRoom = this[x, y];
                if(newRoom == null)
                {
                    newRoom = new Room(x, y);
                    this[x,y] = newRoom;
                }
                
                doorAdder(newRoom);
                return newRoom;
            }

            public override string Print(bool toConsole = true)
            {
                StringBuilder sb = new StringBuilder();

                const int cellsPerRoom = 3;
                for (long y = _maxY * cellsPerRoom; y >= _minY * cellsPerRoom ; y--)
                {
                    long subY = Math.Abs(y) % cellsPerRoom;
                    long realY = GetRealValue(y, cellsPerRoom);
                    for (long x = _minX * cellsPerRoom; x <= _maxX * cellsPerRoom; x++)
                    {
                        long realX = GetRealValue(x, cellsPerRoom);
                        var point = this[realX,realY];
                        long subX = Math.Abs(x) % cellsPerRoom;
                        if(point == null)
                        {
                            if(toConsole) Console.Write(" ");
                            else sb.Append(" ");
                        }
                        else
                        {
                            string value = "#";
                            if(subX == 1 && subY == 1)
                            {
                                // Middle of the room
                                value = ".";
                            }
                            else if(subX == 1)
                            {
                                if(subY == 0)
                                {
                                    // North door
                                    value = point.NorthRoomThroughDoor == null ? "#" : "-";
                                }
                                else
                                {
                                    // South door
                                    value = point.SouthRoomThroughDoor == null ? "#" : "-";
                                }
                            }
                            else if(subY == 1)
                            {
                                if(subX == 0)
                                {
                                    // West door
                                    value = point.WestRoomThroughDoor == null ? "#" : "|";
                                }
                                else
                                {
                                    // East door
                                    value = point.EastRoomThroughDoor == null ? "#" : "|";
                                }
                            }


                            if(toConsole) Console.Write(value);
                            else sb.Append(value);
                        }
                    }
                    if(toConsole) Console.WriteLine();
                    else sb.AppendLine();
                }

                if(!toConsole) return sb.ToString();
                return null;
            }

            private long GetRealValue(long xOrY, int cellsPerRoom)
            {
                long value = Math.Abs(xOrY) / cellsPerRoom;
                int modifier = 0;
                if(xOrY % cellsPerRoom != 0) modifier = 1;

                if(xOrY >= 0)
                {
                    return value + modifier;
                }
                

                return -value - modifier;
            }
        }

        private class Room : Point
        {
            public Room NorthRoomThroughDoor { get; private set; }
            public Room SouthRoomThroughDoor { get; private set; }
            public Room EastRoomThroughDoor { get; private set; }
            public Room WestRoomThroughDoor { get; private set; }
            public Room(long x, long y) : base(x, y)
            {
            }

            public void AddNorthDoor(Room connectedRoom)
            {
                NorthRoomThroughDoor = connectedRoom;
                connectedRoom.SouthRoomThroughDoor = this;
            }
            public void AddSouthDoor(Room connectedRoom)
            {
                SouthRoomThroughDoor = connectedRoom;
                connectedRoom.NorthRoomThroughDoor = this;
            }
            public void AddEastDoor(Room connectedRoom)
            {
                EastRoomThroughDoor = connectedRoom;
                connectedRoom.WestRoomThroughDoor = this;
            }
            public void AddWestDoor(Room connectedRoom)
            {
                WestRoomThroughDoor = connectedRoom;
                connectedRoom.EastRoomThroughDoor = this;
            }
        }
    }
}
