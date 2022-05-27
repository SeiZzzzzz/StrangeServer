using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrangeServerCSharp
{
    public class World
    {
        public static Random Random = new Random();
        public static byte[] map;
        public static byte[] roadmap;
        public static int width = 0;
        public static int height = 0;
        public static World THIS;
        public static PeriodicTimer wupdtimer = null;
        public static PeriodicTimer cspdtimer = null;
        public static Newtonsoft.Json.Linq.JObject cellspool = null;
        public static Cell[] cells;
        public static Cell[] cellps = new Cell[255];
        public List<int> crys = new List<int>()
        {
            107,
            109,
            108,
            110,
            111,
            112
        };
        public List<int> xcry = new List<int>()
        {
            71,
            72,
            73,
            74,
            75
        };
        public List<int> road = new List<int>()
        {
            32,
            35,
            37,
            36,
            39
        };
        public List<int> pack = new List<int>()
        {
            38,
            37,
            106,
        };
        public static void InitCell(string name)
        {
            if (cellspool[name] == null)
            {
                return;
            }
            JsonSerializer serializer = new JsonSerializer();
            cellps[int.Parse(name)] = (Cell)serializer.Deserialize(new JTokenReader(cellspool[name]), typeof(Cell));
        }

        public class Cell
        {
            public Cell CloneCell()
            {
                return MemberwiseClone() as Cell;
            }

            public int id { get; set; }
            public string name { get; set; }
            public bool can_build_over { get; set; }
            public bool is_alive { get; set; }
            public bool is_pickable { get; set; }
            public bool is_empty { get; set; }
            public bool is_falling { get; set; }
            public bool is_destructible { get; set; }
            public bool is_destructible_volcano { get; set; }
            public int boom_percent { get; set; }
            public int boom_proton_percent { get; set; }
            public int damage { get; set; }
            public int fall_damage { get; set; }
            public int HP { get; set; }
        }
        public bool ValidCoord(uint x, uint y)
        {
            if ((x >= 0 && y >= 0) && (x < width && y < height))
            {
                return true;
            }
            return false;
        }
        public static void SendPack(char type, uint px, uint py, int cid, int off)
        {
            var xd = (int)Math.Floor((decimal)px / 32);
            var yd = (int)Math.Floor((decimal)py / 32);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((xd + xxx) >= 0 && (yd + yyy) >= 0) && ((xd + xxx) < XServer.THIS.chunkscx && (yd + yyy) < XServer.THIS.chunkscy))
                    {
                        var x = (xd + xxx);
                        var y = (yd + yyy);
                        var ch = Chunk.chunks[x, y];

                        foreach (var id in ch.bots)
                        {
                            var player = XServer.players[id.Key];
                            player.connection.AddPack(type, px, py, cid, off);
                        }
                    }
                }
            }
        }
        public void SendDFToBotsGlobal(int fx, uint fxx, uint fxy, int dir, int bid = 0, int col = 0)
        {
            var xd = (int)Math.Floor((decimal)fxx / 32);
            var yd = (int)Math.Floor((decimal)fxy / 32);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((xd + xxx) >= 0 && (yd + yyy) >= 0) && ((xd + xxx) < XServer.THIS.chunkscx && (yd + yyy) < XServer.THIS.chunkscy))
                    {
                        var x = (xd + xxx);
                        var y = (yd + yyy);
                        var ch = Chunk.chunks[x, y];

                        foreach (var id in ch.bots)
                        {
                            var player = XServer.players[id.Key];
                            player.connection.AddDFX(fx, dir, fxx, fxy, bid, col);

                        }
                    }
                }
            }
        }
        public async void AsyncAction(int secdelay, Action act)
        {
            await Task.Run(delegate ()
            {
                System.Threading.Thread.Sleep(secdelay * 100);
                act();
            });
        }
        public static void ClearPack(uint px, uint py)
        {
            var xd = (int)Math.Floor((decimal)px / 32);
            var yd = (int)Math.Floor((decimal)py / 32);
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((xd + xxx) >= 0 && (yd + yyy) >= 0) && ((xd + xxx) < XServer.THIS.chunkscx && (yd + yyy) < XServer.THIS.chunkscy))
                    {
                        var x = (xd + xxx);
                        var y = (yd + yyy);
                        var ch = Chunk.chunks[x, y];

                        foreach (var id in ch.bots)
                        {
                            var player = XServer.players[id.Key];
                            player.connection.ClearPack(px, py);


                        }
                    }
                }
            }
        }
        public void Boom(uint x, uint y)
        {
            SendPack('B', x, y, 0, 0);
            canboom[x + y * height] = true;
            AsyncAction(7, () =>
             {
                 for (int _x = -4; _x < 4; _x++)
                 {
                     for (int _y = -4; _y < 4; _y++)
                     {
                         if (System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y), new System.Numerics.Vector2((x + _x), (y + _y))) <= 3.5f)
                         {
                             foreach (var id in ContPlayers((uint)(x + _x), (uint)(y + _y)))
                             {
                                 XServer.players[id].Hurt(40);
                             }
                             if (ValidCoord((uint)(x + _x), (uint)(y + _y)) && (Random.Next(0, 100) < GetCellConst((uint)(x + _x), (uint)(y + _y)).boom_percent))
                             {
                                 DestroyWithRoadCell((uint)(x + _x), (uint)(y + _y));
                             }
                         }
                     }
                 }
                 SendDFToBotsGlobal(1, x, y, 3, 0, 0);
                 ClearPack(x, y);
                 canboom[x + y * height] = false;
             });
        }
        public void Prot(uint x, uint y)
        {
            SendPack('B', x, y, 0, 1);
            canboom[x + y * height] = true;
            AsyncAction(10, () =>
            {
                for (int _x = -1; _x < 2; _x++)
                {
                    for (int _y = -1; _y < 2; _y++)
                    {
                        foreach (var id in ContPlayers((uint)(x + _x), (uint)(y + _y)))
                        {
                            XServer.players[id].Hurt(5);
                        }
                        if (System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y), new System.Numerics.Vector2((x + _x), (y + _y))) <= 2f)
                        {
                            if (ValidCoord((uint)(x + _x), (uint)(y + _y)) && (Random.Next(0, 100) < GetCellConst((uint)(x + _x), (uint)(y + _y)).boom_proton_percent))
                            {
                                DestroyWithRoadCell((uint)(x + _x), (uint)(y + _y));
                            }
                        }
                    }
                }
                SendDFToBotsGlobal(1, x, y, 1, 0, 1);
                ClearPack(x, y);
                canboom[x + y * height] = false;
            });
        }
        public bool isCrys(int cell)
        {
            return crys.Contains(cell);
        }
        public Phys phys;
        public static Box[] boxmap;
        public static Building[] packmap;
        public World(int width, int height, byte[] map, byte[] roadmap)
        {
            cellspool = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("MapBlocks.json"));
            for (int i = 0; i < 255; i++)
            {
                InitCell(i.ToString());
            }
            World.map = map;
            World.roadmap = roadmap;
            World.width = width;
            World.height = height;
            cells = new Cell[width * height];
            THIS = this;
            CreateChunks();
            WorldUPD();
            phys = new Phys();
            Phys.Start();
            packmap = new Building[width * height];
            boxmap = new Box[width * height];
            canboom = new bool[width * height];
            c117UPD();
            BDClass.Load();
        }
        public static bool[] canboom;
        public static async void WorldUPD()
        {
            wupdtimer = new PeriodicTimer(TimeSpan.FromSeconds(3));

            while (await wupdtimer.WaitForNextTickAsync())
            {
                THIS.UpdateWorld();
            }
        }
        public static async void c117UPD() //кс
        {
            cspdtimer = new PeriodicTimer(TimeSpan.FromSeconds(60));

            while (await cspdtimer.WaitForNextTickAsync())
            {
                THIS.Updc119();
            }
        }
        public void Updc119()
        {
            for (uint y = 0; y < 1000; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    if (GetCell(x, y) == 117)
                    {
                        this.phys.TryToActivatec117(x, y);
                    }
                }
            }
        }
        public System.Numerics.Vector2 GetChunkPosByCoords(uint x, uint y)
        {
            return new System.Numerics.Vector2((uint)Math.Floor((decimal)x / 32), (uint)Math.Floor((decimal)y / 32));
        }
        public void UpdateWorld()
        {
            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    this.phys.TryToActivate(x, y, map[x + y * height]);
                }
            }
        }
        public static Stack<int> ContPlayers(uint bx, uint by)
        {
            var xd = (int)Math.Floor((decimal)bx / 32);
            var yd = (int)Math.Floor((decimal)by / 32);
            var st = new Stack<int>();
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((xd + xxx) >= 0 && (yd + yyy) >= 0) && ((xd + xxx) < XServer.THIS.chunkscx && (yd + yyy) < XServer.THIS.chunkscy))
                    {
                        var x = (xd + xxx);
                        var y = (yd + yyy);
                        var ch = Chunk.chunks[x, y];

                        foreach (var id in ch.bots)
                        {
                            if (XServer.players.ContainsKey(id.Key))
                            {
                                var player = XServer.players[id.Key];

                                if (player.pos.X == bx && player.pos.Y == by)
                                {
                                    st.Push(id.Key);
                                }
                            }

                        }
                    }
                }
            }
            return st;
        }
        public void DestroyCell(uint x, uint y)
        {
            cells[x + y * height] = cellps[roadmap[x + y * height]].CloneCell();
            World.map[x + y * height] = roadmap[x + y * height];
            System.Numerics.Vector2 chp = GetChunkPosByCoords(x, y);
            Chunk.chunks[(int)chp.X, (int)chp.Y].Update();
        }
        public void DestroyWithRoadCell(uint x, uint y)
        {
            byte r = roadmap[x + y * height];
            if (r == 35)
            {
                roadmap[x + y * height] = 32;
            }
            cells[x + y * height] = cellps[roadmap[x + y * height]].CloneCell();
            World.map[x + y * height] = roadmap[x + y * height];
            System.Numerics.Vector2 chp = GetChunkPosByCoords(x, y);
            Chunk.chunks[(int)chp.X, (int)chp.Y].Update();
        }
        public bool isRoad(byte cell)
        {
            return road.Contains(cell);
        }
        public bool isPack(byte cell)
        {
            return pack.Contains(cell);
        }
        public void SetCell(uint x, uint y, byte cell, int hp = -1)
        {
            if (cell == 0)
            {
                return;
            }
            if (cellps[(int)cell] == null)
            {
                return;
            }
            var c = cellps[(int)cell].CloneCell();
            if (isRoad(cell))
            {
                cells[x + y * height] = c;
                roadmap[x + y * height] = cell;
                World.map[x + y * height] = cell;
                System.Numerics.Vector2 v = GetChunkPosByCoords(x, y);
                Chunk.chunks[(int)v.X, (int)v.Y].Update();
                return;
            }
            if (cells[x + y * height].is_empty)
            {
                roadmap[x + y * height] = World.map[x + y * height];
            }
            if (hp != -1)
            {
                c.HP = hp;
            }
            cells[x + y * height] = c;
            World.map[x + y * height] = cell;
            var h = ContPlayers(x, y);
            foreach (var id in h)
            {
                XServer.players[id].Hurt(c.damage);
            }
            if (cell == 90)
            {
                try
                {
                    Box.CollectBox(x, y, XServer.players[h.First()]);
                }
                catch (Exception e) { }
            }
            System.Numerics.Vector2 chp = GetChunkPosByCoords(x, y);
            Chunk.chunks[(int)chp.X, (int)chp.Y].Update();
        }
        public byte GetCell(uint x, uint y)
        {
            byte cell = World.map[x + y * height];
            if (cellps[cell].is_empty)
            {
                return roadmap[x + y * height];
            }
            return cell;
        }
        public Cell GetCellConst(uint x, uint y)
        {
            if (ValidCoord(x, y))
            {
                return World.cells[x + y * height];
            }
            return null;
        }
        public static long[] costs = new long[] { 10, 25, 20, 25, 21, 50 };
        public static long[] costsbuy = new long[] { 10, 25, 20, 25, 21, 50 };
        public void CreateChunks()
        {
            for (uint chx = 0; chx < XServer.THIS.chunkscx; chx++)
            {
                for (uint chy = 0; chy < XServer.THIS.chunkscy; chy++)
                {
                    Chunk.chunks[chx, chy] = new Chunk(chx, chy);
                    for (uint y = 0; y < 32; y++)
                    {
                        for (uint x = 0; x < 32; x++)
                        {
                            if (Chunk.chunks[chx, chy] != null)
                            {
                                byte cell = GetCell(((chx * 32) + x), (((chy * 32) + y)));
                                Chunk.chunks[chx, chy].cells[x + y * 32] = cell;
                                cells[((chx * 32) + x) + ((chy * 32) + y) * height] = cellps[(int)cell].CloneCell();
                            }
                        }
                    }
                }
            }
        }
    }
}
