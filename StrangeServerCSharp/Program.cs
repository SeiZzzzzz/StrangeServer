using NetCoreServer;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace StrangeServerCSharp
{
    public class Program
    {
        static void Main(string[] a)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);
            int port = 8090;
            var server = new XServer(IPAddress.Any, port);
            server.Start();
            for (; ; )
            {
                string line = Console.ReadLine();
                if (line.StartsWith("restart"))
                {
                    server.Stop();
                    server = new XServer(IPAddress.Any, port);
                    server.Start();
                }
                else if (string.IsNullOrEmpty(line))
                    break;
            }
            server.Stop();
        }
        private static void OnExit(object s, EventArgs arg)
        {
                XServer.THIS.SaveMap();
        }

    }
    public class XServer : TcpServer
    {
        public const int width = 3200;
        public const int height = 3200;
        public int chunkscx = 0;
        public int chunkscy = 0;
        public static World world = null;
        public static XServer THIS;
        public static Dictionary<int, Player> players = new Dictionary<int, Player>();
        public static Dictionary<Guid, Player> playersconn = new Dictionary<Guid, Player>();
        public XServer(IPAddress address, int port) : base(address, port)
        {
            Console.WriteLine("Started");
            THIS = this;
            chunkscx = width / 32;
            chunkscy = height / 32;
            Chunk.chunks = new Chunk[chunkscx, chunkscy];
            var map = File.ReadAllBytes("cum.map");
            var roadmap = new byte[width * height];
            if (File.Exists("cumroad.map"))
            {
                roadmap = File.ReadAllBytes("cumroad.map");
            }
            else
            {
                for (int i = 0; i < roadmap.Length; i++)
                {
                    roadmap[i] = 32;
                }
            }
            world = new World(width, height, map, roadmap);

        }
        public void SaveMap()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (World.map[x + y * height] == 90)
                    {
                        World.map[x + y * height] = 32;
                    }
                    else if (World.THIS.isPack(World.map[x + y * height]) || World.THIS.isPack(World.roadmap[x + y * height]))
                    {
                        World.map[x + y * height] = 32;
                        World.roadmap[x + y * height] = 32;
                    }
                }
            }
            File.WriteAllBytes("cum.map", World.map);
            File.WriteAllBytes("cumroad.map", World.roadmap);
        }
        protected override TcpSession CreateSession() { return new Session(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Nigga Balls {error}");
        }
    }
    public class Session : TcpSession
    {
        public async void AsyncAction(int delay, Action act)
        {
            await Task.Run(delegate ()
            {
                System.Threading.Thread.Sleep(delay);
                act();
            });
        }
        public static int id = 1;
        public int bid;
        public Player player;
        public int lst = 0;
        public int prp = 0;
        public static long online = 0;
        public Session(TcpServer server) : base(server) { }
        protected override void OnConnected()
        {
            this.player = new Player(this, id, 340, 15);
            Console.WriteLine("connected " + player.id);
            XServer.players.Add(id, player);
            XServer.playersconn.Add(this.Id, player);
            Session.id++;
            Console.WriteLine(this.Id.ToString());
            Send("AU", Server.ConnectedSessions.ToString());
            online++;
            foreach (var player in XServer.players)
            {
                player.Value.connection.SendOnline();
            }
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected " + player.id);
            if (player.timer != null)
            {
                this.player.timer.Dispose();
            }
            this.player.Death();
            this.player.ForceRemove();
            online--;
        }
        public void SendOnline()
        {
            this.Send("ON", online +":0");
        }
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            Packet p = new Packet(buffer);
            if (p.eventType == "AU")
            {
                Send("PI", "0:0:0");
                Send("cf", "{\"width\":" + XServer.width + ",\"height\":" + XServer.height + ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                Send("CF", "{\"width\":" + XServer.width + ",\"height\":" + XServer.height + ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                Send("sp", "125:57:200");
                Send("BA", "0");
                Send("BD", "0");
                Send("GE", " ");
                Send("@T", $"{this.player.pos.X}:{this.player.pos.Y}");
                Send("BI", "{\"x\":" + this.player.pos.X + ",\"y\":" + this.player.pos.Y + ",\"id\":" + player.id + ",\"name\":\"" + player.name + "\"}");
                Send("sp", "25:20:100000");
                Send("#S", "#cc#10#snd#0#mus#0#isca#0#tsca#0#mous#1#pot#0#frc#0#ctrl#0#mof#0");
                Send("@B", this.player.crys.GetCry);
                this.player.SendMoney();
                this.player.SendHp();
                this.player.SendLvl();
                this.player.TryToGetChunks();
                SendOnline();
                this.player.inventory.Choose(-1);

            }
            else if (p.eventType == "PO")
            {
                string[] resp = Encoding.UTF8.GetString(p.data).Split(':');
                prp = int.Parse(resp[0]);
                prp++;
                lst = int.Parse(resp[1]);
                lst++;
                AsyncAction(20, () => { Send("PI", prp + ":" + lst + ":20"); });

            }
            else if (p.eventType == "TY")
            {
                TYPacket ty = new TYPacket(p.data);
                if (ty.eventType == "Xmov")
                {
                    int.TryParse(Encoding.UTF8.GetString(ty.data).Trim(), out int dir);
                    this.player.Move(ty.x, ty.y, dir > 9 ? dir - 10 : dir);
                }
                if (ty.eventType == "GUI_" && player.win != "")
                {
                    try
                    {
                        HorbDecoder.Decode(Encoding.UTF8.GetString(ty.data), this.player);
                    }
                    catch (Exception ex)
                    {
                        Send("Gu", "");
                        player.win = "";
                    }
                }
                else if (ty.eventType == "Whoi")
                {
                    SendNick(this.player.id, this.player.name);
                }
                else if (ty.eventType == "Locl")
                {
                    var text = Encoding.UTF8.GetString(ty.data);
                    if (text.StartsWith("console"))
                    {
                        player.ShowConsole();
                        return;
                    }
                    else if (text.StartsWith(">"))
                    {
                        if (text.Length > 1)
                        {
                            HorbDecoder.Console(text.Substring(1, text.Length - 1), this.player);
                        }
                        player.ShowConsole();
                        return;

                    }
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        this.player.SendLocalMsg(ty.data);
                    }
                }
                else if (ty.eventType == "Chat")
                {
                    var text = Encoding.UTF8.GetString(ty.data);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        new GlobalChat.CHPacket(new string[] { GlobalChat.CHPacket.GetBody(this.player, Encoding.UTF8.GetString(ty.data)) }, "FED");
                    }
                }
                else if (ty.eventType == "DPBX")
                {
                    HorbConst c = new HorbConst();
                    this.player.crys.BuildBox(c);
                    c.Send("box", this.player);
                }
                else if (ty.eventType == "TADG")
                {
                    if (player != null)
                        Send("BD", (player.autoDig = !player.autoDig) ? "1" : "0");
                }
                else if (ty.eventType == "Xdig")
                {
                    string tmp = Encoding.UTF8.GetString(ty.data).Trim();
                    int.TryParse(tmp, out int dir);
                    player.Move((uint)ty.x, (uint)ty.y, dir);
                    uint x = (uint)this.player.GetDirCord().X;
                    uint y = (uint)this.player.GetDirCord().Y;
                    if (World.THIS.ValidCoord(x, y))
                    {
                        this.player.Bz(x, y);
                    }
                }
                else if (ty.eventType == "Xgeo")
                {
                    uint x = (uint)this.player.GetDirCord().X;
                    uint y = (uint)this.player.GetDirCord().Y;
                    if (World.THIS.ValidCoord(x, y))
                    {
                        this.player.UseGeo(x, y);
                    }
                }
                else if (ty.eventType == "Xhea")
                {
                    this.player.Heal();
                }
                else if (ty.eventType == "Xbld")
                {
                    string tmp = Encoding.UTF8.GetString(ty.data).Trim();
                    uint x = (uint)this.player.GetDirCord().X;
                    uint y = (uint)this.player.GetDirCord().Y;
                    if (World.THIS.ValidCoord(x, y))
                    {
                        this.player.Build(x, y, tmp.Substring(1, 1));
                    }
                }
                else if (ty.eventType == "RESP")
                {
                    if (player.win != "")
                    {
                        Send("Gu", "");
                        player.win = "";
                    }
                    this.player.Death();
                }
                else if (ty.eventType == "INCL")
                {

                    string tmp = Encoding.UTF8.GetString(ty.data).Trim();
                    int.TryParse(tmp, out int type);
                    if (type == -1)
                    {

                        player.inventory.Choose(-1);
                        Send("IN", "close:0:0:");
                    }
                    else
                    {
                        player.inventory.Choose(type);

                    }
                }
                else if (ty.eventType == "INUS")
                {
                    uint x = (uint)(this.player.pos.X + (this.player.dir == 3 ? 1 : this.player.dir == 1 ? -1 : 0));
                    uint y = (uint)(this.player.pos.Y + (this.player.dir == 0 ? 1 : this.player.dir == 2 ? -1 : 0));
                    player.inventory.Use(x, y);
                }

            }
        }
        public void Send(string eventType, byte[] data)
        {
            Send(new Packet("B", eventType, data));
        }
        public void Send(string eventType, string data)
        {
            Send(new Packet("U", eventType, data));
        }
        public void Send(Packet p)
        {
            //Console.WriteLine("[S->C] " + p.dataType + " " + p.eventType + " [" + string.Join(",", p.data) + "] " + Encoding.UTF8.GetString(p.data));
            SendAsync(p.Compile);
        }
        public void SendCells(int w, int h, uint x, uint y, byte[] cells)
        {
            byte[] data = new byte[7 + cells.Length];
            data[0] = (byte)'M';
            data[1] = (byte)w;
            data[2] = (byte)h;
            byte[] _x = BitConverter.GetBytes(x);
            System.Buffer.BlockCopy(_x, 0, data, 3, 2);
            byte[] _y = BitConverter.GetBytes(y);
            System.Buffer.BlockCopy(_y, 0, data, 5, 2);
            System.Buffer.BlockCopy(cells, 0, data, 7, cells.Length);
            Send("HB", data);
        }
        public void SendBot(int bid, uint x, uint y, int dir, int cid, int skin, int tail)
        {
            byte[] data = new byte[13];
            data[0] = (byte)'X';
            data[1] = (byte)dir;
            data[2] = (byte)0;
            data[3] = (byte)0;
            System.Buffer.BlockCopy(BitConverter.GetBytes(bid), 0, data, 4, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(x), 0, data, 6, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(y), 0, data, 8, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(cid), 0, data, 10, 2);
            Send("HB", data);
        }
        public void AddDFX(int fx, int dir, uint x, uint y, int bid, int col = 0)
        {
            byte[] data = new byte[10];
            data[0] = (byte)'D';
            System.Buffer.BlockCopy(BitConverter.GetBytes(fx), 0, data, 1, 1);
            System.Buffer.BlockCopy(BitConverter.GetBytes(dir), 0, data, 2, 1);
            System.Buffer.BlockCopy(BitConverter.GetBytes(col), 0, data, 3, 1);
            System.Buffer.BlockCopy(BitConverter.GetBytes(x), 0, data, 4, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(y), 0, data, 6, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(bid), 0, data, 8, 2);

            Send("HB", data);
        }
        public void ClearPack(uint x, uint y)
        {
            byte[] data = new byte[15];
            data[0] = (byte)'O';
            uint index = x + y * (uint)World.height;
            System.Buffer.BlockCopy(BitConverter.GetBytes(index), 0, data, 1, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes((uint)0), 0, data, 5, 2);
            Send("HB", data);
        }
        public void AddPack(char type, uint x, uint y, int cid, int off)
        {
            byte[] data = new byte[15];
            data[0] = (byte)'O';
            uint index = x + y * (uint)World.height;
            System.Buffer.BlockCopy(BitConverter.GetBytes(index), 0, data, 1, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes((uint)1), 0, data, 5, 2);
            data[7] = (byte)type;
            System.Buffer.BlockCopy(BitConverter.GetBytes(x), 0, data, 8, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(y), 0, data, 10, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(cid), 0, data, 12, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(off), 0, data, 14, 1);
            Send("HB", data);
        }
        public void AddPack(Pack p)
        {
            byte[] data = new byte[15];
            data[0] = (byte)'O';
            uint index = p.x + p.y * (uint)World.height;
            System.Buffer.BlockCopy(BitConverter.GetBytes(index), 0, data, 1, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes((uint)1), 0, data, 5, 2);
            data[7] = (byte)p.type;
            System.Buffer.BlockCopy(BitConverter.GetBytes(p.x), 0, data, 8, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(p.y), 0, data, 10, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(p.cid), 0, data, 12, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(p.off), 0, data, 14, 1);
            Send("HB", data);
        }
        public void AddFX(int fx, uint x, uint y)
        {
            byte[] data = new byte[6];
            data[0] = (byte)'F';
            System.Buffer.BlockCopy(BitConverter.GetBytes(fx), 0, data, 1, 1);
            System.Buffer.BlockCopy(BitConverter.GetBytes(x), 0, data, 2, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(y), 0, data, 4, 2);
            Send("HB", data);
        }
        public void SendNick(int id, string nick)
        {
            Send("NL", id + ":" + nick);
        }
        public void SendCell(uint x, uint y, byte cell)
        {
            if ((x >= 0 && y >= 0) && (x <= XServer.width && y <= XServer.height))
            {
                byte[] dat = new byte[1];
                dat[0] = cell;
                SendCells(1, 1, x, y, dat);
            }
        }
        public void AddGlobalChatMsg()
        {

        }
        public void SendLocalChat(int datal, int bid, uint x, uint y, byte[] msg)
        {
            byte[] mess = new byte[9 + datal];
            mess[0] = (byte)'C';
            System.Buffer.BlockCopy(BitConverter.GetBytes(bid), 0, mess, 1, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(x), 0, mess, 3, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(y), 0, mess, 5, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(datal), 0, mess, 7, 2);
            System.Buffer.BlockCopy(msg, 0, mess, 9, datal);
            Send("HB", mess);
        }
    }
}