﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace StrangeServerCSharp
{
    public abstract class Building
    {

        public int Id { get; set; }
        public Building() { }
        [NotMapped]
        public abstract string winid { get; set; }
        public abstract void Open(Player p, string tab);
        public abstract void Rebild();
        public void UpdatePackVis()
        {
            var v = World.THIS.GetChunkPosByCoords(x, y);
            int chunkx = (int)v.X;
            int chunky = (int)v.Y;
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    
                    if (((chunkx + xxx) >= 0 && (chunky + yyy) >= 0) && ((chunkx + xxx) < XServer.THIS.chunkscx && (chunky + yyy) < XServer.THIS.chunkscy))
                    {
                        var ch = Chunk.chunks[chunkx + xxx, chunky + yyy];
                        foreach (var p in ch.bots)
                        {
                            XServer.players[p.Key].connection.ClearPack(this.x, this.y);
                            XServer.players[p.Key].connection.AddPack(this.type,this.x, this.y,this.cid,this.off);
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
        public static string[] mpcosts = new string[]
        {
           "8:^$10000;!$10000:",
           "1:^$0;!$0:",
           "2:^$0;!$0:",
           "3:^$0;!$0:",
           "4:^$0;!$0:",
           "5:^$0;!$0:",
           "6:^$0;!$0:",
           "7:^$0;!$0:",
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
           "26:^$0;!$0:",
           "27:^$0;!$0:",
           "28:^$0;!$0:",
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
        public Market() { winid = "market.tab_sell"; }
        public override string winid { get; set; }
        public override void Rebild()
        {
            if (World.THIS.GetCellConst(x + 3, y) != null)
            {
                World.THIS.SetCell(x + 3, y, 35);
                World.THIS.GetCellConst(x + 3, y).is_destructible = false;
                World.THIS.GetCellConst(x + 3, y).can_build_over = false;
            }
            if (World.THIS.GetCellConst(x + 4, y) != null)
            {
                World.THIS.SetCell(x + 4, y, 35);
                World.THIS.GetCellConst(x + 4, y).is_destructible = false;
                World.THIS.GetCellConst(x + 4, y).can_build_over = false;
            }
            if (World.THIS.GetCellConst(x - 3, y) != null)
            {
                World.THIS.SetCell(x - 3, y, 35);
                World.THIS.GetCellConst(x - 3, y).is_destructible = false;
                World.THIS.GetCellConst(x - 3, y).can_build_over = false;
            }
            if (World.THIS.GetCellConst(x - 4, y) != null)
            {
                World.THIS.SetCell(x - 4, y, 35);
                World.THIS.GetCellConst(x - 4, y).is_destructible = false;
                World.THIS.GetCellConst(x - 4, y).can_build_over = false;
            }

            //y
            if (World.THIS.GetCellConst(x, y + 3) != null)
            {
                World.THIS.SetCell(x, y + 3, 35);
                World.THIS.GetCellConst(x, y + 3).is_destructible = false;
                World.THIS.GetCellConst(x, y + 3).can_build_over = false;
            }
            if (World.THIS.GetCellConst(x, y + 4) != null)
            {
                World.THIS.SetCell(x, y + 4, 35);
                World.THIS.GetCellConst(x, y + 4).is_destructible = false;
                World.THIS.GetCellConst(x, y + 4).can_build_over = false;
            }
            if (World.THIS.GetCellConst(x, y - 3) != null)
            {
                World.THIS.SetCell(x, y - 3, 35);
                World.THIS.GetCellConst(x, y - 3).is_destructible = false;
                World.THIS.GetCellConst(x, y - 3).can_build_over = false;
            }
            if (World.THIS.GetCellConst(x, y - 4) != null)
            {
                World.THIS.SetCell(x, y - 4, 35);
                World.THIS.GetCellConst(x, y - 4).is_destructible = false;
                World.THIS.GetCellConst(x, y - 4).can_build_over = false;
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
            await Task.Run(delegate ()
            {
                System.Threading.Thread.Sleep(msdelay);
                act();
            });
        }
        public static bool checkcan(uint px, uint py, Player p)
        {
            int valid = 0;
            for (int x = -4; x <= 4; x++)
            {
                for (int y = -4; y <= 4; y++)
                {
                    if (!World.THIS.ValidCoord((uint)(px + x), (uint)(py + y)))
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
            var c = new HorbConst();
            if (p.win == "")
            {
                return;
            }
            if (tab == this.winid)
            {
                c.AddTab("ПРОДАТЬ КРИ", "");
                c.AddTab("КУПИТЬ КРИ", "tab_buy");
                c.AddTab("КРЕДИТЫ И ПРОЧЕЕ", "tab_misc");
                c.AddTab("ОРДЕРЫ", "tab_mkt");
                c.AddTab("АУКЦИОН", "tab_auc");
                c.AddTitle("МАРКЕТ");
                c.AddTextLine("Используйте полосы прокрутки, чтобы выбрать сколько продать кристаллов");
                c.AddTextLine("");
                c.AddCrysRight("будет продано");
                c.AddCrysLeft("  останется");
                var cry = p.crys.cry;
                for (int i = 0; i < 6; i++)
                {
                    c.AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[i], value = 0, descText = "x " + World.costs[i] });
                }
                c.AddButton("ПРОДАТЬ", "@sell:%M%");
                c.AddButton("ПРОДАТЬ ВСЕ", "@sellall");
                c.AddButton("ВЫЙТИ", "exit");
                if (p == BDClass.THIS.players.First(p => p.id == ownerid))
                {
                    c.Admin();
                }
            }
            else if (tab == "market.tab_buy")
            {
                c.AddTab("ПРОДАТЬ КРИ", "tab_sell");
                c.AddTab("КУПИТЬ КРИ", "");
                c.AddTab("КРЕДИТЫ И ПРОЧЕЕ", "tab_misc");
                c.AddTab("ОРДЕРЫ", "tab_mkt");
                c.AddTab("АУКЦИОН", "tab_auc");
                c.AddTitle("МАРКЕТ");
                c.SetText("<color=#f88></color>");
                c.AddCrysRight("сколько покупаем");
                c.AddCrysLeft("станет в грузе");
                c.AddCrysBuy();
                for (int i = 0; i < 6; i++)
                {
                    c.AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = (p.money / World.costsbuy[i]), value = 0, descText = "x " + World.costsbuy[i] });
                }
                c.AddButton("КУПИТЬ", "buy:%M%");
                c.AddButton("ВЫЙТИ", "exit");
                if (p == BDClass.THIS.players.First(p => p.id == ownerid))
                {
                    c.Admin();
                }
            }
            else if (tab == "market.tab_misc")
            {
                c.AddTab("ПРОДАТЬ КРИ", "tab_sell");
                c.AddTab("КУПИТЬ КРИ", "tab_buy");
                c.AddTab("КРЕДИТЫ И ПРОЧЕЕ", "");
                c.AddTab("ОРДЕРЫ", "tab_mkt");
                c.AddTab("АУКЦИОН", "tab_auc");
                c.AddTitle("МАРКЕТ");
                c.SetText("##");
                c.AddButton("ПРОДЛИТЬ ЛИЦЕНЗИЮ", "maker");
                c.AddButton("ВЫЙТИ", "exit");
                c.AddCss(68, 596, 0, "misc");
                c.rhorb.inv = InvGen.getmarket(mpcosts);
            }
            else if (tab.StartsWith("market.misc"))
            {
                string sd = tab.Split(':')[1];
                string cost = mpcosts.First(i => i.Split(':')[0] == sd);
                string sell = cost.Substring(cost.IndexOf('^') + 2, cost.Substring(cost.IndexOf('^') + 2).IndexOf(';'));
                string buy = cost.Substring(cost.IndexOf("!") + 2).Replace(":","");
                c.AddTab("ПРОДАТЬ КРИ", "tab_sell");
                c.AddTab("КУПИТЬ КРИ", "tab_buy");
                c.AddTab("КРЕДИТЫ И ПРОЧЕЕ", "");
                c.AddTab("ОРДЕРЫ", "tab_mkt");
                c.AddTab("АУКЦИОН", "tab_auc");
                c.SetText("\n ");
                c.AddTitle("МАРКЕТ");
                c.AddButton("НАЗАД", "<tab_misc");
                c.AddButton("ВЫЙТИ", "exit");
                c.AddList();
                c.AddListLine($"ПРОДАТЬ ПО ЛУЧШЕЙ ЦЕНЕ: 1 ЗА ${sell}", "ПРОДАТЬ 1", "miscsell");
                c.AddListLine($"                       10 ЗА ${(int.Parse(sell) * 10)}", "ПРОДАТЬ 10", "miscsellX");
                c.AddListLine($"КУПИТЬ ПО ЛУЧШЕЙ ЦЕНЕ: 1 ЗА ${buy}", "КУПИТЬ 1", "miscbuy");
                c.AddListLine($"                       10 ЗА ${(int.Parse(buy) * 10)}","КУПИТЬ 10","miscbuyX");
                c.AddMarketCard(sd," ");

            }
            c.Send(p.win, p);
        }
        public static string mtext = "choose:\n[ENTER] = установить рынок\n[ESC] = отмена:3:4:4:9:9:000000000000000000001101100001101100000000000001101100001101100000000000000000000 ";
    }
    public class Clans : Building
    {
        public Clans() { winid = "clans"; }

        public override string winid { get; set; }

        public override void Open(Player p, string tab)
        {
            if (tab == "!!" + winid)
            {
                var c = new HorbConst();
                c.AddTitle("КЛАНЫ");
                c.AddClanList();
                foreach (var cl in BDClass.THIS.clans.Where(clan => clan.name != ""))
                {
                    c.AddClanListLine(cl.id.ToString(), $"<color=white>{cl.name}</color>  [{cl.abr}]", "<color=#88ff88ff>прием открыт</color>", "clan:" + cl.id);
                }
                c.AddButton("ВЫХОД", "exit");
                c.Send(tab, p);
                return;
            }
        }

        public override void Rebild()
        {
            throw new NotImplementedException();
        }
    }
    public class Resp : Building
    {
        public uint respcost { get; set; }
        public int cryinside { get; set; }
        public int crymax { get; set; }
        public int moneyinside{ get; set; }
        public override void Rebild()
        {
            rb(this.x, this.y);
        }
        public override string winid { get; set; }
        public Resp() { winid = "resp.base"; }
        public static string rtext = "choose:\n[ENTER] = установить респаун\n[ESC] = отмена:3:2:2:9:7:000000000011100000010000000011100000010100000000000000000000000 ";
        public string resped(Player p)
        {
            if (p.resp == this)
            {
                return $"Цена восстановления: <color=green>${respcost}</color>\n\n<color=#8f8>Вы привязаны к этому респу.</color>";
            }
            else
            {
                return $"Цена восстановления: <color=green>${respcost}</color>\n\n<color=#f88>Привязать робота к респу?</color>";
            }
        }
        public static Resp Build(uint x, uint y, Player owner)
        {
            if (owner.connection == null)
            {
                World.THIS.SetCell(x, y, 37);
                var v1 = World.THIS.GetChunkPosByCoords(x, y);
                Chunk.chunks[(uint)v1.X, (uint)v1.Y].AddPack(x, y);
                World.packmap[(int)(x + y * World.height)] = new Resp() { ownerid = 0, x = x, y = y, type = 'R', respcost = 20, cryinside = 100, off = 1 };
                return (Resp)World.packmap[(int)(x + y * World.height)];
            }
            if (!checkcan(x, y, owner))
            {
                return null;
            }
            rb(x, y);
            var v = World.THIS.GetChunkPosByCoords(x, y);
            Chunk.chunks[(uint)v.X, (uint)v.Y].AddPack(x, y);   
            return new Resp() { ownerid = owner.id, x = x, y = y, type = 'R', respcost = 20, cryinside = 100, off = 1, crymax = 1000 };
        }
        public void OnDeath(Player p)
        {
            if (ownerid > 0)
            {
                if (cryinside == 0)
                {
                    p.resp = BDClass.THIS.resps.First();
                    off = 0;
                    UpdatePackVis();
                    return;
                }
                cryinside--;
                if (p.money >= respcost)
                {
                    p.money -= respcost;
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
            for (int px = 2; px < 6; px++)
            {
                for (int py = -1; py < 3; py++)
                {
                    if (World.THIS.GetCellConst((uint)(x + px), (uint)(y + py)) != null)
                    {
                        World.THIS.SetCell((uint)(x + px), (uint)(y + py), 35);
                        World.THIS.GetCellConst((uint)(x + px), (uint)(y + py)).is_destructible = false;
                        World.THIS.GetCellConst((uint)(x + px), (uint)(y + py)).can_build_over = false;
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
            int valid = 0;
            for (int x = -3; x <= 7; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    if (!World.THIS.ValidCoord((uint)(px + x), (uint)(py + y)))
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
            var c = new HorbConst();
            if (this.cryinside == 0 && ownerid != p.id)
            {
                return;
            }
            if (tab == this.winid)
            {
                c.AddTitle("РЕСП");
                c.SetText($"@@Респ - это место, где будет появляться ваш робот\nпосле уничтожения (HP = 0)\n\n{resped(p)}");
                if (p.resp != this)
                {
                    c.AddButton("ПРИВЯЗАТЬ", "bind");
                }
                c.AddButton("ВЫХОД", "exit");
                if (ownerid > 0)
                {
                    if (p == BDClass.THIS.players.First(p => p.id == ownerid))
                    {
                        c.Admin();
                    }
                }
            }
            else if (tab == "resp.ADMN")
            {
                c.rhorb.richList = new string[0];
                c.AddTitle("хуй");
                c.AddButton("СОХРАНИТЬ", "save:%R%");
                c.AddButton("ВЫЙТИ", "exit");
                string[] l = new string[0];
                l = l.Concat(RichListGenerator.Fill("Заряд",cryinside, crymax, 1, "fill:b_100", "fill:b_1000", "fill:b_max").ToList()).ToArray();
                l = l.Concat(RichListGenerator.UInt("Стоимость", "cost", respcost).ToList()).ToArray();
                c.rhorb.richList = l;
            }
            c.Send(p.win, p);
        }
    }
}
