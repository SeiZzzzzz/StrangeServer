using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace StrangeServerCSharp
{
    public static class HorbDecoder
    {
        public static void Decode(string c, Player p)
        {
            JObject jo = null;
            try
            {
                jo = JObject.Parse(c);
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return;
            }
            var button = jo["b"];
            if (button != null)
            {
                if (p.win == "")
                {
                    return;
                }
                if (p.win == "box")
                {
                    box((string)button, p);
                }
                else if (p.win == "console")
                {
                    Console((string)button, p);
                }
                else if (p.win.StartsWith("market"))
                {
                    Market((string)button, p);
                }
                else if (p.win.StartsWith("resp"))
                {
                    Resp((string)button, p);
                }

                if (button != null && (string)button == "exit")
                {
                    Exit((string)button, p);
                }
            }
        }
        private static void Exit(string text, Player p)
        {
            if (text == "exit")
            {
                p.connection.Send("Gu", "");
                p.win = "";
                p.cpack = null;
            }
        }

        private static void Resp(string text, Player p)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            if (text.StartsWith("tab"))
            {
                p.win = "resp." + text;
                p.cpack.Open(p, p.win);
                return;
            }
            if (text.StartsWith("bind"))
            {
                p.resp = (Resp)p.cpack;
                p.cpack.Open(p, p.win);
                return;
            }
            if (p.id != p.cpack.ownerid)
            {
                return;
            }
            if (text.StartsWith("fill"))
            {
                var resp = p.cpack as Resp;
                if (text.StartsWith("fill:b_max"))
                    {
                    resp.cryinside = resp.crymax;
                    resp.off = 1;
                    resp.UpdatePackVis();
                }
            }
        }
        public static void Console(string text, Player p)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                p.AddConsoleLine(text);
                if (text.StartsWith("newnick"))
                {
                    string[] t = text.Split(" ");
                    if (!string.IsNullOrWhiteSpace(t[1]))
                    {
                        if (BDClass.NickAvl(t[1]))
                        {
                            p.AddConsoleLine("недоступен");
                        }
                        else
                        {
                            p.SetNick(text.Split(" ")[1]);
                            p.AddConsoleLine("новый ник:" + p.name);
                            BDClass.THIS.SaveChanges();
                        }
                    }
                }
                if (text.StartsWith("setcell"))
                {
                    if (!string.IsNullOrWhiteSpace(text.Split(" ")[1]))
                    {
                        p.cellg = byte.Parse(text.Split(" ")[1]);
                        p.AddConsoleLine("cell:" + p.cellg);
                    }
                }
            }
            p.ShowConsole();
        }
        private static void Market(string text, Player p)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            if (text.StartsWith("tab"))
            {
                p.win = "market." + text;
                p.cpack.Open(p, p.win);
                return;
            }
            if (p.win == "market.tab_sell")
            {
                if (!text.StartsWith("sell"))
                {
                    return;
                }
                if (text.StartsWith("sellall"))
                {
                    if (p.cpack == null)
                    {

                        Exit("exit", p);
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        long remcry = p.crys.cry[i];
                        if (p.crys.RemoveCrys(i, remcry))
                        {
                            p.money += (remcry * World.costs[i]);
                        }
                    }
                    p.SendMoney();
                    p.cpack.Open(p, p.win);
                    Exit("exit", p);
                    return;

                }
                if (p.cpack == null)
                {
                    Exit("exit", p);
                }
                string[] cry = text.Split(":");
                for (int i = 0; i < 6; i++)
                {
                    long remcry = long.Parse(cry[i + 1]);
                    if (p.crys.RemoveCrys(i, remcry))
                    {
                        p.money += (remcry * World.costs[i]);
                    }
                }
                p.SendMoney();
                p.cpack.Open(p, p.win);

            }
            else if (p.win == "market.tab_buy")
            {
                if (!text.StartsWith("buy"))
                {
                    return;
                }
                if (p.cpack == null)
                {
                    Exit("exit", p);
                }
                string[] cry = text.Split(":");
                for (int i = 0; i < 6; i++)
                {
                    long buycry = long.Parse(cry[i + 1]);
                    if (!p.crys.BuyCrys(i, buycry))
                    {
                        p.SendMoney();
                        p.cpack.Open(p, p.win);
                        return;
                    }
                }
                p.SendMoney();
                p.cpack.Open(p, p.win);
            }
        }
        private static void box(string text, Player p)
        {
            if (text.StartsWith("dropbox"))
            {
                string[] cry = text.Split(":");
                long[] box = new long[6];
                uint x = (uint)p.GetDirCord().X;
                uint y = (uint)p.GetDirCord().Y;
                if (!World.THIS.ValidCoord(x, y) || !(World.THIS.GetCellConst(x, y).is_empty && World.THIS.GetCellConst(x, y).can_build_over))
                {
                    return;
                }
                for (int i = 0; i < 6; i++)
                {
                    long remcry = long.Parse(cry[i + 1]);
                    if (p.crys.RemoveCrys(i, remcry))
                    {
                        box[i] = remcry;
                    }
                }
                if (box.Sum() <= 0)
                {
                    return;
                }
                Box.BuildBox(x, y, box);
                p.connection.Send("Gu", "");
                p.win = "";
            }
        }
    }
}
