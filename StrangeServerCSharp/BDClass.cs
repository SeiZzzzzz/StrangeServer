using Microsoft.EntityFrameworkCore;

namespace StrangeServerCSharp
{
    public class BDClass : DbContext
    {
        public DbSet<Player> players { get; set; }
        public DbSet<Building> packs { get; set; }
        public DbSet<Market> markets { get; set; }
        public DbSet<Resp> resps { get; set; }
        public DbSet<Gun> guns { get; set; }
        public DbSet<Box> boxes { get; set; }
        public DbSet<Settings> settings { get; set; }
        public DbSet<Clan> clans { get; set; }
        public DbSet<Prog> progs { get; set; }


        public BDClass()
        {
            Database.EnsureCreated();
        }

        public static bool NickAvl(string nick)
        {
            using var db = new BDClass();
            try
            {
                Console.WriteLine(db.players.Count(p => p.name == nick));

                return db.players.Count(p => p.name == nick) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Player CreatePlayer(string name, string passwd)
        {
            using var db = new BDClass();
            var player = new Player { pos = new System.Numerics.Vector2(351, 22) };
            try
            {
                player.resp = db.resps.First();
            }
            catch (Exception)
            {
                player.resp = Resp.Build(player.x, player.y, player);
                player.resp.ownerid = 0;
                player.resp.Rebild();
                player.settings = new Settings();
                db.settings.Add(player.settings);
            }

            player.name = name;
            player.settings = new Settings();
            player.passwd = passwd;
            db.settings.Add(player.settings);
            db.players.Add(player);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return player;
            }

            return player;
        }

        public static void Save()
        {
            using var db = new BDClass();
            foreach (var box in World.boxmap)
            {
                if (box != null)
                {
                    db.boxes.Add(box);
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbType = Config.THIS.DBType;
            switch (dbType)
            {
                case "sqlite":
                    optionsBuilder.UseSqlite("Data Source=mines.db");
                    break;
                case "localdb":
                default:
                    optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;MultipleActiveResultSets=true;Database=X;Trusted_Connection=True;");
                    break;
            }
        }

        public static void Load()
        {
            using var db = new BDClass();
            try
            {
                foreach (var i in db.markets.ToList())
                {
                    i.Rebild();
                    World.packmap[i.x + i.y * World.height] = i;
                    var v = World.THIS.GetChunkPosByCoords(i.x, i.y);
                    Chunk.chunks[(uint)v.X, (uint)v.Y].AddPack(i.x, i.y);
                }

                foreach (var i in db.resps.ToList())
                {
                    i.Rebild();
                    World.packmap[i.x + i.y * World.height] = i;
                    var v = World.THIS.GetChunkPosByCoords(i.x, i.y);
                    Chunk.chunks[(uint)v.X, (uint)v.Y].AddPack(i.x, i.y);
                }

                foreach (var i in db.guns.ToList())
                {
                    i.Rebild();
                    World.packmap[i.x + i.y * World.height] = i;
                    var v = World.THIS.GetChunkPosByCoords(i.x, i.y);
                    Chunk.chunks[(uint)v.X, (uint)v.Y].AddPack(i.x, i.y);
                }

                World.packlist = db.packs.ToList();
                foreach (var i in db.boxes.ToList())
                {
                    World.boxmap[i.x + i.y * World.height] = i;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}