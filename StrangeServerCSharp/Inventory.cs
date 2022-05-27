namespace StrangeServerCSharp
{
    public class Inventory
    {
        public Inventory(Player pl)
        {
            this.player = pl;
            for (int i = 0; i < 49; i++)
            {
                items.Add(i, new Item(i, 0));
            }
            items[3].count++;
            items[5].count++;
            items[6].count++;
            items[7].count++;

        }
        public Player player;
        public int GetInvL()
        {
            int i = 0;
            for (int j = 0; j < 49; j++)
            {
                if (items[i].count > 0)
                {
                    i++;
                }
            }
            return i;
        }
        public string getinv()
        {
            string text = "";
            foreach (var item in items.Values)
            {
                string t2 = item.getit();
                if (t2 != "")
                {
                    text += item.getit() + "#";
                }
            }
            if (text == "")
            {
                return "";
            }
            return text.Substring(0, text.Length - 1);
        }
        public void Choose(int id)
        {
            sel = id;
            this.player.connection.Send("IN", "show:" + GetInvL() + ":" + sel + ":" + getinv());
            if (sel == 3)
            {
                this.player.connection.Send("IN", Market.mtext);
            }
            else  if (sel == 5)
            {
                this.player.connection.Send("IN", Item.usetextboom);
            }
            else if (sel == 6)
            {
                this.player.connection.Send("IN", Item.usetextprot);
            }
        }
        public void Use(uint x, uint y)
        {
            if (items[sel].count <= 0 || !World.THIS.ValidCoord(x, y) || sel < 0)
            {
                return;
            }
            if (sel == 3)
            {
                uint xp = (uint)(x + (player.dir == 3 ? 2 : player.dir == 1 ? -2 : 0));
                uint yp = (uint)(y + (player.dir == 0 ? 2 : player.dir == 2 ? -2 : 0));
                var m = Market.Build(xp, yp, this.player);
                if (m != null)
                {
                    World.packmap[xp + yp * World.width] = m;
                }
            }
            this.player.GimmePacks();
            if (!World.THIS.GetCellConst(x, y).is_empty)
            {
                return;
            }
            if (!CanBoom(x, y))
            {
                return;
            }
            if (this.sel == 5)
            {
                World.THIS.Boom(x, y);
            }
            else if (this.sel == 6)
            {
                World.THIS.Prot(x, y);
            }
            else if (this.sel == 7)
            {
                World.THIS.Raz(x, y);
            }
            if (items[sel].count > 0)
            {
                //items[sel].count--;
            }
            this.player.connection.Send("IN", "show:" + GetInvL() + ":" + sel + ":" + getinv());
        }
        public bool CanBoom(uint x, uint y)
        {
            if (World.canboom[x + y * World.height] == false)
            {
                return true;
            }
            return false;
        }
        public int sel = -1;
        public Dictionary<int, Item> items = new Dictionary<int, Item>();
    }
    public class Item
    {
        public Item(int id, int c)
        {
            this.id = id;
            this.count = c;
        }
        public string getit()
        {
            if (count == 0)
            {
                return "";
            }
            return id + "#" + count;
        }
        public static string usetextboom = "choose:\n[ENTER] = установить бомбу\n[ESC] = отмена:1:3:3:7:7:0011100011111011111111111111111111101111100011100 ";
        public static string usetextprot = "choose:\n[ENTER] = установить бомбу\n[ESC] = отмена:1:1:1:3:3:111111111 ";
        public int id;
        public int count;
    }
}
