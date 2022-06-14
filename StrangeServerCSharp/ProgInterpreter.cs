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
                        (int X, int Y) labelCoords;
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
                            case ActionType.BuildBlock:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "G");
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.BuildPillar:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "O");
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.BuildRoad:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "R");
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.BuildMilitaryBlock:
                                player.Build((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y, "V");
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.Geology:
                                player.UseGeo((uint)player.GetDirCord().X, (uint)player.GetDirCord().Y);
                                player.ProgData.AddDelay(400);
                                break;
                            case ActionType.Heal:
                                player.Heal();
                                player.ProgData.AddDelay(160);
                                break;
                            case ActionType.Label:
                                player.Heal();
                                player.ProgData.AddDelay(160);
                                break;
                            case ActionType.And:
                                player.ProgData.ConditionMode = ActionType.And;
                                break;
                            case ActionType.Or:
                                player.ProgData.ConditionMode = ActionType.Or;
                                break;
                            case ActionType.IsHpLower100:
                                player.ProgData.Condition = player.hp < player.maxhp;
                                break;
                            case ActionType.IsHpLower50:
                                player.ProgData.Condition = player.hp < player.maxhp / 2;
                                break;
                            case ActionType.RunIfFalse:
                                labelCoords = player.ProgData.IndexOf(action.Label);
                                if (!player.ProgData.Condition)
                                {
                                    player.ProgData.X = labelCoords.X;
                                    player.ProgData.Y = labelCoords.Y;
                                    continue;
                                }
                                break;
                            case ActionType.RunIfTrue:
                                labelCoords = player.ProgData.IndexOf(action.Label);
                                if (player.ProgData.Condition)
                                {
                                    player.ProgData.X = labelCoords.X;
                                    player.ProgData.Y = labelCoords.Y;
                                    continue;
                                }
                                break;
                            case ActionType.GoTo:
                                labelCoords = player.ProgData.IndexOf(action.Label);
                                player.ProgData.X = labelCoords.X;
                                player.ProgData.Y = labelCoords.Y;
                                continue;
                            case ActionType.RunSub:
                                labelCoords = player.ProgData.IndexOf(action.Label);
                                player.ProgData.ReturnX = player.ProgData.X + 1;
                                player.ProgData.ReturnY = player.ProgData.Y;
                                player.ProgData.X = labelCoords.X;
                                player.ProgData.Y = labelCoords.Y;
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

                Thread.Sleep(1);
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
        public bool Condition { get; set; }
        public int CheckX { get; set; }
        public int CheckY { get; set; }
        public ActionType? ConditionMode { get; set; }


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
                ActionMatrix = new ProgAction[180, 16],
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
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.BuildBlock);
                        break;
                    case 'q':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.BuildPillar);
                        break;
                    case 'r':
                        data.ActionMatrix[y, x] = new ProgAction(ActionType.BuildRoad);
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
                    case '?':
                        next = text[(i + 1)..].IndexOf('>');
                        if (next != -1)
                        {
                            next++;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.RunIfFalse,
                                text[i..][1..next]);
                            i += next;
                        }

                        break;
                    case '!':
                        i++;
                        if (text[i] == '?')
                        {
                            next = text[(i + 1)..].IndexOf('>');
                            if (next != -1)
                            {
                                next++;
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.RunIfTrue,
                                    text[i..][1..next]);
                                i += next;
                            }
                        }

                        break;
                    case '[':
                        i++;
                        next = text[i..].IndexOf(']');
                        var option = text[i..][..next];
                        switch (option)
                        {
                            case "W":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckUp);
                                break;
                            case "A":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckLeft);
                                break;
                            case "S":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckDown);
                                break;
                            case "D":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckRight);
                                break;
                            case "w":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.ShiftUp);
                                break;
                            case "a":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.ShiftLeft);
                                break;
                            case "s":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.ShiftDown);
                                break;
                            case "d":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.ShiftRight);
                                break;
                            case "AS":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckDownLeft);
                                break;
                            case "WA":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckUpLeft);
                                break;
                            case "DW":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckUpRight);
                                break;
                            case "SD":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckDownRight);
                                break;
                            case "F":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckForward);
                                break;
                            case "f":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.ShiftForward);
                                break;
                            case "r":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckRightRelative);
                                break;
                            case "l":
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.CheckLeftRelative);
                                break;
                        }

                        break;
                    case '#':
                        i++;
                        switch (text[i])
                        {
                            case 'S':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.Start);
                                break;
                            case 'E':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.Stop);
                                break;
                            case 'R':
                                next = text[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    data.ActionMatrix[y, x] =
                                        new ProgAction(ActionType.RunOnRespawn, text[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case ':':
                        i++;
                        switch (text[i])
                        {
                            case '>':
                                next = text[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    data.ActionMatrix[y, x] = new ProgAction(ActionType.RunSub, text[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '-':
                        i++;
                        switch (text[i])
                        {
                            case '>':
                                next = text[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    data.ActionMatrix[y, x] =
                                        new ProgAction(ActionType.RunFunction, text[i..][1..next]);
                                    i += next;
                                }

                                break;
                        }

                        break;
                    case '=':
                        i++;
                        switch (text[i])
                        {
                            case '>':
                                next = text[(i + 1)..].IndexOf('>');
                                if (next != -1)
                                {
                                    next++;
                                    data.ActionMatrix[y, x] = new ProgAction(ActionType.RunState, text[i..][1..next]);
                                    i += next;
                                }

                                break;
                            case 'n':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsNotEmpty);
                                break;
                            case 'e':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsEmpty);
                                break;
                            case 'f':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsFalling);
                                break;
                            case 'c':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsCrystal);
                                break;
                            case 'a':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsLivingCrystal);
                                break;
                            case 'b':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsBoulder);
                                break;
                            case 's':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsSand);
                                break;
                            case 'k':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsBreakableRock);
                                break;
                            case 'd':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsUnbreakable);
                                break;
                            case 'A':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsSlime);
                                break;
                            case 'B':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsRedRock);
                                break;
                            case 'K':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsBlackRock);
                                break;
                            case 'g':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsGreenBlock);
                                break;
                            case 'y':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsYellowBlock);
                                break;
                            case 'r':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsRedBlock);
                                break;
                            case 'o':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsSupport);
                                break;
                            case 'q':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsQuadBlock);
                                break;
                            case 'R':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsRoad);
                                break;
                            case 'x':
                                data.ActionMatrix[y, x] = new ProgAction(ActionType.IsBox);
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
                            case '-':
                                i++;
                                if (text[i] == '|')
                                {
                                    data.ActionMatrix[y, x] = new ProgAction(ActionType.ReturnFunction);
                                }

                                break;
                            case '=':
                                i++;
                                if (text[i] == '|')
                                {
                                    data.ActionMatrix[y, x] = new ProgAction(ActionType.ReturnState);
                                }

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
                        else if (currentText.StartsWith("VB;"))
                        {
                            i += 2;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.BuildMilitaryBlock);
                        }
                        else if (currentText.StartsWith("DIGG;"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.MacrosDig);
                        }
                        else if (currentText.StartsWith("BUILD;"))
                        {
                            i += 5;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.MacrosBuild);
                        }
                        else if (currentText.StartsWith("HEAL;"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.MacrosHeal);
                        }
                        else if (currentText.StartsWith("MINE;"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.MacrosMine);
                        }
                        else if (currentText.StartsWith("FLIP;"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.Flip);
                        }
                        else if (currentText.StartsWith("OR"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.Or);
                        }
                        else if (currentText.StartsWith("AND"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.And);
                        }
                        else if (currentText.StartsWith("AUT+"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.EnableAutoDig);
                        }
                        else if (currentText.StartsWith("AUT-"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.DisableAutoDig);
                        }
                        else if (currentText.StartsWith("ARG+"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.EnableAgression);
                        }
                        else if (currentText.StartsWith("ARG-"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.DisableAgression);
                        }
                        else if (currentText.StartsWith("=hp-"))
                        {
                            i += 3;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.IsHpLower100);
                        }
                        else if (currentText.StartsWith("=hp50"))
                        {
                            i += 4;
                            data.ActionMatrix[y, x] = new ProgAction(ActionType.IsHpLower50);
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
        BuildBlock,
        BuildPillar,
        BuildRoad,
        BuildMilitaryBlock,
        Geology,
        Heal,
        NextRow,
        Label,
        GoTo,
        RunSub,
        RunFunction,
        RunState,
        RunOnRespawn,
        RunIfTrue,
        RunIfFalse,
        Return,
        ReturnFunction,
        ReturnState,
        Start,
        Stop,
        CheckUp,
        CheckLeft,
        CheckDown,
        CheckRight,
        CheckUpLeft,
        CheckUpRight,
        CheckDownLeft,
        CheckDownRight,
        CheckForward,
        CheckLeftRelative,
        CheckRightRelative,
        ShiftUp,
        ShiftLeft,
        ShiftDown,
        ShiftRight,
        ShiftForward,
        EnableAgression,
        DisableAgression,
        EnableAutoDig,
        DisableAutoDig,
        Flip,
        MacrosDig,
        MacrosBuild,
        MacrosHeal,
        MacrosMine,
        Or,
        And,
        IsHpLower100,
        IsHpLower50,
        IsNotEmpty,
        IsEmpty,
        IsFalling,
        IsCrystal,
        IsLivingCrystal,
        IsBoulder,
        IsSand,
        IsBreakableRock,
        IsUnbreakable,
        IsSlime,
        IsRedRock,
        IsBlackRock,
        IsGreenBlock,
        IsYellowBlock,
        IsRedBlock,
        IsSupport,
        IsQuadBlock,
        IsRoad,
        IsBox
    }
}