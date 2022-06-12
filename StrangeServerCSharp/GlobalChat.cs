namespace StrangeServerCSharp
{
    public static class GlobalChat
    {
        public class CHPacket
        {
            public CHPacket(string[] h, string ch)
            {
                this.h = h;
                this.ch = ch;
                this.FullPacket = Newtonsoft.Json.JsonConvert.SerializeObject(this).Replace("\\n", "");
                SendMsg();
            }

            public void SendMsg()
            {
                foreach (var pl in XServer.players)
                {
                    pl.Value.connection.Send("mO", "FED:F");
                    pl.Value.connection.Send("mU", this.FullPacket);
                }
            }

            public string[] h;
            public string ch;

            public static string GetBody(Player player, string msg)
            {
                id++;
                time++;
                return CHPacket.id + "±" + 15 + "±" + player.clanid + "±" + CHPacket.time + "±" + player.name + "±" +
                       msg + "±" + player.id;
            }

            public static int id = 0;
            public static int time = 0;
            public string FullPacket;
        }
    }
}