namespace StrangeServerCSharp
{
    public abstract class Building
    {
        public Building(Player p,uint x, uint y, char type, int cid = 0, int off = 0)
        {
            owner = p;
            this.x = x;
            this.y = y;
            this.type = type;
            this.cid = cid;
            this.off = off;
            winid = "";
        }
        public abstract string winid { get; set; }
        public abstract void Open(Player p, string tab);
        public Pack GetShpack
        {
            get { return new Pack { cid = this.cid, off = this.off, type = this.type, x = this.x, y = this.y }; }
        }
        public int cid;
        public int off;
        public char type;
        public uint x;
        public uint y;
        public Player owner;
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
        public Market(Player p,uint x, uint y, char type) : base(p,x, y, type) { winid = "market.tab_sell"; }
        public override string winid { get; set; }
        public static Market Build(uint x, uint y, Player owner)
        {
            if (!checkcan(x, y, owner))
            {
                return null;
            }
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

            var v = World.THIS.GetChunkPosByCoords(x, y);
            Chunk.chunks[(uint)v.X,(uint)v.Y].AddPack(x, y);
            return new Market(owner, x, y, 'M');
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
            for(int x = -3; x <= 3;x++)
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
                if (p == owner)
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
                if (p == owner)
                {
                    c.Admin();
                }
            }
            else if (tab == "market.tab_misc")
            {

            }
            c.Send(p.win,p);
        }
        public static string mtext = "choose:\n[ENTER] = установить рынок\n[ESC] = отмена:3:4:4:9:9:000000000000000000001101100001101100000000000001101100001101100000000000000000000 ";
    }
    public class Resp : Building
    {
        public override string winid { get; set; }
        public Resp(Player p,uint x, uint y, char type, int off = 0) : base(p,x, y, type, off) { winid = "resp"; }
        public override void Open(Player p, string tab)
        {
            return;
        }
    }
}
