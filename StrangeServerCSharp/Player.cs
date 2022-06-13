using System.Numerics;
using System.Text;

namespace StrangeServerCSharp
{
    public class Player
    {
        public int id { get; set; }
        public string name { get; set; }
        public int bid = 0;
        public int level;
        public long creds { get; set; }
        public long money { get; set; }
        public string hash { get; set; }
        public string passwd { get; set; }
        public Vector2 pos = new Vector2(0, 0);
        public uint x { get { return (uint)pos.X; } set { pos.X = value; } }
        public uint y { get { return (uint)pos.Y; } set { pos.Y = value; } }
        public Resp resp { get; set; }
        public int clanid { get; set; }
        public Settings settings { get; set; }
        public int dir;
        public int skin;
        public int tail;
        public Session connection;
        public int chunkx;
        public int lastupdchchunkx;
        public int chunky;
        public int lastupdchchunky;
        public bool autoDig = false;
        public bool offline = false;
        public PeriodicTimer timer;
        public Inventory inventory;
        public BasketCrys crys;
        public int hp;
        public int gbcost = 0;
        public int ybcost = 0;
        public int rbcost = 0;
        public int maxhp = 200;
        public string win = "";
        public int[] counter = { 0, 0, 0 };
        public int[] nextlvl = { 20, 200, 200 };
        public int[] lvl = { 1, 3, 200 };
        public Stack<byte> geo = new Stack<byte>();
        Queue<Line> console = new Queue<Line>();
        public Building cpack = null;
        public Player()
        {
            inventory = new Inventory(this);
            crys = new BasketCrys(this);
            this.hp = maxhp;
            this.chunkx = (int)Math.Floor(this.pos.X / 32);
            this.chunky = (int)Math.Floor(this.pos.Y / 32);
            level = 0;
            creds = 0;
            money = 0;
            clanid = 0;
            dir = 0;
            skin = 0;
            tail = 0;
            chunkx = 0;
            chunky = 0;
            lastupdchchunkx = 0;
            lastupdchchunky = 0;
            console.Enqueue(new Line { text = "@@> Добро пожаловать в консоль!" });
            for (int i = 0; i < 4; i++)
            {
                AddConsoleLine();
            }
            AddConsoleLine("Если вы не понимаете, что происходит,");
            AddConsoleLine("или вас попросили выполнить команду,");
            AddConsoleLine("сосите хуй глотайте сперму");
            for (int i = 0; i < 8; i++)
            {
                AddConsoleLine();
            }
            hash = GenerateHash();

        }
        public void SendClan()
        {
            if (this.connection == null)
            {
                return;
            }
            if (clanid == 0)
            {
                this.connection.Send("cH", "_");
            }
            else
            {
                this.connection.Send("cS", clanid.ToString());
            }
        }
        public string GenerateHash()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public class Line
        {
            public string text { get; set; }
        }
        public void ForceRemove()
        {
            while (Chunk.chunks[this.chunkx, this.chunky].bots.ContainsKey(this.id))
            {
                Chunk.chunks[this.chunkx, this.chunky].bots.Remove(this.id);
            }
        }
        public void SendMoney()
        {
            this.connection.Send("P$", new V { money = this.money, creds = this.creds }.ToString());
        }
        public struct V
        {
            public long money;
            public long creds;
            public override string ToString()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }
        }
        public void ShowConsole()
        {
            var c = new HorbConst();
            c.AddIConsole();
            c.AddIConsolePlace("ваша команда");
            foreach (var i in console)
            {
                c.AddTextLine(i.text);
            }
            c.AddButton("ВЫПОЛНИТЬ", "%I%");
            c.AddButton("ВЫЙТИ", "exit");
            c.Send("!!console", this);
        }
        public void AddConsoleLine(string text)
        {
            console.Enqueue(new Line { text = ">" + text });
            if (console.Count > 16)
            {
                console.Dequeue();
                var l = console.First();
                l.text = "@@" + l.text;
            }
        }
        public void AddConsoleLine()
        {
            console.Enqueue(new Line { text = ">    " });
            if (console.Count > 16)
            {
                console.Dequeue();
                var l = console.Peek();
                l.text = "@@" + l.text;
            }
        }
        public void SendHp()
        {
            this.connection.Send("@L", hp + ":" + maxhp);
        }
        public void UpHp(int damage)
        {
            counter[2] += damage;
            if (nextlvl[2] <= counter[2])
            {
                counter[2] = 0;
                lvl[2] = (int)(lvl[2] * 1.25f);
                maxhp = lvl[2];
                SendLvl();
                nextlvl[2] = (int)(nextlvl[2] * 1.5f);
            }
        }
        public void Hurt(int damage)
        {
            if (damage <= 0)
            { return; }
            UpHp(damage);
            if (hp - damage <= 0)
            {
                this.Death();
            }
            else if (hp - damage > 0)
            {
                hp -= damage;
                SendHp();
                SendDFToBots(6, 0, 0, 0, 0);
            }
            var cell = World.cellps[World.THIS.GetCell((uint)this.pos.X, (uint)this.pos.Y)];

            if (!cell.is_destructible)
            {
                //this.Move((uint)this.pos.X, (uint)this.pos.Y, this.dir);
              
                return;
            }
            World.THIS.DestroyCell((uint)this.pos.X, (uint)this.pos.Y);
        }
        public void Death()
        {
            HorbDecoder.Exit("exit", this);
            this.resp.OnDeath(this);
            var rx = resp.x + 2;
            var ry = resp.y;
            uint dx = (uint)this.pos.X;
            uint dy = (uint)this.pos.Y;
            var resped = false;
            while (!resped)
            {
                var x = World.Random.Next(0, 4);
                var y = World.Random.Next(-1, 3); ;

                var tx = (uint)(rx + x);
                var ty = (uint)(ry + y);
                if (World.THIS.ValidCoord(tx, ty) && (World.Random.Next(0, 100) < 5))
                {
                    SendFXoBots(2, dx, dy);
                    this.pos = new Vector2(tx, ty);
                    this.connection.Send("@T", $"{pos.X}:{pos.Y}");
                    if (this.crys.AllCry > 0)
                    {
                        Box.BuildBox(dx, dy, this.crys.cry,this);
                        this.crys.ClearCrys();
                    }
                    hp = maxhp;
                    SendHp();
                    resped = true;
                }
            }
        }
        public Vector2 GetDirCord()
        {
            uint x = (uint)(pos.X + (dir == 3 ? 1 : dir == 1 ? -1 : 0));
            uint y = (uint)(pos.Y + (dir == 0 ? 1 : dir == 2 ? -1 : 0));
            return new Vector2(x, y);
        }
        public void UseGeo(uint x, uint y)
        {
            var cell = World.THIS.GetCellConst(x, y);
            if (World.ongun[x + y * World.height] != null)
            {
                if (World.ongun[x + y * World.height].Count > 0)
                {


                    if (World.ongun[x + y * World.height].Count > 1 || World.ongun[x + y * World.height].First() != this.clanid)
                    {
                        byte[] dat = Encoding.UTF8.GetBytes("заблокировано под пушкой");

                        this.connection.SendLocalChat(dat.Length, 0, x, y, dat);
                        return;
                    }
                }
            }
            if (cell.HP < 0)
            {
                return;
            }
            if (cell.is_empty && cell.can_build_over)
            {
                if (this.geo.Count > 0)
                {
                    World.THIS.SetCell(x, y, this.geo.Pop());
                    World.cells[x + y * World.width].HP = 0;
                    var j = World.ContPlayers(x, y);
                    foreach (var i in j)
                    {
                        Console.WriteLine(XServer.players[i].name + ":");
                    }
                    if (this.geo.Count == 0)
                    {
                        this.connection.Send("GE", "empty");
                    }
                    else
                    {
                        this.connection.Send("GE", World.cellps[this.geo.Peek()].name);
                    }
                }
            }
            else if (cell.is_pickable)
            {
                this.geo.Push(World.THIS.GetCell(x, y));
                this.connection.Send("GE", cell.name);
                World.THIS.DestroyCell(x, y);
            }
        }
        public async void GimmeBotsUPD()
        {
            try
            {
                timer = new PeriodicTimer(TimeSpan.FromMilliseconds(50));

                while (await timer.WaitForNextTickAsync())
                {
                    GimmeBots();
                }
            } catch (Exception ex) { }
        }
        public void GimmeBots()
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((this.chunkx + xxx) >= 0 && (this.chunky + yyy) >= 0) && ((this.chunkx + xxx) < XServer.THIS.chunkscx && (this.chunky + yyy) < XServer.THIS.chunkscy))
                    {
                        var x = (this.chunkx + xxx);
                        var y = (this.chunky + yyy);
                        var ch = Chunk.chunks[x, y];

                        foreach (var id in ch.bots)
                        {
                            var player = XServer.players[id.Key];

                            this.connection.SendBot(player.id, (uint)player.pos.X, (uint)player.pos.Y, player.dir, player.clanid, player.skin, player.tail);
                            this.connection.SendNick(id.Key, player.name);

                        }
                    }
                }
            }
        }
        public void GimmePacks()
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((this.chunkx + xxx) >= 0 && (this.chunky + yyy) >= 0) && ((this.chunkx + xxx) < XServer.THIS.chunkscx && (this.chunky + yyy) < XServer.THIS.chunkscy))
                    {
                        var x = (this.chunkx + xxx);
                        var y = (this.chunky + yyy);
                        var ch = Chunk.chunks[x, y];

                        foreach (var v in ch.packs)
                        {
                            var pack = World.packmap[(uint)v.Key.X + (uint)v.Key.Y * World.height];
                            if (pack != null)
                            {
                                this.connection.AddPack(pack.GetShpack);
                            }

                        }
                    }
                }
            }
        }
        public void SendDFToBots(int fx, uint fxx, uint fxy, int dir, int col = 0)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((this.chunkx + xxx) >= 0 && (this.chunky + yyy) >= 0) && ((this.chunkx + xxx) < XServer.THIS.chunkscx && (this.chunky + yyy) < XServer.THIS.chunkscy))
                    {
                        var x = (this.chunkx + xxx);
                        var y = (this.chunky + yyy);
                        var ch = Chunk.chunks[x, y];

                        foreach (var id in ch.bots)
                        {
                            var player = XServer.players[id.Key];
                            player.connection.AddDFX(fx, dir, fxx, fxy, this.id, col);

                        }
                    }
                }
            }
        }
        public void SendFXoBots(int fx, uint fxx, uint fxy)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((this.chunkx + xxx) >= 0 && (this.chunky + yyy) >= 0) && ((this.chunkx + xxx) < XServer.THIS.chunkscx && (this.chunky + yyy) < XServer.THIS.chunkscy))
                    {
                        var x = (this.chunkx + xxx);
                        var y = (this.chunky + yyy);
                        var ch = Chunk.chunks[x, y];

                        foreach (var id in ch.bots)
                        {
                            var player = XServer.players[id.Key];
                            player.connection.AddFX(fx, fxx, fxy);

                        }
                    }
                }
            }
        }
        public void Build(uint x, uint y, string type)
        {
            if (!World.THIS.ValidForB(x, y))
            {
                return;
            }
            if (World.ongun[x + y * World.height] != null)
            {
                if (World.ongun[x + y * World.height].Count > 0)
                {


                    if (World.ongun[x + y * World.height].Count > 1 || World.ongun[x + y * World.height].First() != this.clanid)
                    {
                        byte[] dat = Encoding.UTF8.GetBytes("заблокировано под пушкой");

                        this.connection.SendLocalChat(dat.Length, 0, x, y, dat);
                        return;
                    }
                }
            }
            var cell = World.cellps[World.THIS.GetCell(x, y)];
            if (type == "G")
            {
                if ((cell.is_empty && cell.can_build_over) && this.crys.RemoveCrys(0, gbcost))
                {
                    World.THIS.SetCell(x, y, 101);
                }
                if (cell.id == 101 && this.crys.RemoveCrys(1, ybcost))
                {
                    World.THIS.SetCell(x, y, 102);
                }
                if (cell.id == 102 && this.crys.RemoveCrys(2, rbcost))
                {
                    World.THIS.SetCell(x, y, 105);
                }
            }
            else if (type == "V")
            {
                World.THIS.SetCell(x, y, cellg);
            }
            else if (type == "R" && (cell.is_empty && cell.can_build_over))
            {
                World.THIS.SetCell(x, y, 35);
            }
        }
        public byte cellg;
        public void HurtGun(int damage)
        {
            Hurt(damage);
        }
        public void Heal()
        {
            if (hp == maxhp)
            {
                return;
            }
            var cost = 5 / lvl[1];
            if (!this.crys.RemoveCrys(2, (long)cost))
            {
                return;
            }
            int healhp = (int)(this.lvl[1] * 1.7f);
            if ((hp + healhp) > maxhp)
            {
                hp = maxhp;
                SendDFToBots(5, 0, 0, 0, 0);
                SendHp();
            }
            else
            {
                hp += healhp;
                SendDFToBots(5, 0, 0, 0, 0);
                SendHp();
            }
            counter[1] += healhp;
            if (nextlvl[1] <= counter[1])
            {
                counter[1] = 0;
                lvl[1]++;
                SendLvl();
                nextlvl[1] = (int)(nextlvl[1] * 1.05f);
            }
        }
        public void SendLvl()
        {
            string i = lvl.Sum().ToString();
            this.connection.Send("LV", i.ToString());
        }
        public void CDob()
        {
            counter[0]++;
            if (nextlvl[0] <= counter[0])
            {
                SendLvl();
                lvl[0] += (int)(lvl[0] * 1.25f);
                nextlvl[0] = (int)(nextlvl[0] * 1.25f);
            }
        }
        public void CBox(uint x, uint y)
        {
            if (World.boxmap[x + y * World.height] != null)
            {
                var b = World.boxmap[x + y * World.height].AllCrys;
                Box.CollectBox(x, y, this);
                byte[] dat = Encoding.UTF8.GetBytes("+" + b);

                this.connection.SendLocalChat(dat.Length, 0, x, y, dat);
            }
            else
            {
                World.THIS.DestroyCell(x, y);
            }
        }
        public void Bz(uint x, uint y)
        {
            var cell = World.cells[x + y * World.height];
            SendDFToBots(0, (uint)this.pos.X, (uint)this.pos.Y, this.dir);
            if (cell.damage > 0)
            {
                this.Hurt(cell.damage);
            }
            if (!cell.is_destructible)
            {
                return;
            }
            if (cell.id == 90)
            {
                CBox(x, y);
                return;
            }
            if (World.THIS.isCrys(cell.id))
            {
                GetCry(x, y, (byte)cell.id);
            }
            if (cell.HP <= 1)
            {
                if (cell.id == 105)
                {
                    World.THIS.DestroyWithRoadCell(x, y);
                    return;
                }
                World.THIS.DestroyCell(x, y);
                return;
            }
            cell.HP--;
        }
        public void RemoveMeFromChunk()
        {
            var chtoremove = Chunk.chunks[this.lastupdchchunkx, this.lastupdchchunky];
            var chtoadd = Chunk.chunks[this.chunkx, this.chunky];
            if (Chunk.chunks[this.lastupdchchunkx, this.lastupdchchunky] != null)
            {
                if (chtoremove.bots.ContainsKey(this.id))
                {
                    chtoremove.bots.Remove(this.id);
                }
                if (!chtoadd.bots.ContainsKey(this.id))
                {
                    chtoadd.AddBot(this.id);
                }
            }
        }
        public void SendBInfo()
        {
            this.connection.Send("BI", "{\"x\":" + pos.X + ",\"y\":" + pos.Y + ",\"id\":" + id + ",\"name\":\"" + name + "\"}");
        }
        public void SetNick(string text)
        {
            this.name = text;
            this.connection.SendNick(id, name);
            BDClass.THIS.SaveChanges();
        }
        public void SendLocalMsg(byte[] msg)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((this.chunkx + xxx) >= 0 && (this.chunky + yyy) >= 0) && ((this.chunkx + xxx) < XServer.THIS.chunkscx && (this.chunky + yyy) < XServer.THIS.chunkscy))
                    {
                        var x = (this.chunkx + xxx);
                        var y = (this.chunky + yyy);
                        var ch = Chunk.chunks[x, y];
                        foreach (var id in ch.bots)
                        {
                            var player = XServer.players[id.Key];
                            player.connection.SendLocalChat(msg.Length, this.id, (uint)this.pos.X, (uint)this.pos.Y, msg);

                        }
                    }
                }
            }
        }
        public void TryToGetChunks()
        {
            var needupd = false;
            var xd = (int)Math.Floor(this.pos.X / 32);
            var yd = (int)Math.Floor(this.pos.Y / 32);
            if ((xd >= 0 && yd >= 0) && (xd <= XServer.THIS.chunkscx && yd <= XServer.THIS.chunkscy))
            {
                chunkx = xd;
                chunky = yd;
            }
            if (this.chunkx != this.lastupdchchunkx || this.chunky != this.lastupdchchunky)
            {
                needupd = true;
                this.RemoveMeFromChunk();
                this.lastupdchchunkx = this.chunkx;
                this.lastupdchchunky = this.chunky;
            }
            if (needupd)
            {
                GimmePacks();
                for (var xxx = -2; xxx <= 2; xxx++)
                {
                    for (var yyy = -2; yyy <= 2; yyy++)
                    {
                        var x = ((this.chunkx + xxx) * 32);
                        var y = ((this.chunky + yyy) * 32);
                        if (((this.chunkx + xxx) >= 0 && (this.chunky + yyy) >= 0) && ((this.chunkx + xxx) < XServer.THIS.chunkscx && (this.chunky + yyy) < XServer.THIS.chunkscy))
                        {
                            if (Chunk.chunks[x / 32, y / 32] != null)
                            {
                                connection.SendCells(32, 32, (uint)x, (uint)y, Chunk.chunks[(this.chunkx + xxx), (this.chunky + yyy)].cells);
                            }
                        }
                    }
                }
            }
        }
        public void GetCry(uint x, uint y, byte cell)
        {
            CDob();
            for (int p = 0; p < World.THIS.crys.Count; p++)
            {
                if (World.THIS.crys[p] == cell)
                {
                    var d = lvl[0];
                    this.crys.AddCrys(p, d);
                    if (p == 0)
                    {
                        SendDFToBots(2, x, y, d, 0);
                    }
                    if (p == 1)
                    {
                        SendDFToBots(2, x, y, d, 3);
                    }
                    if (p == 2)
                    {
                        SendDFToBots(2, x, y, d, 1);
                    }
                    if (p == 3)
                    {
                        SendDFToBots(2, x, y, d, 2);
                    }
                    if (p == 4)
                    {
                        SendDFToBots(2, x, y, d, 4);
                    }
                    if (p == 5)
                    {
                        SendDFToBots(2, x, y, d, 5);
                    }
                }
            }
        }
        public void Move(uint x, uint y, int dir)
        {
            if (!World.THIS.ValidCoord(x, y))
            {
                return;
            }
            var c = World.cellps[World.THIS.GetCell(x, y)];
            /*
            if (World.THIS.GetCellConst(x, y) == null || !World.THIS.GetCellConst(x, y).is_empty)
            {
                this.connection.Send("@T", $"{this.pos.X}:{this.pos.Y}");
                return;
            }
            */
            var newpos = new Vector2(x, y);
            if (Vector2.Distance(pos, newpos) < 1.2f)
            {
                var pack = World.packmap[(uint)newpos.X + (uint)newpos.Y * World.width];
                if (pack != null && win == "" && pack.CanOpen(this))
                {
                    win = pack.winid;
                    cpack = pack;
                }
                if (win.StartsWith("!!"))
                    {
                    if (win.StartsWith("!!settings"))
                    {
                        settings.Open(settings.winid,this);
                    }
                    this.connection.Send("@T", $"{this.pos.X}:{this.pos.Y}");
                    return;
                }
                if (win != "" && (World.packmap[(uint)this.pos.X + (uint)this.pos.Y * World.width] != null))
                {
                    World.packmap[(uint)this.pos.X + (uint)this.pos.Y * World.width].Open(this, this.win);
                    this.connection.Send("@T", $"{this.pos.X}:{this.pos.Y}");
                    return;
                }
                pos = newpos;
                this.dir = dir;
            }
            else
            {
                this.connection.Send("@T", $"{this.pos.X}:{this.pos.Y}");
                return;
            }
            if (!c.is_empty)
            {
                if (c.id == 90)
                {
                    CBox(x, y);
                }
                this.Hurt(c.fall_damage);
            }
            TryToGetChunks();
        }
    }
}
