using Microsoft.EntityFrameworkCore;

namespace StrangeServerCSharp
{
    public class BDClass : DbContext
    {
        public DbSet<Player> players { get; set; }
        public DbSet<Building> packs { get; set; }
        public DbSet<Market> markets { get; set; }
        public DbSet<Resp> resps { get; set; }
        public DbSet<Box> boxes { get; set; }
        //public DbSet<Clan> clans { get; set; }
        public BDClass()
        {
            Console.WriteLine(Database.EnsureCreated());
            THIS = this;
        }
        public static bool NickAvl(string nick)
        {
            try
            {
                Console.WriteLine(THIS.players.Where(p => p.name == nick).Count());

                return THIS.players.Where(p => p.name == nick).Count() > 0;
            }
            catch (Exception) { return false; }
        }
        public Player CreatePlayer(string name, string passwd)
        {
            var player = new Player { pos = new System.Numerics.Vector2(351, 22) };
            try
            {
                player.resp = BDClass.THIS.resps.First();
            }
            catch (Exception) { player.resp = Resp.Build(player.x, player.y, player); player.resp.ownerid = 0; }
            player.name = name;
            player.passwd = passwd;
            THIS.players.Add(player);
            try
            {
                THIS.SaveChanges();
            }
            catch (Exception) { return player; }
            return player;
        }
        public static BDClass THIS;
        public static void Save()
        {
            foreach (var box in World.boxmap)
            {
                if (box != null)
                {
                    THIS.boxes.Add(box);
                }
            }
            try
            {
                THIS.SaveChanges();
            }
            catch (Exception ex) { }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=X;Trusted_Connection=True;");
        }
        public static void Load()
        {
            try
            {
                foreach (var i in THIS.markets.ToList())
                {
                    i.Rebild();
                    World.packmap[i.x + i.y * World.height] = i;
                    var v = World.THIS.GetChunkPosByCoords(i.x, i.y);
                    Chunk.chunks[(uint)v.X, (uint)v.Y].AddPack(i.x, i.y);
                }
                foreach (var i in THIS.resps.ToList())
                {
                    i.Rebild();
                    World.packmap[i.x + i.y * World.height] = i;
                    var v = World.THIS.GetChunkPosByCoords(i.x, i.y);
                    Chunk.chunks[(uint)v.X, (uint)v.Y].AddPack(i.x, i.y);
                }
                foreach (var i in THIS.boxes.ToList())
                {
                    World.boxmap[i.x + i.y * World.height] = i;
                }
            }
            catch (Exception) { }
        }
    }
}
