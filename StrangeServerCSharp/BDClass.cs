using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Player GetPlayer(int id,Session s, out bool needr)
        {
            try
            {
                needr = false;
                return players.First(p => p.id == id);
            }
            catch(Exception) {
                var playerr = new Player { pos = new System.Numerics.Vector2(340, 15), resppos = new System.Numerics.Vector2(340, 15) };
                THIS.players.Add(playerr);
                playerr.name = playerr.id.ToString();
                THIS.SaveChanges();
                s.Send("AH", playerr.id + "_" + playerr.hash);
                needr = true;
                return playerr;
            }
            needr = true;
            var player = new Player { pos = new System.Numerics.Vector2(340, 15), resppos = new System.Numerics.Vector2(340, 15) };
            THIS.players.Add(player);
            THIS.SaveChanges();
            var n = players.First(p => p.name == p.name);
            s.Send("AH", n.id + "_" + n.hash);
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
            THIS.SaveChanges();

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
