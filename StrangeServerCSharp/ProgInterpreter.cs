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
                    var action = player.ProgData.ActionMatrix[player.ProgData.Y, player.ProgData.X];
                    if (action != null)
                    {
                        switch (action.Type)
                        {
                            case ActionType.MoveUp:
                                player.Move(player.x, player.y - 1, 2, true);
                                break;
                            case ActionType.MoveLeft:
                                player.Move(player.x - 1, player.y, 1, true);
                                break;
                            case ActionType.MoveDown:
                                player.Move(player.x, player.y + 1, 0, true);
                                break;
                            case ActionType.MoveRight:
                                player.Move(player.x + 1, player.y, 3, true);
                                break;
                            case ActionType.RotateUp:
                                player.Move(player.x, player.y, 2, true);
                                break;
                            case ActionType.RotateLeft:
                                player.Move(player.x, player.y, 1, true);
                                break;
                            case ActionType.RotateDown:
                                player.Move(player.x, player.y, 0, true);
                                break;
                            case ActionType.RotateRight:
                                player.Move(player.x, player.y, 3, true);
                                break;
                            case ActionType.Dig:
                                var x = (uint)player.GetDirCord().X;
                                var y = (uint)player.GetDirCord().Y;
                                if (World.THIS.ValidCoord(x, y))
                                {
                                    player.Bz(x, y);
                                }
                                break;
                            case ActionType.PlaceBlock:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "G");
                                break;
                            case ActionType.PlacePillar:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "V");
                                break;
                            case ActionType.PlaceRoad:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "R");
                                break;
                        }

                        player.ProgData.NextRun = DateTime.Now + TimeSpan.FromMilliseconds(500);
                    }
                    
                    player.ProgData.X++;
                    if (action?.Type == ActionType.NextRow)
                    {
                        player.ProgData.X = 0;
                        player.ProgData.Y++;
                    }

                    if (player.ProgData.X == 16)
                    {
                        player.ProgData.X = 0;
                        player.ProgData.Y = 0;
                    }
                }

                Thread.Sleep(50);
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
                    case ',':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.NextRow);
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
        RotateUp,
        RotateLeft,
        RotateDown,
        RotateRight,
        Dig,
        PlaceBlock,
        PlacePillar,
        PlaceRoad,
        NextRow
    }
}