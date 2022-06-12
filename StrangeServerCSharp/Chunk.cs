namespace StrangeServerCSharp
{
    public class Chunk
    {
        public byte[] cells = new byte[32 * 32];
        public Dictionary<int, string> bots = new Dictionary<int, string>();
        public Dictionary<System.Numerics.Vector2, string> packs = new Dictionary<System.Numerics.Vector2, string>();
        public static Chunk[,] chunks;
        public System.Numerics.Vector2 chpos;

        public Chunk(uint x, uint y)
        {
            this.chpos = new System.Numerics.Vector2(x, y);
        }

        public void SendCellToBots(uint x, uint y, byte cell)
        {
            for (var xxx = -2; xxx <= 2; xxx++)
            {
                for (var yyy = -2; yyy <= 2; yyy++)
                {
                    if (((this.chpos.X + xxx) >= 0 && (this.chpos.Y + yyy) >= 0) &&
                        ((this.chpos.X + xxx) < XServer.THIS.chunkscx && (this.chpos.Y + yyy) < XServer.THIS.chunkscy))
                    {
                        var cx = (this.chpos.X + xxx);
                        var cy = (this.chpos.Y + yyy);
                        var ch = Chunk.chunks[(int)cx, (int)cy];
                        foreach (var id in ch.bots)
                        {
                            var player = XServer.players[id.Key];
                            player.connection.SendCell(x, y, cell);
                        }
                    }
                }
            }
        }

        public void AddPack(uint x, uint y)
        {
            if (this != null)
            {
                var v = new System.Numerics.Vector2(x, y);
                if (!packs.ContainsKey(v))
                {
                    packs.Add(v, "");
                }
            }
        }

        public void RemovePack(uint x, uint y)
        {
            if (this != null)
            {
                var v = new System.Numerics.Vector2(x, y);
                if (packs.ContainsKey(v))
                {
                    packs.Remove(v);
                }
            }
        }

        public void Update()
        {
            for (uint y = 0; y < 32; y++)
            {
                for (uint x = 0; x < 32; x++)
                {
                    var chx = (int)chpos.X;
                    var chy = (int)chpos.Y;
                    if (Chunk.chunks[chx, chy] != null)
                    {
                        var cell = World.THIS.GetCell((uint)((chx * 32) + x), (uint)(((chy * 32) + y)));
                        if (cells[x + y * 32] != cell)
                        {
                            cells[x + y * 32] = cell;
                            SendCellToBots((uint)((chx * 32) + x), (uint)((chy * 32) + y), cell);
                        }
                    }
                }
            }
        }

        public void AddBot(int id)
        {
            if (this != null)
            {
                this.bots.Add(id, "");
            }
        }
    }
}