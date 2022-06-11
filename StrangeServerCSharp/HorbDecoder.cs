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
                if (p.win.StartsWith("!!"))
                {
                    if (p.win.StartsWith("!!settings"))
                    {
                        Sett((string)button, p);
                    }
                    if (p.win == "!!console")
                    {
                        Console((string)button, p);
                    }
                }
                else if (p.win == "box")
                {
                    box((string)button, p);
                }
                else if (p.win.StartsWith("market"))
                {
                    MarketO((string)button, p);
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
        public static void Exit(string text, Player p)
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
                    p.cpack.Open(p, p.win);
                    return;
                }
            }
        }
        public static void Sett(string text, Player p)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (text.StartsWith("<"))
                {
                    p.win = "!!settings." + text.Substring(1);
                    p.cpack.Open(p, p.win);
                    return;
                }
                if (text.StartsWith("change_settings"))
                {
                    string[] ar = text.Replace("#", ":").Split(':');
                    p.settings.isca = int.Parse(ar[2]);
                    p.settings.tsca = int.Parse(ar[4]);
                    p.settings.mous = int.Parse(ar[6]);
                    p.settings.pot = int.Parse(ar[8]);
                    p.settings.frc = int.Parse(ar[10]);
                    p.settings.ctrl = int.Parse(ar[12]);
                    p.settings.mof = int.Parse(ar[14]);
                    p.settings.SendSett(p);
                    BDClass.THIS.SaveChanges();
                }
                if (p.clanid != 0)
                {
                    return;
                }
                if (text.StartsWith("choose"))
                {
                    if (string.IsNullOrEmpty(Clan.finclan(int.Parse(text.Split(':')[1]) - 200).owner))
                    {
                        p.settings.Open(p.settings.winid + "." + text, p);
                        return;
                    }
                    else
                    {
                        p.settings.Open(p.settings.winid + ".create_сlan2", p);
                        System.Console.WriteLine("хуй2");
                    }
                    System.Console.WriteLine("хуй");
                    return;
                }
                else if (text.StartsWith("create_сlan2"))
                {
                    p.settings.Open(p.settings.winid + ".create_сlan2", p);
                    return;
                }
                else if (text.StartsWith("create_сlan3"))
                {
                    if (string.IsNullOrWhiteSpace(text.Split(':')[2]))
                    {
                        p.settings.Open(p.settings.winid + ".choose:" + (200 +int.Parse(text.Split(':')[1])), p);
                        return;
                    }
                    p.settings.Open(p.settings.winid + "." + text, p);
                    return;
                }
                else if (text.StartsWith("create_сlan4"))
                {
                    if (text.Split('#')[1].Length > 3 && string.IsNullOrWhiteSpace(text.Split('#')[1]))
                    {
                        Exit("exit", p);
                        return;
                    }
                    p.settings.Open(p.settings.winid + "." + text, p);
                    return;
                }
                else if (text.StartsWith("create_сlan5"))
                {
                    if (p.creds - 1000 >= 0)
                    {
                        p.creds -= 1000;
                        Clan.CreateClan(int.Parse(text.Split(':')[1].Split('#')[0]), text.Split('#')[1], text.Split('#')[2], p);
                        p.settings.Open(p.settings.winid + "." + text, p);
                    }
                    Exit("exit", p);
                    return;
                }
                else if (text.StartsWith("create_сlan"))
                {
                    p.settings.Open(p.settings.winid + ".create_сlan", p);
                    return;
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
                if (text.StartsWith("clans"))
                {
                    new Clans().Open(p, "!!clans");
                    return;
                }
            }
            p.ShowConsole();
        }
        public static void ConsClans(string text, Player p)
        {

        }
        private static void MarketO(string text, Player p)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            if (text.StartsWith("<"))
            {
                p.win = "market." + text.Substring(1);
                p.cpack.Open(p, p.win);
                return;
            }
            if (text.StartsWith("miscsellX"))
            {
                string sd = p.win.Split(':')[1];
                TrySell(10, sd, p);
                p.inventory.SendInv();
                p.cpack.Open(p, p.win);
                return;
            }
            else if (text.StartsWith("miscsell"))
            {
                string sd = p.win.Split(':')[1];
                TrySell(1, sd, p);
                p.inventory.SendInv();
                p.cpack.Open(p, p.win);
                return;
            }
            else if (text.StartsWith("miscbuyX"))
            {
                string sd = p.win.Split(':')[1];
                TryBuy(10, sd, p);
                p.inventory.SendInv();
                p.cpack.Open(p, p.win);
                return;
            }
            else if (text.StartsWith("miscbuy"))
            {
                string sd = p.win.Split(':')[1];
                TryBuy(1, sd, p);
                p.inventory.SendInv();
                p.cpack.Open(p, p.win);
                return;
            }
            else if (text.StartsWith("misc"))
            {
                p.win = "market." + text;
                p.cpack.Open(p, p.win);
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
        public static void TryBuy(int col, string itemid, Player p)
        {
            var cost = Market.findcost(itemid);
            long buy = long.Parse(cost.Substring(cost.IndexOf("!") + 2).Replace(":", ""));
            if (itemid == "8")
            {
                if (col == 1)
                {
                    if (p.money - buy >= 0)
                    {
                        p.money -= buy;
                        p.creds += 1;
                    }
                }
                else if (col == 10)
                {
                    if (p.money - (buy * 10) >= 0)
                    {
                        p.money -= (buy * 10);
                        p.creds += 10;
                    }
                }
                p.SendMoney();
                return;
            }
            else
            {
                if (col == 1)
                {
                    if (p.money - buy >= 0)
                    {
                        p.money -= buy;
                        p.inventory.items[int.Parse(itemid)].count++;
                    }
                }
                else if (col == 10)
                {
                    if (p.money - (buy * 10) >= 0)
                    {
                        p.money -= (buy * 10);
                        p.inventory.items[int.Parse(itemid)].count += 10;
                    }
                }
            }
            p.SendMoney();
        }
        public static void TrySell(int col, string itemid, Player p)
        {
            var cost = Market.findcost(itemid);
            long sell = long.Parse(cost.Substring(cost.IndexOf('^') + 2, cost.Substring(cost.IndexOf('^') + 2).IndexOf(';')));
            if (itemid == "8")
            {
                if (col == 1)
                {
                    if ((p.creds - 1) >= 0)
                    {
                        p.money += sell;
                        p.creds--;
                    }
                }
                if (col == 10)
                {
                    if ((p.creds - 10) >= 0)
                    {
                        p.money += (sell * 10);
                        p.creds -= 10;
                    }
                }
                p.SendMoney();
                return;
            }
            else
            {
                if (col == 1)
                {
                    if ((p.inventory.items[int.Parse(itemid)].count - 1) >= 0)
                    {
                        p.money += sell;
                        p.inventory.items[int.Parse(itemid)].count--;
                    }
                }
                if (col == 10)
                {
                    if ((p.inventory.items[int.Parse(itemid)].count - 10) >= 0)
                    {
                        p.money += (sell * 10);
                        p.inventory.items[int.Parse(itemid)].count -= 10;


                    }
                }
            }
            p.SendMoney();
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
