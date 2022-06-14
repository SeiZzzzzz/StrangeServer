using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StrangeServerCSharp
{
    public abstract class Building
    {
        public int Id { get; set; }

        protected Building()
        {
        }

        [NotMapped] public abstract string winid { get; set; }
        public abstract void Open(Player p, string tab);
        public abstract void Rebild();

        public static void AddPack()
        {
            using var db = new BDClass();
            World.packlist = db.packs.ToList();
        }

        public abstract bool CanOpen(Player p);
        public abstract void Remove();
        public abstract void Update(string type);

        public void UpdatePackVis()
        {
            var v = World.THIS.GetChunkPosByCoords(x, y);
            var chunkx = (int)v.X;
            var chunky = (int)v.Y;
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((chunkx + xxx) >= 0 && (chunky + yyy) >= 0) && ((chunkx + xxx) < XServer.THIS.chunkscx &&
                                                                         (chunky + yyy) < XServer.THIS.chunkscy))
                    {
                        var ch = Chunk.chunks[chunkx + xxx, chunky + yyy];
                        foreach (var p in ch.bots)
                        {
                            XServer.players[p.Key].connection.ClearPack(this.x, this.y);
                            XServer.players[p.Key].connection.AddPack(this.type, this.x, this.y, this.cid, this.off);
                        }
                    }
                }
            }
        }

        public Pack GetShpack
        {
            get { return new Pack { cid = this.cid, off = this.off, type = this.type, x = this.x, y = this.y }; }
        }

        public int cid { get; set; }
        public int off { get; set; }
        public char type { get; set; }
        public int hp { get; set; }
        public uint x { get; set; }

        public uint y { get; set; }
        public int ownerid { get; set; }
    }

    public struct Pack
    {
        public int cid;
        public int off;
        public char type;
        public uint x;
        public uint y;
    }

    public class Market : Building
    {
        public override void Remove()
        {
            World.THIS.SetCell(x + 3, y, 35);
            World.THIS.SetCell(x + 4, y, 35);
            World.THIS.SetCell(x - 3, y, 35);
            World.THIS.SetCell(x - 4, y, 35);


            //y
                World.THIS.SetCell(x, y + 3, 35);
                World.THIS.SetCell(x, y + 4, 35);

                World.THIS.SetCell(x, y - 3, 35);

                World.THIS.SetCell(x, y - 4, 35);
            //osn
            World.THIS.SetCell(x, y, 32);
            World.THIS.SetCell(x + 1, y, 32);
            World.THIS.SetCell(x + 2, y, 32);
            World.THIS.SetCell(x - 2, y, 32);
            World.THIS.SetCell(x - 1, y, 32);
            World.THIS.SetCell(x, y + 1, 32);
            World.THIS.SetCell(x, y + 2, 32);
            World.THIS.SetCell(x, y - 2, 32);
            World.THIS.SetCell(x, y - 1, 32);
            //
            World.THIS.SetCell(x - 1, y - 1, 32);
            World.THIS.SetCell(x + 1, y - 1, 32);
            World.THIS.SetCell(x - 1, y + 1, 32);
            World.THIS.SetCell(x + 1, y + 1, 32);
            //
            World.THIS.SetCell(x - 1, y - 2, 32);
            World.THIS.SetCell(x - 2, y - 1, 32);
            World.THIS.SetCell(x - 2, y - 2, 32);
            //
            World.THIS.SetCell(x - 1, y + 2, 32);
            World.THIS.SetCell(x - 2, y + 1, 32);
            World.THIS.SetCell(x - 2, y + 2, 38);
            //
            World.THIS.SetCell(x + 1, y - 2, 32);
            World.THIS.SetCell(x + 2, y - 1, 32);
            World.THIS.SetCell(x + 2, y - 2, 32);
            //
            World.THIS.SetCell(x + 1, y + 2, 32);
            World.THIS.SetCell(x + 2, y + 1, 32);
            World.THIS.SetCell(x + 2, y + 2, 32);
            using var db = new BDClass();
            db.markets.Remove(this);
            db.SaveChanges();
            this.UpdatePackVis();
            World.ClearPack(x, y);
        }

        public override void Update(string type)
        {
        }

        public override bool CanOpen(Player p)
        {
            return true;
        }

        public static string[] mpcosts = new string[]
        {
            "8:^$1000000;!$1000000:",
            "1:^$0;!$0:",
            "2:^$0;!$0:",
            "3:^$0;!$0:",
            "4:^$0;!$0:",
            "5:^$500000;!$500000:",
            "6:^$20000000;!$20000000:",
            "7:^$1000000;!$1000000:",
            "9:^$0;!$0:",
            "10:^$0;!$0:",
            "11:^$0;!$0:",
            "12:^$0;!$0:",
            "13:^$0;!$0:",
            "14:^$0;!$0:",
            "15:^$0;!$0:",
            "16:^$0;!$0:",
            "17:^$0;!$0:",
            "18:^$0;!$0:",
            "19:^$0;!$0:",
            "20:^$0;!$0:",
            "21:^$0;!$0:",
            "22:^$0;!$0:",
            "23:^$0;!$0:",
            "24:^$0;!$0:",
            "25:^$0;!$0:",
            "26:^$100000000;!$100000000:",
            "27:^$0;!$0:",
            "28:^$20000000;!$20000000:",
            "29:^$0;!$0:",
            "30:^$0;!$0:",
            "34:^$0;!$0:",
            "35:^$0;!$0:",
            "36:^$0;!$0:",
            "37:^$0;!$0:",
            "38:^$0;!$0:",
            "39:^$0;!$0:",
            "40:^$0;!$0:",
            "41:^$0;!$0:",
            "42:^$0;!$0:",
            "43:^$0;!$0:",
            "44:^$0;!$0:",
            "45:^$0;!$0:",
            "46:^$0;!$0:",
            "47:^$0;!$0:",
            "48:^$0;!$0"
        };

        public Market()
        {
            winid = "market.tab_sell";
        }

        public override string winid { get; set; }

        public override void Rebild()
        {

                World.THIS.SetCell(x + 3, y, 35);
                World.THIS.GetCellConst(x + 3, y).is_destructible = false;
                World.THIS.GetCellConst(x + 3, y).HP = -2;
                World.THIS.SetCell(x + 4, y, 35);
                World.THIS.GetCellConst(x + 4, y).is_destructible = false;
                World.THIS.GetCellConst(x + 4, y).HP = -2;
                World.THIS.SetCell(x - 3, y, 35);
                World.THIS.GetCellConst(x - 3, y).is_destructible = false;
                World.THIS.GetCellConst(x - 3, y).HP = -2;
                World.THIS.SetCell(x - 4, y, 35);
                World.THIS.GetCellConst(x - 4, y).is_destructible = false;
                World.THIS.GetCellConst(x - 4, y).HP = -2;

            //y
            if (World.THIS.GetCellConst(x, y + 3) != null)
            {
                World.THIS.SetCell(x, y + 3, 35);
                World.THIS.GetCellConst(x, y + 3).is_destructible = false;
                World.THIS.GetCellConst(x, y + 3).HP = -2;
            }

            if (World.THIS.GetCellConst(x, y + 4) != null)
            {
                World.THIS.SetCell(x, y + 4, 35);
                World.THIS.GetCellConst(x, y + 4).is_destructible = false;
                World.THIS.GetCellConst(x, y + 4).HP = -2;
            }

            if (World.THIS.GetCellConst(x, y - 3) != null)
            {
                World.THIS.SetCell(x, y - 3, 35);
                World.THIS.GetCellConst(x, y - 3).is_destructible = false;
                World.THIS.GetCellConst(x, y - 3).HP = -2;
            }

            if (World.THIS.GetCellConst(x, y - 4) != null)
            {
                World.THIS.SetCell(x, y - 4, 35);
                World.THIS.GetCellConst(x, y - 4).is_destructible = false;
                World.THIS.GetCellConst(x, y - 4).HP = -2;
            }

            //osn
            World.THIS.SetCell(x, y, 37);
            World.THIS.SetCell(x + 1, y, 37);
            World.THIS.SetCell(x + 2, y, 37);
            World.THIS.SetCell(x - 2, y, 37);
            World.THIS.SetCell(x - 1, y, 37);
            World.THIS.SetCell(x, y + 1, 37);
            World.THIS.SetCell(x, y + 2, 37);
            World.THIS.SetCell(x, y - 2, 37);
            World.THIS.SetCell(x, y - 1, 37);
            //
            World.THIS.SetCell(x - 1, y - 1, 106);
            World.THIS.SetCell(x + 1, y - 1, 106);
            World.THIS.SetCell(x - 1, y + 1, 106);
            World.THIS.SetCell(x + 1, y + 1, 106);
            //
            World.THIS.SetCell(x - 1, y - 2, 106);
            World.THIS.SetCell(x - 2, y - 1, 106);
            World.THIS.SetCell(x - 2, y - 2, 38);
            //
            World.THIS.SetCell(x - 1, y + 2, 106);
            World.THIS.SetCell(x - 2, y + 1, 106);
            World.THIS.SetCell(x - 2, y + 2, 38);
            //
            World.THIS.SetCell(x + 1, y - 2, 106);
            World.THIS.SetCell(x + 2, y - 1, 106);
            World.THIS.SetCell(x + 2, y - 2, 38);
            //
            World.THIS.SetCell(x + 1, y + 2, 106);
            World.THIS.SetCell(x + 2, y + 1, 106);
            World.THIS.SetCell(x + 2, y + 2, 38);
        }

        public static Market Build(uint x, uint y, Player owner)
        {
            if (!checkcan(x, y, owner))
            {
                return null;
            }

            var m = new Market() { ownerid = owner.id, x = x, y = y, type = 'M' };
            m.Rebild();
            var v = World.THIS.GetChunkPosByCoords(x, y);
            Chunk.chunks[(uint)v.X, (uint)v.Y].AddPack(x, y);
            return m;
        }

        public static async void AsyncAction(int msdelay, Action act)
        {
            await Task.Run(delegate()
            {
                System.Threading.Thread.Sleep(msdelay);
                act();
            });
        }

        public static bool checkcan(uint px, uint py, Player p)
        {
            if (World.ongun[px + py * World.height] != null)
            {
                if (World.ongun[px + py * World.height].Count > 0)
                {
                    if (World.ongun[px + py * World.height].First() != p.clanid ||
                        World.ongun[px + py * World.height].Count > 1)
                    {
                        var dat = System.Text.Encoding.UTF8.GetBytes("блок под пуфкой");

                        p.connection.SendLocalChat(dat.Length, 0, px, py, dat);
                        return false;
                    }
                }
            }

            var valid = 0;
            for (var x = -4; x <= 4; x++)
            {
                for (var y = -4; y <= 4; y++)
                {
                    if (!World.THIS.ValidCoordForPlace((uint)(px + x), (uint)(py + y)))
                    {
                        valid++;
                    }
                    else
                    {
                        var c = World.THIS.GetCellConst((uint)(px + x), (uint)(py + y));
                        if (!(c.is_empty && c.can_build_over))
                        {
                            p.connection.AddFX(0, (uint)(px + x), (uint)(py + y));
                            valid++;
                        }
                    }
                }
            }

            if (valid > 0)
            {
                return false;
            }

            return true;
        }

        public static string findcost(string id)
        {
            return mpcosts.First(i => i.Split(':')[0] == id);
        }

        public override void Open(Player p, string tab)
        {
            var builder = new HorbBuilder();
            if (p.win == "")
            {
                return;
            }

            using var db = new BDClass();
            if (tab == this.winid)
            {
                var cry = p.crys.cry;

                builder.AddTab("ПРОДАТЬ КРИ", "")
                    .AddTab("КУПИТЬ КРИ", "tab_buy")
                    .AddTab("КРЕДИТЫ И ПРОЧЕЕ", "tab_misc")
                    .AddTab("ОРДЕРЫ", "tab_mkt")
                    .AddTab("АУКЦИОН", "tab_auc")
                    .SetTitle("МАРКЕТ")
                    .AddTextLine("Используйте полосы прокрутки, чтобы выбрать сколько продать кристаллов")
                    .AddTextLine("")
                    .AddCrysRight("будет продано")
                    .AddCrysLeft("  останется")
                    .AddCrysLines(World.costs.Select((cost, index) => new CrysLine
                        { leftMin = 0, rightMin = 0, d = cry[index], value = 0, descText = "x " + cost }).ToArray())
                    .AddButton("ПРОДАТЬ", "@sell:%M%")
                    .AddButton("ПРОДАТЬ ВСЕ", "@sellall")
                    .AddButton("ВЫЙТИ", "exit");
                if (p == db.players.First(p => p.id == ownerid))
                {
                    builder.Admin();
                }
            }
            else if (tab == "market.tab_buy")
            {
                builder.AddTab("ПРОДАТЬ КРИ", "tab_sell")
                    .AddTab("КУПИТЬ КРИ", "")
                    .AddTab("КРЕДИТЫ И ПРОЧЕЕ", "tab_misc")
                    .AddTab("ОРДЕРЫ", "tab_mkt")
                    .AddTab("АУКЦИОН", "tab_auc")
                    .SetTitle("МАРКЕТ")
                    .SetText("<color=#f88></color>")
                    .AddCrysRight("сколько покупаем")
                    .AddCrysLeft("станет в грузе")
                    .AddCrysBuy()
                    .AddCrysLines(World.costsbuy.Select((cost, index) => new CrysLine
                        { leftMin = 0, rightMin = 0, d = p.money / cost, value = 0, descText = "x " + cost }).ToArray())
                    .AddButton("КУПИТЬ", "buy:%M%")
                    .AddButton("ВЫЙТИ", "exit");
                if (p == db.players.First(p => p.id == ownerid))
                {
                    builder.Admin();
                }
            }
            else if (tab == "market.tab_misc")
            {
                builder.AddTab("ПРОДАТЬ КРИ", "tab_sell")
                    .AddTab("КУПИТЬ КРИ", "tab_buy")
                    .AddTab("КРЕДИТЫ И ПРОЧЕЕ", "")
                    .AddTab("ОРДЕРЫ", "tab_mkt")
                    .AddTab("АУКЦИОН", "tab_auc")
                    .SetTitle("МАРКЕТ")
                    .SetText("##")
                    .AddButton("ПРОДЛИТЬ ЛИЦЕНЗИЮ", "maker")
                    .AddButton("ВЫЙТИ", "exit")
                    .AddCss(68, 596, 0, "misc")
                    .SetInv(InvGen.getmarket(mpcosts));
            }
            else if (tab.StartsWith("market.misc"))
            {
                var sd = tab.Split(':')[1];
                var cost = mpcosts.First(i => i.Split(':')[0] == sd);
                var sell = cost.Substring(cost.IndexOf('^') + 2, cost.Substring(cost.IndexOf('^') + 2).IndexOf(';'));
                var buy = cost.Substring(cost.IndexOf("!") + 2).Replace(":", "");
                builder.AddTab("ПРОДАТЬ КРИ", "tab_sell")
                    .AddTab("КУПИТЬ КРИ", "tab_buy")
                    .AddTab("КРЕДИТЫ И ПРОЧЕЕ", "")
                    .AddTab("ОРДЕРЫ", "tab_mkt")
                    .AddTab("АУКЦИОН", "tab_auc")
                    .SetText("\n ")
                    .SetTitle("МАРКЕТ")
                    .AddButton("НАЗАД", "<tab_misc")
                    .AddButton("ВЫЙТИ", "exit")
                    .AddListLine($"ПРОДАТЬ ПО ЛУЧШЕЙ ЦЕНЕ: 1 ЗА ${makeK(sell)}", "ПРОДАТЬ 1", "miscsell")
                    .AddListLine($"                       10 ЗА ${makeK((int.Parse(sell) * 10).ToString())}", "ПРОДАТЬ 10", "miscsellX")
                    .AddListLine($"КУПИТЬ ПО ЛУЧШЕЙ ЦЕНЕ: 1 ЗА ${makeK(buy)}", "КУПИТЬ 1", "miscbuy")
                    .AddListLine($"                       10 ЗА ${makeK((int.Parse(buy) * 10).ToString())}", "КУПИТЬ 10", "miscbuyX")
                    .AddMarketCard(sd, " ");
            }

            builder.Send(p.win, p);
        }
        public static string makeK(string text)
        {
            text = Regex.Replace(text, "000", "k", RegexOptions.RightToLeft);
            return text;
        }
        public static string mtext =
            "choose:\n[ENTER] = установить рынок\n[ESC] = отмена:3:4:4:9:9:000000000000000000001101100001101100000000000001101100001101100000000000000000000 ";
    }

    public class Clans : Building
    {
        public override void Remove()
        {
        }

        public override bool CanOpen(Player p)
        {
            return true;
        }

        public override void Update(string type)
        {
        }

        public Clans()
        {
            winid = "clans";
        }

        public override string winid { get; set; }

        public override void Open(Player p, string tab)
        {
            var builder = new HorbBuilder();
            if (tab.StartsWith("!!clans.clan"))
            {
                var id = tab.Split(':')[1];
                builder.SetTitle("КЛАНЫ")
                    .SetText(
                        "@@\n<color=#88ff88ff>Прием в клан открыт.</color> Условия для подачи заявки:\n\n<color=#88ff88ff>Подать заявку может кто угодно</color>\n");
                if (p.clanid == 0)
                {
                    builder.AddButton("ПОДАТЬ ЗАЯВКУ", $"@recruit_in:{id}");
                }

                Clan clan = null;
                try
                {
                    using var db = new BDClass();
                    clan = db.clans.First(c => c.id.ToString() == id);
                }
                catch (Exception)
                {
                }

                if (clan == null)
                {
                    return;
                }

                builder.AddCard("c", id,
                        $"<color=white>{clan.name}</color>\nУчастники: <color=white>{clan.GetMemberList().Count + 1}</color>")
                    .AddButton("НАЗАД", "<" + winid);
            }
            else if (tab.StartsWith("!!" + winid))
            {
                builder.SetTitle("КЛАНЫ")
                    .AddClanList()
                    .SetText("@@Кланы шахт. Кликните на клан для подробной информации\n");
                using var db = new BDClass();
                foreach (var cl in db.clans.Where(clan => !string.IsNullOrEmpty(clan.owner)))
                {
                    builder.AddClanListLine(cl.id.ToString(), $"<color=white>{cl.name}</color>  [{cl.abr}]",
                        "<color=#88ff88ff>прием открыт</color>", "clan:" + cl.id);
                }

                builder.AddButton("ВЫХОД", "exit");
            }

            builder.Send(tab, p);
        }

        public override void Rebild()
        {
            throw new NotImplementedException();
        }
    }

    public class Gun : Building
    {
        public override void Remove()
        {
            using var db = new BDClass();
            World.THIS.SetCell(x, y, 32);
            World.cells[x + y * World.height].HP = 1;
            World.THIS.SetCell(x + 1, y + 1, 32);
            World.THIS.SetCell(x - 1, y + 1, 32);
            World.THIS.SetCell(x + 1, y - 1, 32);
            World.THIS.SetCell(x - 1, y - 1, 32);
            //roads
            World.THIS.SetCell(x + 1, y, 35);
            World.THIS.SetCell(x + 2, y, 35);
            World.THIS.SetCell(x - 1, y, 35);
            World.THIS.SetCell(x - 2, y, 35);
            //
            World.THIS.SetCell(x, y + 1, 35);
            World.THIS.SetCell(x, y + 2, 35);
            World.THIS.SetCell(x, y - 1, 35);
            World.THIS.SetCell(x, y - 2, 35);
            try
            {
                Box.BuildBox(x, y, new long[] { 0, 0, 0, 0, 0, cryinside }, null);
            }
            catch (Exception e) { }
            db.guns.Remove(this);
            db.SaveChanges();
            this.UpdatePackVis();
            World.ClearPack(x, y);
            World.THIS.OnGunDel(x, y, this.cid);
        }

        public override bool CanOpen(Player p)
        {
            return p.clanid == cid;
        }

        public override void Update(string type)
        {
            
                if (type == "gun")
                {
                    if (cryinside > 0)
                    {
                        World.GUN(this.x, this.y, this.cid, this);
                    }
                    else
                    {
                        World.THIS.OnGunDel(x, y, this.cid);
                    }
                }
           
        }

        public bool OnShot(int cost)
        {
            if (cryinside < 0)
            {
                cryinside = 0;
                return false;
            }
            if ((cryinside - cost <= 0) && off == 1)
            {
                off = 0;
                UpdatePackVis();
            }

            cryinside -= cost;
            return true;
        }

        public override string winid { get; set; }
        public int cryinside { get; set; }
        public int crymax { get; set; }

        public static string gtext =
            "choose:\n[ENTER] = установить пушку\n[ESC] = отмена:2:20:20:41:41:0000000000000000000010000000000000000000000000000000000111111011111100000000000000000000000000110000000000000110000000000000000000000110000000000000000011000000000000000000110000000000000000000001100000000000000010000000000000000000000000100000000000001000000000000000000000000000100000000000100000000000000000000000000000100000000010000000000000000000000000000000100000000100000000000000000000000000000001000000010000000000000000000000000000000001000000100000000000000000000000000000000010000010000000000000000000000000000000000010000100000000000000000000000000000000000100010000000000000000000000000000000000000100100000000000000000000000000000000000001001000000000000000000000000000000000000010010000000000000000000000000000000000000100100000000000000000000000000000000000001001000000000000000001010000000000000000010100000000000000000000000000000000000000010100000000000000000101000000000000000001001000000000000000000000000000000000000010010000000000000000000000000000000000000100100000000000000000000000000000000000001001000000000000000000000000000000000000010010000000000000000000000000000000000000100010000000000000000000000000000000000010000100000000000000000000000000000000000100000100000000000000000000000000000000010000001000000000000000000000000000000000100000001000000000000000000000000000000010000000010000000000000000000000000000000100000000010000000000000000000000000000010000000000010000000000000000000000000001000000000000010000000000000000000000000100000000000000011000000000000000000000110000000000000000001100000000000000000110000000000000000000000110000000000000110000000000000000000000000011111101111110000000000000000000000000000000000100000000000000000000 ";

        public Gun()
        {
            winid = "gun";
            crymax = 10000;
            cryinside = 1000;
            type = 'G';
            off = 1;
        }

        public static bool checkcan(uint px, uint py, Player p)
        {
            if (World.ongun[px + py * World.height] != null)
            {
                if (World.ongun[px + py * World.height].Count > 0)
                {
                    if (World.ongun[px + py * World.height].First() != p.clanid ||
                        World.ongun[px + py * World.height].Count > 1)
                    {
                        var dat = System.Text.Encoding.UTF8.GetBytes("блок под пуфкой");

                        p.connection.SendLocalChat(dat.Length, 0, px, py, dat);
                        return false;
                    }
                }
            }

            var valid = 0;
            for (var x = -2; x <= 2; x++)
            {
                for (var y = -2; y <= 2; y++)
                {
                    if (!World.THIS.ValidCoordForPlace((uint)(px + x), (uint)(py + y)))
                    {
                        valid++;
                    }
                    else
                    {
                        var c = World.THIS.GetCellConst((uint)(px + x), (uint)(py + y));
                        if (!(c.is_empty && c.can_build_over))
                        {
                            p.connection.AddFX(0, (uint)(px + x), (uint)(py + y));
                            valid++;
                        }
                    }
                }
            }

            if (valid > 0)
            {
                return false;
            }

            return true;
        }

        public override void Open(Player p, string tab)
        {
            if (p.clanid != cid)
            {
                return;
            }
            var l = new string[0];
            l = l.Concat(RichListGenerator
                .Fill("Заряд", cryinside, crymax, 5, "fill:b_100", "fill:b_1000", "fill:b_max").ToList()).ToArray();
            var builder = new HorbBuilder()
                .SetTitle("ПУФКА")
                .AddButton("ВЫХОД", "exit")
                .SetRichList(l)
                .Send(this.winid, p);
        }

        public static Gun Build(uint x, uint y, Player p)
        {
            if (p.clanid == 0 || !checkcan(x, y, p))
            {
                return null;
            }

            rb(x, y);
            var g = new Gun() { cid = p.clanid, ownerid = p.id, x = x, y = y };
            var v = World.THIS.GetChunkPosByCoords(x, y);
            Chunk.chunks[(uint)v.X, (uint)v.Y].AddPack(x, y);
            using var db = new BDClass();
            db.guns.Add(g);
            db.SaveChanges();
            return g;
        }

        public static void rb(uint x, uint y)
        {
            World.THIS.SetCell(x, y, 32);
            World.THIS.GetCellConst(x, y).is_destructible = false;
            World.THIS.GetCellConst(x, y).HP = -2;
            //walls
            World.THIS.SetCell(x + 1, y + 1, 106);
            World.THIS.SetCell(x - 1, y + 1, 106);
            World.THIS.SetCell(x + 1, y - 1, 106);
            World.THIS.SetCell(x - 1, y - 1, 106);
            //roads
            World.THIS.SetCell(x + 1, y, 35);
            World.THIS.GetCellConst((uint)(x + 1), (uint)(y)).is_destructible = false;
            World.THIS.GetCellConst((uint)(x + 1), (uint)(y)).HP = -2;
            World.THIS.SetCell(x + 2, y, 35);
            World.THIS.GetCellConst((uint)(x + 2), (uint)(y)).is_destructible = false;
            World.THIS.GetCellConst((uint)(x + 2), (uint)(y)).HP = -1;
            World.THIS.SetCell(x - 1, y, 35);
            World.THIS.GetCellConst((uint)(x - 1), (uint)(y)).is_destructible = false;
            World.THIS.GetCellConst((uint)(x - 1), (uint)(y)).HP = -2;
            World.THIS.SetCell(x - 2, y, 35);
            World.THIS.GetCellConst((uint)(x - 2), (uint)(y)).is_destructible = false;
            World.THIS.GetCellConst((uint)(x - 2), (uint)(y)).HP = -1;
            //
            World.THIS.SetCell(x, y + 1, 35);
            World.THIS.GetCellConst((uint)(x), (uint)(y + 1)).is_destructible = false;
            World.THIS.GetCellConst((uint)(x), (uint)(y + 1)).HP = -2;
            World.THIS.SetCell(x, y + 2, 35);
            World.THIS.GetCellConst((uint)(x), (uint)(y + 2)).is_destructible = false;
            World.THIS.GetCellConst((uint)(x), (uint)(y + 2)).HP = -1;
            World.THIS.SetCell(x, y - 1, 35);
            World.THIS.GetCellConst((uint)(x), (uint)(y - 1)).is_destructible = false;
            World.THIS.GetCellConst((uint)(x), (uint)(y - 1)).HP = -2;
            World.THIS.SetCell(x, y - 2, 35);
            World.THIS.GetCellConst((uint)(x), (uint)(y - 2)).is_destructible = false;
            World.THIS.GetCellConst((uint)(x), (uint)(y - 2)).HP = -1;
        }

        public override void Rebild()
        {
            rb(x, y);
            World.THIS.OnGunBuild(x, y, cid);
        }
    }

    public class Resp : Building
    {
        public override void Remove()
        {
            World.THIS.SetCell(x, y, 32);
            World.THIS.SetCell(x, y + 2, 32);
            World.THIS.SetCell(x + 1, y, 32);
            for (int px = 2; px < 6; px++)
            {
                for (int py = -1; py < 3; py++)
                {
                    if (World.THIS.GetCellConst((uint)(x + px), (uint)(y + py)) != null)
                    {
                        World.THIS.SetCell((uint)(x + px), (uint)(y + py), 35);
                    }
                }
            }
            //walls
            World.THIS.SetCell(x - 1, y, 32);
            World.THIS.SetCell(x - 1, y + 1, 32);
            World.THIS.SetCell(x - 1, y - 1, 32);
            World.THIS.SetCell(x, y - 1, 32);
            World.THIS.SetCell(x, y + 1, 32);
            World.THIS.SetCell(x + 1, y + 1, 32);
            World.THIS.SetCell(x + 1, y - 1, 32);
            World.THIS.SetCell(x + 1, y + 2, 32);
            World.THIS.SetCell(x - 1, y + 2, 32);
            using var db = new BDClass();
            try
            {
                Box.BuildBox(x, y, new long[] { 0, cryinside, 0, 0, 0, 0 }, null);
                foreach (var p in db.players.Where(p => p.resp == this))
                {
                    p.resp = db.resps.First();
                }
            }
            catch (Exception e) { }
            db.resps.Remove(this);
            db.SaveChanges();
            this.UpdatePackVis();
            World.ClearPack(x, y);
        }

        public override bool CanOpen(Player p)
        {
            return true;
        }

        public override void Update(string type)
        {
        }

        public uint respcost { get; set; }
        public int cryinside { get; set; }
        public int crymax { get; set; }
        public int moneyinside { get; set; }

        public override void Rebild()
        {
            rb(this.x, this.y);
        }

        public override string winid { get; set; }

        public Resp()
        {
            winid = "resp.base";
        }

        public static string rtext =
            "choose:\n[ENTER] = установить респаун\n[ESC] = отмена:3:2:2:9:7:000000000011100000010000000011100000010100000000000000000000000 ";

        public string resped(Player p)
        {
            if (p.resp == this)
            {
                return
                    $"Цена восстановления: <color=green>${respcost}</color>\n\n<color=#8f8>Вы привязаны к этому респу.</color>";
            }
            else
            {
                return
                    $"Цена восстановления: <color=green>${respcost}</color>\n\n<color=#f88>Привязать робота к респу?</color>";
            }
        }

        public static Resp Build(uint x, uint y, Player owner)
        {
            if (owner.connection == null)
            {
                World.THIS.SetCell(x, y, 37);
                var v1 = World.THIS.GetChunkPosByCoords(x, y);
                Chunk.chunks[(uint)v1.X, (uint)v1.Y].AddPack(x, y);
                World.packmap[(int)(x + y * World.height)] = new Resp()
                    { ownerid = 0, x = x, y = y, type = 'R', respcost = 20, cryinside = 100, off = 1 };
                return (Resp)World.packmap[(int)(x + y * World.height)];
            }

            if (!checkcan(x, y, owner))
            {
                return null;
            }

            rb(x, y);
            var v = World.THIS.GetChunkPosByCoords(x, y);
            Chunk.chunks[(uint)v.X, (uint)v.Y].AddPack(x, y);
            return new Resp()
            {
                ownerid = owner.id, x = x, y = y, type = 'R', respcost = 20, cryinside = 100, off = 1, crymax = 1000
            };
        }

        public void OnDeath(Player p)
        {
            if (ownerid > 0)
            {
                if (cryinside == 0)
                {
                    using var db = new BDClass();
                    p.resp = db.resps.First();
                    off = 0;
                    UpdatePackVis();
                    return;
                }

                cryinside--;
                if (p.money >= respcost)
                {
                    p.money -= respcost;
                    p.SendMoney();
                }
                else
                {
                    p.hp = 100;
                }
            }
        }

        public void AddCry(long col)
        {
            if (cryinside > 0)
            {
                off = 0;
            }
        }

        public static void rb(uint x, uint y)
        {
            World.THIS.SetCell(x, y, 37);
            World.THIS.SetCell(x, y + 2, 37);
            World.THIS.SetCell(x + 1, y, 37);
            for (var px = 2; px < 6; px++)
            {
                for (var py = -1; py < 3; py++)
                {
                    if (World.THIS.GetCellConst((uint)(x + px), (uint)(y + py)) != null)
                    {
                        World.THIS.SetCell((uint)(x + px), (uint)(y + py), 35);
                        World.THIS.GetCellConst((uint)(x + px), (uint)(y + py)).is_destructible = false;
                        World.THIS.GetCellConst((uint)(x + px), (uint)(y + py)).HP = -2;
                    }
                }
            }

            //walls
            World.THIS.SetCell(x - 1, y, 106);
            World.THIS.SetCell(x - 1, y + 1, 106);
            World.THIS.SetCell(x - 1, y - 1, 106);
            World.THIS.SetCell(x, y - 1, 106);
            World.THIS.SetCell(x, y + 1, 106);
            World.THIS.SetCell(x + 1, y + 1, 106);
            World.THIS.SetCell(x + 1, y - 1, 106);
            World.THIS.SetCell(x + 1, y + 2, 106);
            World.THIS.SetCell(x - 1, y + 2, 106);
        }

        public static bool checkcan(uint px, uint py, Player p)
        {
            if (World.ongun[px + py * World.height] != null)
            {
                if (World.ongun[px + py * World.height].Count > 0)
                {
                    if (World.ongun[px + py * World.height].First() != p.clanid ||
                        World.ongun[px + py * World.height].Count > 1)
                    {
                        var dat = System.Text.Encoding.UTF8.GetBytes("блок под пуфкой");

                        p.connection.SendLocalChat(dat.Length, 0, px, py, dat);
                        return false;
                    }
                }
            }

            var valid = 0;
            for (var x = -3; x <= 7; x++)
            {
                for (var y = -3; y <= 3; y++)
                {
                    if (!World.THIS.ValidCoordForPlace((uint)(px + x), (uint)(py + y)))
                    {
                        valid++;
                    }
                    else
                    {
                        var c = World.THIS.GetCellConst((uint)(px + x), (uint)(py + y));
                        if (!(c.is_empty && c.can_build_over))
                        {
                            p.connection.AddFX(0, (uint)(px + x), (uint)(py + y));
                            valid++;
                        }
                    }
                }
            }

            if (valid > 0)
            {
                return false;
            }

            return true;
        }

        public override void Open(Player p, string tab)
        {
            
            var builder = new HorbBuilder();
            if (this.cryinside == 0 && ownerid != p.id)
            {
                return;
            }

            if (tab == this.winid)
            {
                builder.SetTitle("РЕСП")
                    .SetText(
                        $"@@Респ - это место, где будет появляться ваш робот\nпосле уничтожения (HP = 0)\n\n{resped(p)}");
                if (p.resp != this)
                {
                    builder.AddButton("ПРИВЯЗАТЬ", "bind");
                }

                builder.AddButton("ВЫХОД", "exit");
                if (ownerid > 0)
                {
                    var db = new BDClass();
                    try
                    {
                        if (p == db.players.First(p => p.id == ownerid))
                        {
                             builder.Admin();
                        }
                    }
                    catch (Exception) { }
                }
            }
            else if (tab == "resp.ADMN")
            {
                var l = new string[0];
                l = l.Concat(RichListGenerator
                    .Fill("Заряд", cryinside, crymax, 1, "fill:b_100", "fill:b_1000", "fill:b_max").ToList()).ToArray();
                l = l.Concat(RichListGenerator.UInt("Стоимость", "cost", respcost).ToList()).ToArray();
                builder.SetTitle("хуй")
                    .AddButton("СОХРАНИТЬ", "save:%R%")
                    .AddButton("ВЫЙТИ", "exit")
                    .SetRichList(l);
            }

            builder.Send(p.win, p);
        }
    }
}