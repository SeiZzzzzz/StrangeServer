namespace StrangeServerCSharp
{
    public class Inventory
    {
        public Inventory(Player pl)
        {
            this.player = pl;
            for (var i = 0; i < 49; i++)
            {
                items.Add(i, new Item(i, 0));
            }

            items[1].count++;
            items[3].count++;
            items[5].count++;
            items[6].count++;
            items[7].count++;
        }

        public Player player;

        public int GetInvL()
        {
            var i = 0;
            for (var j = 0; j < 49; j++)
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
            var text = "";
            foreach (var item in items.Values)
            {
                var t2 = item.getit();
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
            this.SendInv();
            if (sel == 1)
            {
                this.player.connection.Send("IN", Resp.rtext);
            }

            if (sel == 3)
            {
                this.player.connection.Send("IN", Market.mtext);
            }
            else if (sel == 5)
            {
                this.player.connection.Send("IN", Item.usetextboom);
            }
            else if (sel == 6)
            {
                this.player.connection.Send("IN", Item.usetextprot);
            }
            else if (sel == 7)
            {
                this.player.connection.Send("IN", Item.usetextraz);
            }
            else if (sel == 26)
            {
                this.player.connection.Send("IN", Gun.gtext);
            }
            else if (sel == 28)
            {
                this.player.connection.Send("IN", dizz);
            }
        }

        public void Use(uint x, uint y)
        {
            if (items[sel].count <= 0 || !World.THIS.ValidCoord(x, y) || sel < 0 || World.THIS.GetCell(x, y) == 37)
            {
                return;
            }

            if (sel == 1)
            {
                var xp = (uint)(x + (player.dir == 3 ? 2 : player.dir == 1 ? -2 : 0));
                var yp = (uint)(y + (player.dir == 0 ? 2 : player.dir == 2 ? -2 : 0));
                var m = Resp.Build(xp, yp, this.player);
                if (m != null)
                {
                    using var db = new BDClass();
                    World.packmap[xp + yp * World.width] = m;
                    db.resps.Add(m);
                    items[sel].count--;
                    this.SendInv();
                    Building.AddPack();
                }

                player.GimmePacks();
                return;
            }

            if (sel == 3)
            {
                var xp = (uint)(x + (player.dir == 3 ? 2 : player.dir == 1 ? -2 : 0));
                var yp = (uint)(y + (player.dir == 0 ? 2 : player.dir == 2 ? -2 : 0));
                var m = Market.Build(xp, yp, this.player);
                if (m != null)
                {
                    using var db = new BDClass();
                    World.packmap[xp + yp * World.width] = m;
                    db.markets.Add(m);
                    items[sel].count--;
                    this.SendInv();
                    Building.AddPack();
                }

                player.GimmePacks();
                return;
            }

            if (sel == 26)
            {
                if (player.clanid == 0)
                {
                    var dat = System.Text.Encoding.UTF8.GetBytes("ДОЛБАЕБ КЛАН НУЖЕН");

                    player.connection.SendLocalChat(dat.Length, 0, x, y, dat);
                    return;
                }
                if (y < 70)
                {
                    byte[] dat = System.Text.Encoding.UTF8.GetBytes("крит высота пух 70");

                    player.connection.SendLocalChat(dat.Length, 0, x, y, dat);
                    return;
                }
                var xp = (uint)(x + (player.dir == 3 ? 1 : player.dir == 1 ? -1 : 0));
                var yp = (uint)(y + (player.dir == 0 ? 1 : player.dir == 2 ? -1 : 0));
                var g = Gun.Build(xp, yp, this.player);
                if (g != null)
                {
                    World.packmap[xp + yp * World.width] = g;
                    items[sel].count--;
                    this.SendInv();
                    Building.AddPack();
                }

                player.GimmePacks();
                return;
            }

            if (sel == 28)
            {
                if (Diz(x, y, out var rpack))
                {
                    Building.AddPack();
                    if (rpack.type == 'G')
                    {
                        items[26].count++;
                    }
                    else if (rpack.type == 'R')
                    {
                        items[1].count++;
                    }
                    else if (rpack.type == 'M')
                    {
                        items[3].count++;
                    }
                    this.player.GimmePacks();
                    items[sel].count--;
                }

                return;
            }

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
                items[sel].count--;
            }

            this.SendInv();
        }

        public void SendInv()
        {
            this.player.connection.Send("IN", "show:" + GetInvL() + ":" + sel + ":" + getinv());
        }

        public bool Diz(uint x, uint y, out Building rpack)
        {
            x = player.x;
            y = player.y;
            if (World.THIS.ValidCoord(x, y))
            {
                if (World.packmap[x + y * World.width] != null &&
                    World.packmap[x + y * World.width].ownerid == player.id)
                {
                    rpack = World.packmap[x + y * World.width];
                    World.packmap[x + y * World.width].Remove();
                    World.packmap[x + y * World.width] = null;
                    return true;
                }
            }
            int xp = (player.dir == 3 ? 3 : player.dir == 1 ? -3 : 0);
            int yp = (player.dir == 0 ? 3 : player.dir == 2 ? -3 : 0);
            if (player.dir == 3 || player.dir == 1)
            {
                if (xp < 0)
                {
                    for (;(x + xp) <= x;xp++)
                    {
                        if (World.THIS.ValidCoord((uint)(x + xp), y))
                        {
                            if (World.packmap[(uint)(x + xp) + y * World.width] != null && World.packmap[(uint)(x + xp) + y * World.width].ownerid == player.id)
                            {
                                rpack = World.packmap[(uint)(x + xp) + y * World.width];
                                World.packmap[(uint)(x + xp) + y * World.width].Remove();
                                World.packmap[(uint)(x + xp) + y * World.width] = null;
                                return true;
                            }
                        }
                    }
                }
                if (xp > 0)
                {
                    for (; (y + xp) >= y; xp--)
                    {
                        if (World.THIS.ValidCoord((uint)(x + xp), y))
                        {
                            if (World.packmap[(uint)(x + xp) + y * World.width] != null && World.packmap[(uint)(x + xp) + y * World.width].ownerid == player.id)
                            {
                                rpack = World.packmap[(uint)(x + xp) + y * World.width];
                                World.packmap[(uint)(x + xp) + y * World.width].Remove();
                                World.packmap[(uint)(x + xp) + y * World.width] = null;
                                return true;
                            }
                        }
                    }
                }
            }
            else if (player.dir == 0 || player.dir == 2)
            {
                if ((y + yp) < y)
                {
                    for (; (y + yp) <= y; yp++)
                    {
                        if (World.THIS.ValidCoord(x, (uint)(y + yp)))
                        {
                            if (World.packmap[x + (uint)(y + yp) * World.width] != null && World.packmap[x + (uint)(y + yp) * World.width].ownerid == player.id)
                            {
                                rpack = World.packmap[x + (uint)(y + yp) * World.width];
                                World.packmap[x + (uint)(y + yp) * World.width].Remove();
                                World.packmap[x + (uint)(y + yp) * World.width] = null;
                                return true;
                            }
                        }
                    }
                }
                if (yp > y)
                {
                    for (; (y + yp) >= y; yp--)
                    {
                        if (World.THIS.ValidCoord(x, (uint)(y + yp)))
                        {
                            if (World.packmap[x + (uint)(y + yp) * World.width] != null && World.packmap[x + (uint)(y + yp) * World.width].ownerid == player.id)
                            {
                                rpack = World.packmap[x + (uint)(y + yp) * World.width];
                                World.packmap[x + (uint)(y + yp) * World.width].Remove();
                                World.packmap[x + (uint)(y + yp) * World.width] = null;
                                return true;
                            }
                        }
                    }
                }
            }

            rpack = null;
            return false;
        }

        public static string dizz = "choose:ДИЗЗ\n\n[ENTER] = собрать здание в пак\n[ESC] = отмена:1:0:0:0:0:0 ";

        public bool CanBoom(uint x, uint y)
        {
            if (World.ongun[x + y * World.height] != null)
            {
                if (World.ongun[x + y * World.height].Count > 0)
                {
                    if (World.ongun[x + y * World.height].First() != player.clanid ||
                        World.ongun[x + y * World.height].Count > 1)
                    {
                        var dat = System.Text.Encoding.UTF8.GetBytes("блок под пуфкой");

                        player.connection.SendLocalChat(dat.Length, 0, x, y, dat);
                        return false;
                    }
                }
            }

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

        public static string usetextraz =
            "choose:\n[ENTER] = установить бомбу\n[ESC] = отмена:1:9:9:19:19:0000001111111000000000011111111111000000011111111111110000011111111111111100011111111111111111001111111111111111101111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111011111111111111111001111111111111111100011111111111111100000111111111111100000001111111111100000000001111111000000 ";

        public static string usetextboom =
            "choose:\n[ENTER] = установить бомбу\n[ESC] = отмена:1:3:3:7:7:0011100011111011111111111111111111101111100011100 ";

        public static string usetextprot = "choose:\n[ENTER] = установить бомбу\n[ESC] = отмена:1:1:1:3:3:111111111 ";
        public int id;
        public int count;
    }
}