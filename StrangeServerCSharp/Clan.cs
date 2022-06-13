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
                for (var i = 1; i <= 219; i++)
                {
                    BDClass.THIS.clans.Add(new Clan() { abr = "", owner = "", name = "" });
                }
            }

            BDClass.THIS.SaveChanges();
        }

        public static void CreateClan(int id, string name, string abr, Player p)
        {
            var clan = BDClass.THIS.clans.First(i => i.id == id);
            clan.name = name;
            clan.abr = abr;
            clan.owner = p.id.ToString();
            p.clanid = id;
            p.SendClan();
            BDClass.THIS.SaveChanges();
        }

        public void AddReq(int id)
        {
            if (!reqs.Contains(id))
            {
                reqs.Add(id);
            }
        }

        public void RemoveReq(int id)
        {
            if (reqs.Contains(id))
            {
                reqs.Remove(id);
            }
        }

        public static string winid = "!!clan";
        public List<int> reqs = new List<int>();

        public static void Open(string t, Player p)
        {
            var builder = new HorbBuilder();
            if (t == winid)
            {
                if (p.clanid == 0)
                {
                    return;
                }

                var clan = BDClass.THIS.clans.First(clan => clan.id == p.clanid);
                builder.SetTitle(clan.name)
                    .AddCard("c", p.clanid.ToString(),
                        $"<color=white>Winx </color>\nУчастники: <color=white> {1 + clan.GetMemberList().Count} </color> ")
                    .AddTab("ОБЗОР", "");
                builder.AddTab("СПИСОК", "list");
                if (clan.reqs.Count > 0)
                {
                    builder.AddTab($"<color=#ff8888ff>ЗАЯВКИ ({clan.reqs.Count})</color>", "reqs");
                }
            }
            else if (t.StartsWith("reqs"))
            {
                if (p.clanid == 0)
                {
                    return;
                }

                var clan = BDClass.THIS.clans.First(clan => clan.id == p.clanid);
                builder.SetTitle(clan.name);
                var num = 0;
                builder.AddTab("ОБЗОР", "clan");
                builder.AddTab("СПИСОК", "list");
                if (clan.reqs.Count > 0)
                {
                    builder.AddTab($"<color=#ff8888ff>ЗАЯВКИ ({clan.reqs.Count})</color>", "");
                }

                foreach (var id in clan.reqs)
                {
                    var player = BDClass.THIS.players.First(i => i.id == id);
                    num++;
                    builder.AddListLine($"{num} <color=white>{player.name}</color>", "...", $"req:{id}:{clan.id}");
                }
            }
            else if (t.StartsWith("list"))
            {
                if (p.clanid == 0)
                {
                    return;
                }

                var clan = BDClass.THIS.clans.First(clan => clan.id == p.clanid);
                builder.SetTitle(clan.name);
                builder.AddClanList();
                var num = 0;
                builder.AddTab("ОБЗОР", "clan");
                builder.AddTab("СПИСОК", "");
                if (clan.reqs.Count > 0)
                {
                    builder.AddTab($"<color=#ff8888ff>ЗАЯВКИ ({clan.reqs.Count})</color>", "");
                }

                foreach (var id in clan.GetMemberList())
                {
                    var player = BDClass.THIS.players.First(i => i.id == id);
                    num++;
                    builder.AddClanListLine("0", $"{num}.<color=white>{player.name}</color>", "", "listrow:" + id);
                }
            }

            builder.Send("!!clan." + t, p);
        }

        public static void DeleteClan(int id, Player p)
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

        public void Addmember(int memberid)
        {
            Player player = null;
            try
            {
                player = BDClass.THIS.players.First(p => p.id == memberid);
            }
            catch (Exception ex)
            {
            }

            if (player == null)
            {
                return;
            }

            player.clanid = id;
            player.SendClan();
            if (!GetMemberList().Contains(memberid))
            {
                memberList += ":" + memberid.ToString();
            }

            BDClass.THIS.SaveChanges();
        }

        public static List<Clan> GetAvlClanIcon()
        {
            var l = new List<Clan>();
            var col = 8;
            for (var i = 0; i < col; i++)
            {
                var r = World.Random.Next(1, 219);
                var c = BDClass.THIS.clans.Where(j => string.IsNullOrEmpty(j.owner)).ToList();
                var clan = c.ElementAt(new Index(World.Random.Next(1, c.Count())));
                l.Add(clan);
            }

            return l;
        }

        public List<int> GetMemberList()
        {
            var m = memberList.Split(":");
            var mem = new int[] { };
            foreach (var mid in m)
            {
                if (!string.IsNullOrEmpty(mid))
                {
                    mem = mem.Concat(new int[] { int.Parse(mid) }).ToArray();
                }
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