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
            memberList = "";
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
        public static void Open(Player p)
        {
            if (p.clanid == 0)
            {
                return;
            }
            var clan = BDClass.THIS.clans.First(clan => clan.id == p.clanid);
            var c = new HorbConst();
            c.AddTitle(clan.name);
            c.AddCard("c", p.clanid.ToString(), $"<color=white>Winx </color>\nУчастники: <color=white> {1 + clan.GetMemberList().Count} </color> ");
            c.AddTab("ОБЗОР", "");c.AddTab("СПИСОК", "list");
            c.Send("!!clan",p);
        }
        public static void DeleteClan(int id,Player p)
        {
            var clan = BDClass.THIS.clans.First(i => i.id == id);
            clan.name = "";
            clan.abr = "";
            clan.owner = "";
            p.clanid = 0;
            p.SendClan();
            BDClass.THIS.SaveChanges();
        }
        public static Clan finclan(int id)
        {
            return BDClass.THIS.clans.First(i => i.id == id);
        }
        public static void Addmember(int clanid,int memberid)
        {
            var clan = BDClass.THIS.clans.First(i => i.id == clanid);
            clan.memberList += ":" + memberid.ToString();
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
        public List<int> GetMemberList()
        {
            string[] m = memberList.Split(":");
            int[] mem = new int[] { };
            foreach (string mid in m)
            {
                if (!string.IsNullOrEmpty(mid)) { mem = mem.Concat(new int[] { int.Parse(mid) }).ToArray(); }
            }
            return mem.ToList();
        }
        public string abr { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public string memberList { get; set; }
    }
}
