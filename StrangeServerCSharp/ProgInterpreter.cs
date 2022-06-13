using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeServerCSharp
{
    public class ProgInterpreter
    {
        private static ProgInterpreter _this;
        public static ProgInterpreter THIS => _this ??= new ProgInterpreter();

        public void Init()
        {
            while (true)
            {
                if (XServer.THIS.PlayerSessions == null)
                {
                    continue;
                }

                foreach (var player in XServer.THIS.PlayerSessions.Where(x => x.player is { ProgData.IsActive: true })
                             .Select(x => x.player))
                {
                    if (player.ProgData.NextRun > DateTime.Now)
                    {
                        continue;
                    }

                    if (player.ProgData.X == 16)
                    {
                        player.ProgData.X = 0;
                        player.ProgData.Y = 0;
                    }

                    var action = player.ProgData.ActionMatrix[player.ProgData.Y, player.ProgData.X];
                    while (action == null && player.ProgData.X < 15)
                    {
                        player.ProgData.X++;
                        action = player.ProgData.ActionMatrix[player.ProgData.Y, player.ProgData.X];
                    }

                    if (action != null)
                    {
                        switch (action.Type)
                        {
                            case ActionType.MoveUp:
                                player.Move(player.x, player.y - 1, 2, true);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.MoveLeft:
                                player.Move(player.x - 1, player.y, 1, true);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.MoveDown:
                                player.Move(player.x, player.y + 1, 0, true);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.MoveRight:
                                player.Move(player.x + 1, player.y, 3, true);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.MoveForward:
                                player.Move((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, player.dir, true);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.RotateUp:
                                player.Move(player.x, player.y, 2, true);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.RotateLeft:
                                player.Move(player.x, player.y, 1, true);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.RotateDown:
                                player.Move(player.x, player.y, 0, true);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.RotateRight:
                                player.Move(player.x, player.y, 3, true);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.RotateLeftRelative:
                                player.dir = player.dir switch
                                {
                                    0 => 3,
                                    1 => 0,
                                    2 => 1,
                                    3 => 2
                                };
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.RotateRightRelative:
                                player.dir = player.dir switch
                                {
                                    0 => 1,
                                    1 => 2,
                                    2 => 3,
                                    3 => 0
                                };
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.RotateRandom:
                                var rand = new Random(Guid.NewGuid().GetHashCode());
                                player.dir = rand.Next(4);
                                player.ProgData.AddDelay(50);
                                break;
                            case ActionType.Dig:
                                var x = (uint)player.GetDirCord().X;
                                var y = (uint)player.GetDirCord().Y;
                                if (World.THIS.ValidCoord(x, y))
                                {
                                    player.Bz(x, y);
                                }

                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.PlaceBlock:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "G");
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.PlacePillar:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "V");
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.PlaceRoad:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "R");
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.Geology:
                                player.UseGeo((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y);
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.Heal:
                                player.Heal();
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.Label:
                                player.Heal();
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.GoTo:
                                var labelCoords = player.ProgData.IndexOf(action.Label);
                                player.ProgData.X = labelCoords.X;
                                player.ProgData.Y = labelCoords.Y;
                                continue;
                            case ActionType.RunSub:
                                var subCoords = player.ProgData.IndexOf(action.Label);
                                player.ProgData.ReturnX = player.ProgData.X + 1;
                                player.ProgData.ReturnY = player.ProgData.Y;
                                player.ProgData.X = subCoords.X;
                                player.ProgData.Y = subCoords.Y;
                                continue;
                            case ActionType.Return:
                                player.ProgData.X = player.ProgData.ReturnX;
                                player.ProgData.Y = player.ProgData.ReturnY;
                                continue;
                            case ActionType.NextRow:
                                player.ProgData.X = 0;
                                player.ProgData.Y++;
                                continue;
                        }
                    }

                    player.ProgData.X++;
                }

                Thread.Sleep(5);
            }
        }
    }

    public class ProgData
    {
        public ProgAction[,] ActionMatrix { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsActive { get; set; }
        public DateTime NextRun { get; set; }
        public int ReturnX { get; set; }
        public int ReturnY { get; set; }

        public void AddDelay(int ms)
        {
            NextRun = DateTime.Now + TimeSpan.FromMilliseconds(ms);
        }

        public (int X, int Y) IndexOf(string label)
        {
            for (var i = 0; i < ActionMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < ActionMatrix.GetLength(1); j++)
                {
                    var action = ActionMatrix[i, j];
                    if (action == null)
                    {
                        continue;
                    }

                    if (action.Label == label && action.Type == ActionType.Label)
                    {
                        return (j, i);
                    }
                }
            }

            return (0, 0);
        }

        public static ProgData FromString(string text)
        {
            var index = text.IndexOf("$");
            text = text.Substring(index + 1);
            var data = new ProgData
            {
                ActionMatrix = new ProgAction[100, 16],
                X = 0,
                Y = 0,
                IsActive = true,
                NextRun = DateTime.Now
            };
            var x = 0;
            var y = 0;
            for (var i = 0; i < text.Length; i++)
            {
                int next;
                switch (text[i])
                {
                    case 'w':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.RotateUp);
                        break;
                    case 'a':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.RotateLeft);
                        break;
                    case 's':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.RotateDown);
                        break;
                    case 'd':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.RotateRight);
                        break;
                    case 'z':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.Dig);
                        break;
                    case 'b':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.PlaceBlock);
                        break;
                    case 'q':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.PlacePillar);
                        break;
                    case 'r':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.PlaceRoad);
                        break;
                    case 'g':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.Geology);
                        break;
                    case 'h':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.Heal);
                        break;
                    case ',':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.NextRow);
                        break;
                    case '#':
                        i++;
                        switch (text[i])
                        {
                            case 'E':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.Stop);
                                break;
                        }

                        break;
                    case ':':
                        i++;
                        switch (text[i])
                        {
                            case '>':
                                next = text[(i+1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    data.ActionMatrix[y, x] = new ProgAction(ActionType.RunSub,
                                        text[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '>':
                        next = text[(i + 1)..].IndexOf('|');
                        if (next != -1)
                        {
                            next++;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.GoTo, text[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '|':
                        next = text[(i + 1)..].IndexOf(':');
                        if (next != -1)
                        {
                            next++;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.Label, text[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '<':
                        i++;
                        switch (text[i])
                        {
                            case '|':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.Return);

                                break;
                        }

                        break;
                    case '^':
                        i++;
                        switch (text[i])
                        {
                            case 'W':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.MoveUp);
                                break;
                            case 'A':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.MoveLeft);
                                break;
                            case 'S':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.MoveDown);
                                break;
                            case 'D':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.MoveRight);
                                break;
                            case 'F':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.MoveForward);
                                break;
                        }

                        break;
                    default:
                        var currentText = text[i..];
                        if (currentText.StartsWith("CCW;"))
                        {
                            i += 3;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.RotateLeftRelative);
                        }
                        else if (currentText.StartsWith("CW;"))
                        {
                            i += 2;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.RotateRightRelative);
                        }
                        else if (currentText.StartsWith("RAND;"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.RotateRandom);
                        }

                        break;
                }

                x++;
                if (x == 16 || text[i] == '\n')
                {
                    x = 0;
                    y++;
                }
            }

            return data;
        }
    }

    public class ProgAction
    {
        public ActionType Type { get; set; }
        public string Label { get; set; }

        public ProgAction(ActionType type, string label = "")
        {
            Type = type;
            Label = label;
        }
    }

    public enum ActionType
    {
        MoveUp,
        MoveLeft,
        MoveDown,
        MoveRight,
        MoveForward,
        RotateUp,
        RotateLeft,
        RotateDown,
        RotateRight,
        RotateLeftRelative,
        RotateRightRelative,
        RotateRandom,
        Dig,
        PlaceBlock,
        PlacePillar,
        PlaceRoad,
        Geology,
        Heal,
        NextRow,
        Label,
        GoTo,
        RunSub,
        RunFunction,
        Return,
        Stop
    }
}