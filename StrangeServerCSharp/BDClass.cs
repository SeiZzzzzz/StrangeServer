using Microsoft.EntityFrameworkCore;

namespace StrangeServerCSharp
{
    public class BDClass : DbContext
    {
        public DbSet<Player> players { get; set; }
        public DbSet<Building> packs { get; set; }
        public DbSet<Market> markets { get; set; }
        public DbSet<Box> boxes { get; set; }
        public BDClass()
        {
            Console.WriteLine(Database.EnsureCreated());
            THIS = this;
        }
        public static bool NickAvl(string nick)
        {
            Console.WriteLine(THIS.players.Where(p => p.name == nick).Count());
            return THIS.players.Where(p => p.name == nick).Count() > 0;
        }
        public Player CreatePlayer()
        {
            var player = new Player { pos = new System.Numerics.Vector2(340, 15), resppos = new System.Numerics.Vector2(340, 15) };
            THIS.players.Add(player);
            THIS.SaveChanges();
            return player;
        }
        public static BDClass THIS;
        public static void Save()
        {
            foreach (var pack in World.packmap)
            {
                if (pack != null)
                {
                    THIS.Set<Building>().Add(pack);
                }
            }
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
