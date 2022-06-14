using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                    else if (p.win == "!!console")
                    {
                        Console((string)button, p);
                    }
                    else if (p.win.StartsWith("!!clans"))
                    {
                        ConsClans((string)button, p);
                    }
                    else if (p.win.StartsWith("!!clan"))
                    {
                        cl((string)button, p);
                    }
                }
                else if (p.win == "box")
                {
                    box((string)button, p);
                }
                else if (p.win == "gun")
                {
                    gun((string)button, p);
                }
                else if (p.win.StartsWith("market"))
                {
                    MarketO((string)button, p);
                }
                else if (p.win.StartsWith("resp"))
                {
                    Resp((string)button, p);
                }
                else if (p.win == "prog")
                {
                    Prog((string)button, p);
                }
                if (button != null && (string)button == "exit")
                {
                    Exit((string)button, p);
                }
            }
        }

        public static void Prog(string text, Player p)
        {
            using var db = new BDClass();
            var progs = db.players.Include(x => x.Progs).FirstOrDefault(x => x.id == p.id)?.Progs;
            if (text.StartsWith("open"))
            {
                var id = int.Parse(text.Split(":").Last());
                p.CurrentProgId = id;
                var prog = progs.FirstOrDefault(x => x.id == id);
                p.connection.Send("Gu", "");
                p.win = null;
                p.connection.Send("#P", JsonConvert.SerializeObject(prog));
            }
            else if (text == "create")
            {
                new HorbBuilder()
                    .SetTitle("ПРОГРАММАТОР")
                    .AddButton("СОЗДАТЬ ПРОГРАММУ", "create2:%I%")
                    .AddButton("ВЫЙТИ", "exit")
                    .SetText("Введите название вашей программы\n")
                    .AddIConsolePlace("Название программы...")
                    .AddTab("КАТАЛОГ", "cat")
                    .AddTab("СОБСТВЕННЫЕ ПРОГРАММЫ", "")
                    .AddCss("fixScroll=prg")
                    .Send("prog", p);
            }
            else if (text.StartsWith("create2"))
            {
                var title = text.Split(":").Last();
                var prog = new Prog
                {
                    title = title,
                    source = ""
                };
                db.progs.Add(prog);
                progs.Add(prog);
                db.SaveChanges();
                p.CurrentProgId = prog.id;
                p.connection.Send("Gu", "");
                p.win = null;
                p.connection.Send("#P", JsonConvert.SerializeObject(prog));
            }
            else if (text.StartsWith("copy"))
            {
                var id = int.Parse(text.Split(":").Last());
                var oldProg = progs.FirstOrDefault(x => x.id == id);
                var newTitle = oldProg.title;
                if (newTitle[newTitle.Length - 2] == '#')
                {
                    if (int.TryParse(newTitle.Last().ToString(), out var index))
                    {
                        newTitle = newTitle[..^1] + (index + 1);
                    }

                }
                else
                {
                    newTitle += " #2";
                }

                var prog = new Prog
                {
                    title = newTitle,
                    source = oldProg.source
                };
                db.progs.Add(prog);
                progs.Add(prog);
                db.SaveChanges();
                p.CurrentProgId = prog.id;
                p.connection.Send("Gu", "");
                p.win = null;
                p.connection.Send("#P", JsonConvert.SerializeObject(prog));
            }
            else if (text == "cat")
            {
                new HorbBuilder()
                    .SetTitle("ПРОГРАММАТОР")
                    .AddTab("КАТАЛОГ", "")
                    .AddTab("СОБСТВЕННЫЕ ПРОГРАММЫ", "prog")
                    .AddButton("ВЫЙТИ", "exit")
                    .SetText("")
                    .SetRichList("ПРЯМАЯ ДОБЫЧА&ЧИСТКА ПЕСКА&КАРЬЕР",
                        "3card",
                        "<color=#aaffaaff>ОТКРЫТЬ</color>&<color=#aaffaaff>ОТКРЫТЬ</color>&<color=#aaffaaff>ОТКРЫТЬ</color>",
                        "open:st&open:sn&open:cr",
                        "http://minesgame.ru/img/p_st.png%180%100&http://minesgame.ru/img/p_sn.png%180%100&http://minesgame.ru/img/p_cr.png%180%100",
                        "ТРАМВАЙ&ОТ СКАЛЫ ДО СКАЛЫ&ОКОЛОСКАЛ",
                        "3card",
                        "<color=#aaffaaff>ОТКРЫТЬ</color>&<color=#aaffaaff>ОТКРЫТЬ</color>&<color=#aaffaaff>ОТКРЫТЬ</color>",
                        "open:tr&open:ss&open:os",
                        "http://minesgame.ru/img/p_tr.png%180%100&http://minesgame.ru/img/p_ss.png%180%100&http://minesgame.ru/img/p_os.png%180%100")
                    .AddCss("fixScroll=prg")
                    .SetNoScroll()
                    .Send("prog", p);
            }
            else if (text == "prog")
            {
                var builder = new HorbBuilder()
                    .SetTitle("ПРОГРАММАТОР")
                    .AddButton("СОЗДАТЬ ПРОГРАММУ", "create")
                    .AddButton("ВЫЙТИ", "exit")
                    .AddTab("КАТАЛОГ", "cat")
                    .AddTab("СОБСТВЕННЫЕ ПРОГРАММЫ", "")
                    .AddCss("fixScroll=prg");
                for (var i = 0; i < progs.Count; i++)
                {
                    builder.AddListLine($"#{i + 1}. {progs[i].title}", "ОТКРЫТЬ", "open:" + progs[i].id);
                }

                builder.Send("prog", p);
            }
            else if (text == "rename")
            {
                new HorbBuilder()
                    .SetTitle("ПРОГРАММАТОР")
                    .AddButton("ПЕРЕИМЕНОВАТЬ", "rename:%I%")
                    .AddButton("ОТМЕНА", "exit")
                    .SetText("Введите новое название вашей программы\n")
                    .AddIConsolePlace("Новое название программы...")
                    .Send("prog", p);
            }
            else if (text.StartsWith("rename"))
            {
                var title = text.Split(":").Last();
                var prog = progs.FirstOrDefault(x => x.id == p.CurrentProgId);
                prog.title = title;
                db.SaveChanges();
                Prog("open:" + p.CurrentProgId, p);
            }
        }

        public static void gun(string text, Player p)
        {
            if (text.StartsWith("fill"))
            {
                var g = p.cpack as Gun;
                if (text.StartsWith("fill:b_max"))
                {
                    var have = p.crys.cry[5];
                    var needcry = g.crymax - g.cryinside;
                    if (needcry <= have)
                    {
                        if (p.crys.RemoveCrys(5, needcry))
                        {
                            g.cryinside += needcry;
                        }
                    }
                    else if (needcry >= have)
                    {
                        if (p.crys.RemoveCrys(5, have))
                        {
                            g.cryinside += (int)have;
                        }
                    }
                    g.off = g.cryinside > 0 ? 1 : 0;
                    g.UpdatePackVis();
                    p.cpack.Open(p, p.win);
                    return;
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
            if (p.settings == null)
            {
                var db = new BDClass();
                p.settings = db.settings.First(s => s.id == p.id);
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                if (text.StartsWith("<"))
                {
                    p.win = "!!settings." + text.Substring(1);
                    p.settings.Open(p.win, p);
                    return;
                }

                if (text.StartsWith("change_settings"))
                {
                    var ar = text.Replace("#", ":").Split(':');
                    p.settings.isca = int.Parse(ar[2]);
                    p.settings.tsca = int.Parse(ar[4]);
                    p.settings.mous = int.Parse(ar[6]);
                    p.settings.pot = int.Parse(ar[8]);
                    p.settings.frc = int.Parse(ar[10]);
                    p.settings.ctrl = int.Parse(ar[12]);
                    p.settings.mof = int.Parse(ar[14]);
                    p.settings.SendSett(p);
                    var db = new BDClass();
                    db.SaveChanges();
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
                        p.settings.Open(p.settings.winid + ".choose:" + (200 + int.Parse(text.Split(':')[1])), p);
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
                        Clan.CreateClan(int.Parse(text.Split(':')[1].Split('#')[0]), text.Split('#')[1],
                            text.Split('#')[2], p);
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
                    var t = text.Split(" ");
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

        public static void cl(string text, Player p)
        {
            var db = new BDClass();
            if (p.clanid == 0)
            {
                return;
            }
            if (text.StartsWith("clan"))
            {
                Clan.Open("!!clan", p);
                return;
            }
            else if (text.StartsWith("leave"))
            {
                var clan = db.clans.First(c => c.id == p.clanid);
                clan.Kick(p.id);
                Exit("exit", p);
                return;
            }
            else if (text.StartsWith("listrow_kick"))
            {
                Clan clan = null;
                var id = text.Split(':')[1];
                clan = db.clans.First(c => c.id == p.clanid);
                if (int.TryParse(text.Split(':')[1], out var t))
                {
                    clan.Kick(t);
                    Clan.Open("list", p);
                    return;
                }
                Clan.Open("list", p);
            }
            else if (text.StartsWith("listrow"))
            {
                Clan.Open(text, p);
            }
            else if (text.StartsWith("list"))
            {
                Clan.Open(text, p);
            }

            if (text.StartsWith("reqs"))
            {
                Clan.Open(text, p);
            }
            else if (text.StartsWith("req"))
            {
                Clan clan = null;
                var id = text.Split(':')[2];
                try
                {
                    clan = db.clans.First(c => c.id.ToString() == id);
                }
                catch (Exception)
                {
                }

                if (clan == null)
                {
                    return;
                }

                if (int.TryParse(text.Split(':')[1], out var t))
                {
                    if (!clan.reqs.Contains(t))
                    {
                        Clan.Open("!!clan", p);
                        return;
                    }

                    clan.AddMember(t);
                    clan.RemoveReq(t);
                    Clan.Open("!!clan", p);
                    return;
                }
                else
                {
                    clan.RemoveReq(t);
                    return;
                }

                Clan.Open("!!clan", p);
            }
        }

        public static void ConsClans(string text, Player p)
        {
            var db = new BDClass();
            if (text.StartsWith("clans"))
            {
                new Clans().Open(p, "!!clans");
            }
            else if (text.StartsWith("clan"))
            {
                new Clans().Open(p, "!!clans." + text);
            }

            if (p.clanid != 0)
            {
                return;
            }
            else if (text.StartsWith("recruit_in"))
            {
                var id = text.Split(':')[1];
                Clan clan = null;
                try
                {
                    clan = db.clans.First(c => c.id.ToString() == id);
                }
                catch (Exception)
                {
                }

                if (clan == null)
                {
                    return;
                }

                clan.AddReq(p.id);
                new Clans().Open(p, "!!clans");
            }
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
                var sd = p.win.Split(':')[1];
                TrySell(10, sd, p);
                p.inventory.SendInv();
                p.cpack.Open(p, p.win);
                return;
            }
            else if (text.StartsWith("miscsell"))
            {
                var sd = p.win.Split(':')[1];
                TrySell(1, sd, p);
                p.inventory.SendInv();
                p.cpack.Open(p, p.win);
                return;
            }
            else if (text.StartsWith("miscbuyX"))
            {
                var sd = p.win.Split(':')[1];
                TryBuy(10, sd, p);
                p.inventory.SendInv();
                p.cpack.Open(p, p.win);
                return;
            }
            else if (text.StartsWith("miscbuy"))
            {
                var sd = p.win.Split(':')[1];
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

                    for (var i = 0; i < 6; i++)
                    {
                        var remcry = p.crys.cry[i];
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

                var cry = text.Split(":");
                for (var i = 0; i < 6; i++)
                {
                    var remcry = long.Parse(cry[i + 1]);
                    if (p.crys.RemoveCrys(i, remcry))
                    {
                        p.money += (remcry * World.costs[i]);
                    }
                }

                p.SendMoney();
                p.cpack.Open(p, p.cpack.winid);
                return;
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

                var cry = text.Split(":");
                for (var i = 0; i < 6; i++)
                {
                    var buycry = long.Parse(cry[i + 1]);
                    if (!p.crys.BuyCrys(i, buycry))
                    {
                        p.SendMoney();
                        p.cpack.Open(p, p.win);
                        return;
                    }
                }

                p.SendMoney();
                p.cpack.Open(p, p.win);
                return;
            }
            p.SendMoney();
            p.cpack.Open(p, text);
        }

        public static void TryBuy(int col, string itemid, Player p)
        {
            var cost = Market.findcost(itemid);
            var buy = long.Parse(cost.Substring(cost.IndexOf("!") + 2).Replace(":", ""));
            if (itemid == "8")
            {
                if (col == 1)
                {
                    if (p.money - (buy * 10) >= 0)
                    {
                        p.money -= buy;
                        p.creds += 10;
                    }
                }
                else if (col == 10)
                {
                    if (p.money - (buy * 100) >= 0)
                    {
                        p.money -= (buy * 100);
                        p.creds += 100;
                    }
                }

                p.SendMoney();
                return;
            }
            else
            {
                if (col == 1)
                {
                    if (p.money - (buy) >= 0)
                    {
                        p.money -= (buy);
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
            var sell = long.Parse(cost.Substring(cost.IndexOf('^') + 2,
                cost.Substring(cost.IndexOf('^') + 2).IndexOf(';')));
            if (itemid == "8")
            {
                if (col == 1)
                {
                    if ((p.creds - 10) >= 0)
                    {
                        p.money += sell * 10;
                        p.creds -= 10;
                    }
                }

                if (col == 10)
                {
                    if ((p.creds - 100) >= 0)
                    {
                        p.money += (sell * 100);
                        p.creds -= 100;
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
                if (!(World.THIS.GetCellConst(x, y).is_empty && World.THIS.GetCellConst(x, y).can_build_over))
                {
                    return;
                }

                for (var i = 0; i < 6; i++)
                {
                    long remcry = long.Parse(cry[i + 1]);
                    box[i] = remcry;
                }
               
                Box.BuildBox(x, y, box,p);
                p.connection.Send("Gu", "");
                p.win = "";
            }
        }
    }
}