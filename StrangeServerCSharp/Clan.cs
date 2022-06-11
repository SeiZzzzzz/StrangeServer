using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrangeServerCSharp
{
    public class Clan
    {
        public Clan()
        {
            memberList = new ClanMemberList();
        }
        public static void InitClans()
        {
            if (BDClass.THIS.clans.Count() == 0)
            {
                for (int i = 1; i <= 250; i++)
                {
                    BDClass.THIS.clans.Add(new Clan() { abr = "",owner = "", name = ""});
                }
            }
            BDClass.THIS.SaveChanges();
        }
        public static void CreateClan(int id,string name, string abr, Player p)
        {
            var clan = BDClass.THIS.clans.First(i => i.id == id);
            clan.name = name;
            clan.abr = abr;
            clan.owner = p.id.ToString();
            p.clanid = id;
            p.SendClan();
            BDClass.THIS.SaveChanges();
        }
        public static Clan finclan(int id)
        {
            return BDClass.THIS.clans.First(i => i.id == id);
        }
        public static List<Clan> GetAvlClanIcon()
        {
            List<Clan> l = new List<Clan>();
            var col = 8;
            for (int i = 0; i < col; i++)
            {
                int r = World.Random.Next(1, 200);
                var c = BDClass.THIS.clans.Where(j => string.IsNullOrEmpty(j.owner)).ToList();
                var clan = c.ElementAt(new Index(World.Random.Next(1, c.Count())));
                l.Add(clan);
            }
            return l;
        }
        public string abr { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public ClanMemberList memberList { get; set; }
    }
    public class ClanMemberList : List<int>
    {
        public int id { get; set; }
    }
}
