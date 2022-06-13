using NetCoreServer;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace StrangeServerCSharp
{
    public class Program
    {
        static void Main(string[] a)
        {
            var configPath = "config.json";
            if (File.Exists(configPath))
            {
                Config.THIS = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
            }
            else
            {
                Config.THIS = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(Config.THIS, Formatting.Indented));
            }
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);
            var port = 8090;
            var server = new XServer(IPAddress.Any, port);
            server.Start();
            Task.Run(() =>
            {
                ProgInterpreter.THIS.Init();
            });
            for (;;)
            {
                var line = Console.ReadLine();
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
        public BDClass db = new BDClass();
        public IList<Session> PlayerSessions { get; set; }
        public XServer(IPAddress address, int port) : base(address, port)
        {
            PlayerSessions = new List<Session>();
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
                for (var i = 0; i < roadmap.Length; i++)
                {
                    roadmap[i] = 32;
                }
            }

            world = new World(width, height, map, roadmap);
        }

        public void SaveMap()
        {
            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    if (World.THIS.isPack(World.THIS.GetCell(x, y)))
                    {
                        World.THIS.SetCell(x, y, 32);
                    }
                }
            }

            File.WriteAllBytes("cum.map", World.map);
            File.WriteAllBytes("cumroad.map", World.roadmap);
            BDClass.Save();
        }

        protected override TcpSession CreateSession()
        {
            var session = new Session(this);
            PlayerSessions.Add(session);
            return session;
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Nigga Balls {error}");
        }
    }

    public class Session : TcpSession
    {
        public async void AsyncAction(int delay, Action act)
        {
            await Task.Run(delegate()
            {
                System.Threading.Thread.Sleep(delay);
                act();
            });
        }

        public int bid;
        public Player player;
        public int lst = 0;
        public int prp = 0;
        public static long online = 0;
        public string sid = "";

        public Session(TcpServer server) : base(server)
        {
        }

        protected override void OnConnected()
        {
            Console.WriteLine(this.Id.ToString());
            sid = this.Id.ToString();
            Send("AU", sid);
            online = 1;
            foreach (var player in XServer.players)
            {
                if (player.Value.connection.IsConnected)
                {
                    online++;
                }
            }

            foreach (var player in XServer.players)
            {
                player.Value.connection.SendOnline();
            }
        }

        public static string CalculateMD5Hash(string input)
        {
            HashAlgorithm hashAlgorithm = MD5.Create();
            var bytes = Encoding.ASCII.GetBytes(input);
            var array = hashAlgorithm.ComputeHash(bytes);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        protected override void OnDisconnected()
        {
            if (player == null)
            {
                return;
            }

            Console.WriteLine("Disconnected " + player.id);
            if (player.timer != null)
            {
                this.player.timer.Dispose();
            }

            Task.Run(() =>
            {
                this.player.ForceRemove();
                this.player.Death();
            });
            XServer.THIS.db.SaveChanges();
            XServer.players.Remove(this.player.id);
            online = 1;
            foreach (var player in XServer.players)
            {
                if (player.Value.connection.IsConnected)
                {
                    online++;
                }
            }

            foreach (var player in XServer.players)
            {
                player.Value.connection.SendOnline();
            }
        }

        public void SendOnline()
        {
            this.Send("ON", online + ":0");
        }

        public int winauthtype = 0;
        public bool autosed = false;
        public string auname = "";
        public string aupasswd = "";

        public void Auth(byte[] data, out bool ret)
        {
            ret = true;
            string x = null;
            string[] datax = null;
            if (data != null)
            {
                x = Encoding.UTF8.GetString(data);
                datax = x.Split('_');
                if (datax[1] == "0")
                {
                    Send("AH", "BAD");
                    data = null;
                }
                else
                {
                    try
                    {
                        if (!int.TryParse(datax[1], out var id))
                        {
                            Send("AH", "BAD");
                        }

                        var p = XServer.THIS.db.players.First(p => p.id == id);
                        if (CalculateMD5Hash(p.hash + this.sid) == datax[2])
                        {
                            this.player = p;
                            ret = false;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Send("AH", "BAD");
                    }
                }
            }

            if (data == null || x.Contains("NO_AUTH"))
            {
                f1:
                autosed = true;
                if (winauthtype == -1)
                {
                    Send("PI", "0:0:0");
                    Send("cf",
                        "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                        ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    Send("CF",
                        "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                        ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    new HorbBuilder()
                        .SetTitle("НОВЫЙ ИГРОК")
                        .AddTextLine("Ник.")
                        .AddIConsole()
                        .AddIConsolePlace("")
                        .AddButton("ВЫПОЛНИТЬ", "%I%")
                        .SendToSess(this);
                }
                else if (winauthtype == -2)
                {
                    Send("PI", "0:0:0");
                    Send("cf",
                        "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                        ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    Send("CF",
                        "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                        ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    new HorbBuilder()
                        .SetTitle("НОВЫЙ ИГРОК")
                        .AddTextLine("пароль")
                        .AddIConsole()
                        .AddIConsolePlace("я хуй")
                        .AddButton("ВЫПОЛНИТЬ", "%I%")
                        .SendToSess(this);
                }
                else if (winauthtype == 0)
                {
                    Send("PI", "0:0:0");
                    Send("cf",
                        "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                        ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    Send("CF",
                        "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                        ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    new HorbBuilder()
                        .SetTitle("АВТОРИЗАЦИЯ")
                        .AddTextLine("ник нужен ебать")
                        .AddIConsole()
                        .AddIConsolePlace("")
                        .AddButton("ВЫПОЛНИТЬ", "%I%")
                        .AddButton("НОВЫЙ АКК", "newakk")
                        .SendToSess(this);
                }
                else if (winauthtype == 1)
                {
                    Send("cf",
                        "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                        ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    Send("CF",
                        "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                        ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
                    new HorbBuilder()
                        .SetTitle("АВТОРИЗАЦИЯ")
                        .AddTextLine("пароль нужен ебать")
                        .AddIConsole()
                        .AddIConsolePlace("пароль блядота")
                        .AddButton("ВЫПОЛНИТЬ", "%I%")
                        .SendToSess(this);
                }
                else if (winauthtype > 1)
                {
                    try
                    {
                        var p = XServer.THIS.db.players.First(p => p.name == auname);
                        if (p.passwd == aupasswd)
                        {
                            autosed = false;
                            Send("AH", p.id + "_" + p.hash);
                            this.player = p;
                            ret = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        winauthtype = 0;
                        goto f1;
                    }
                }
                else if (winauthtype < -2)
                {
                    if (!string.IsNullOrWhiteSpace(auname) && !string.IsNullOrWhiteSpace(aupasswd) &&
                        !BDClass.NickAvl(auname))
                    {
                        using var db = new BDClass();
                        this.player = db.CreatePlayer(auname, aupasswd);
                        autosed = false;
                        Send("AH", this.player.id + "_" + this.player.hash);
                        ret = false;
                    }
                    else
                    {
                        winauthtype = 0;
                        goto f1;
                    }
                }
            }
        }

        public void InitPlayer()
        {
            using var db = new BDClass();
            this.player.connection = this;
            Console.WriteLine("connected " + player.id);
            if (!XServer.players.ContainsKey(this.player.id))
            {
                XServer.players.Add(player.id, player);
            }

            Send("PI", "0:0:0");
            Send("cf",
                "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
            Send("CF",
                "{\"width\":" + XServer.width + ",\"height\":" + XServer.height +
                ",\"name\":\"ladno\",\"v\":3410,\"version\":\"COCK\",\"update_url\":\"http://pi.door/\",\"update_desc\":\"ok\"}");
            Send("sp", "125:57:200");
            Send("BA", "0");
            Send("BD", "0");
            Send("GE", " ");
            Send("@T", $"{this.player.pos.X}:{this.player.pos.Y}");
            Send("BI",
                "{\"x\":" + this.player.pos.X + ",\"y\":" + this.player.pos.Y + ",\"id\":" + player.id +
                ",\"name\":\"" + player.name + "\"}");
            Send("sp", "25:20:100000");
            Send("@B", this.player.crys.GetCry);
            this.player.SendClan();
            this.player.SendMoney();
            this.player.SendHp();
            this.player.SendLvl();
            this.player.TryToGetChunks();
            this.player.settings = db.settings.First(i => i.id == player.id);
            if (this.player.settings != null)
            {
                this.player.settings.SendSett(this.player);
            }

            this.player.GimmeBotsUPD();
            SendOnline();
            this.player.inventory.Choose(-1);
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            var p = new Packet(buffer);
            if (p.eventType == "AU")
            {
                var ret = true;
                try
                {
                    Auth(p.data, out ret);
                }
                catch (Exception)
                {
                    this.Disconnect();
                    ret = true;
                }

                if (ret)
                {
                    return;
                }

                InitPlayer();
            }
            else if (p.eventType == "PO")
            {
                var resp = Encoding.UTF8.GetString(p.data).Split(':');
                prp = int.Parse(resp[0]);
                prp++;
                lst = int.Parse(resp[1]);
                lst++;
                AsyncAction(20, () => { Send("PI", prp + ":" + lst + ":20"); });
            }
            else if (p.eventType == "TY")
            {
                var ty = new TYPacket(p.data);
                if (autosed)
                {
                    Newtonsoft.Json.Linq.JObject jo = null;
                    try
                    {
                        jo = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(ty.data));
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        return;
                    }

                    var button = jo["b"];
                    if (button != null)
                    {
                        if (button.ToString() == "newakk")
                        {
                            winauthtype--;
                        }
                        else if (winauthtype == -1)
                        {
                            if (!string.IsNullOrWhiteSpace(button.ToString()))
                            {
                                auname = button.ToString();
                                winauthtype--;
                            }
                        }
                        else if (winauthtype == -2)
                        {
                            if (!string.IsNullOrWhiteSpace(button.ToString()))
                            {
                                aupasswd = button.ToString();
                                winauthtype--;
                            }
                        }
                        else if (winauthtype == 0)
                        {
                            winauthtype++;
                            auname = button.ToString();
                        }
                        else if (winauthtype == 1)
                        {
                            winauthtype++;
                            aupasswd = button.ToString();
                        }
                        else
                        {
                            winauthtype--;
                        }

                        Auth(null, out var ret);
                        return;
                    }

                    return;
                }

                if (ty.eventType == "Xmov")
                {
                    int.TryParse(Encoding.UTF8.GetString(ty.data).Trim(), out var dir);
                    this.player.Move(ty.x, ty.y, dir > 9 ? dir - 10 : dir);
                }

                if (ty.eventType == "Sett")
                {
                    this.player.settings.Open(this.player.settings.winid, this.player);
                }

                if (ty.eventType == "Clan")
                {
                    Clan.Open(Clan.winid, this.player);
                }

                if (ty.eventType == "ADMN")
                {
                    if (this.player.cpack != null && (this.player.id == this.player.cpack.ownerid))
                    {
                        this.player.win = this.player.cpack.winid.Split('.')[0] + ".ADMN";
                        this.player.cpack.Open(this.player, this.player.win);
                    }
                }
                else if (ty.eventType == "GUI_" && player.win != "")
                {
                    try
                    {
                        HorbDecoder.Decode(Encoding.UTF8.GetString(ty.data), this.player);
                    }
                    catch (Exception ex)
                    {
                        Send("Gu", "");
                        player.win = "";
                        Console.WriteLine(ex.Message);
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
                            HorbDecoder.Console(text.Substring(1), this.player);
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
                        new GlobalChat.CHPacket(
                            new string[] { GlobalChat.CHPacket.GetBody(this.player, Encoding.UTF8.GetString(ty.data)) },
                            "FED");
                    }
                }
                else if (ty.eventType == "DPBX")
                {
                    player.crys.BuildBox().Send("box", this.player);
                }
                else if (ty.eventType == "TADG")
                {
                    if (player != null)
                        Send("BD", (player.autoDig = !player.autoDig) ? "1" : "0");
                }
                else if (ty.eventType == "Xdig")
                {
                    var tmp = Encoding.UTF8.GetString(ty.data).Trim();
                    int.TryParse(tmp, out var dir);
                    player.Move((uint)ty.x, (uint)ty.y, dir);
                    var x = (uint)this.player.GetDirCord().X;
                    var y = (uint)this.player.GetDirCord().Y;
                    if (World.THIS.ValidCoord(x, y))
                    {
                        this.player.Bz(x, y);
                    }
                }
                else if (ty.eventType == "Xgeo")
                {
                    var x = (uint)this.player.GetDirCord().X;
                    var y = (uint)this.player.GetDirCord().Y;
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
                    var tmp = Encoding.UTF8.GetString(ty.data).Trim();
                    var x = (uint)this.player.GetDirCord().X;
                    var y = (uint)this.player.GetDirCord().Y;
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
                    var tmp = Encoding.UTF8.GetString(ty.data).Trim();
                    int.TryParse(tmp, out var type);
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
                    var x = (uint)(this.player.pos.X + (this.player.dir == 3 ? 1 : this.player.dir == 1 ? -1 : 0));
                    var y = (uint)(this.player.pos.Y + (this.player.dir == 0 ? 1 : this.player.dir == 2 ? -1 : 0));
                    player.inventory.Use(x, y);
                }
                else if (ty.eventType == "Pope")
                {
                    HorbDecoder.Prog("prog", player);
                }
                else if (ty.eventType == "pRST")
                {
                    Send("@P", "");
                    if (player.ProgData != null && !player.ProgData.IsActive)
                    {
                        player.ProgData.IsActive = false;
                        player.tail = 1;
                        player.win = "";
                        player.cpack = null;
                    }
                    else if (player.ProgData != null)
                    {
                        player.tail = 0;
                        player.ProgData.IsActive = false;
                        player.win = "";
                        player.cpack = null;
                    }
                    
                }
                else if (ty.eventType == "PROG")
                {
                    var length = BitConverter.ToInt32(ty.data.Take(sizeof(int)).ToArray());
                    if (length != 0)
                    {
                        using var db = new BDClass();
                        var id = BitConverter.ToInt32(ty.data.Skip(sizeof(int)).Take(sizeof(int)).ToArray());
                        var source = Encoding.UTF8.GetString(ty.data.Skip(sizeof(int) * 2 + length).ToArray());
                        var progs = db.players.Include(x => x.Progs).FirstOrDefault(x => x.id == player.id)?.Progs;
                        var prog = progs.FirstOrDefault(x => x.id == id);
                        prog.source = source;
                        db.SaveChanges();
                        player.tail = 1;
                        Send("@P", "1");
                        player.ProgData = ProgData.FromString(source);
                    }
                }
                else if (ty.eventType == "PDEL")
                {
                    using var db = new BDClass();
                    
                    var id = int.Parse(Encoding.UTF8.GetString(ty.data).Trim());
                    var prog = db.progs.Include(x => x.player).FirstOrDefault(x => x.id == id);
                    if (prog.player.id == player.id)
                    {
                        db.progs.Remove(prog);
                        db.SaveChanges();
                    }
                    HorbDecoder.Prog("prog", player);
                }
                else if (ty.eventType == "PCOP")
                {
                    var id = int.Parse(Encoding.UTF8.GetString(ty.data).Trim());
                    HorbDecoder.Prog("copy:" + id, player);
                } 
                else if (ty.eventType == "PREN")
                {
                    var id = int.Parse(Encoding.UTF8.GetString(ty.data).Trim());
                    HorbDecoder.Prog("rename", player);
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
            var data = new byte[7 + cells.Length];
            data[0] = (byte)'M';
            data[1] = (byte)w;
            data[2] = (byte)h;
            var _x = BitConverter.GetBytes(x);
            System.Buffer.BlockCopy(_x, 0, data, 3, 2);
            var _y = BitConverter.GetBytes(y);
            System.Buffer.BlockCopy(_y, 0, data, 5, 2);
            System.Buffer.BlockCopy(cells, 0, data, 7, cells.Length);
            Send("HB", data);
        }

        public void SendBot(int bid, uint x, uint y, int dir, int cid, int skin, int tail)
        {
            var data = new byte[13];
            data[0] = (byte)'X';
            data[1] = (byte)dir;
            data[2] = (byte)skin;
            data[3] = (byte)tail;
            System.Buffer.BlockCopy(BitConverter.GetBytes(bid), 0, data, 4, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(x), 0, data, 6, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(y), 0, data, 8, 2);
            System.Buffer.BlockCopy(BitConverter.GetBytes(cid), 0, data, 10, 2);
            Send("HB", data);
        }

        public void AddDFX(int fx, int dir, uint x, uint y, int bid, int col = 0)
        {
            var data = new byte[10];
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
            var data = new byte[15];
            data[0] = (byte)'O';
            var index = x + y * (uint)World.height;
            System.Buffer.BlockCopy(BitConverter.GetBytes(index), 0, data, 1, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes((uint)0), 0, data, 5, 2);
            Send("HB", data);
        }

        public void AddPack(char type, uint x, uint y, int cid, int off)
        {
            var data = new byte[15];
            data[0] = (byte)'O';
            var index = x + y * (uint)World.height;
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
            var data = new byte[15];
            data[0] = (byte)'O';
            var index = p.x + p.y * (uint)World.height;
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
            var data = new byte[6];
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
                var dat = new byte[1];
                dat[0] = cell;
                SendCells(1, 1, x, y, dat);
            }
        }

        public void AddGlobalChatMsg()
        {
        }

        public void SendLocalChat(int datal, int bid, uint x, uint y, byte[] msg)
        {
            var mess = new byte[9 + datal];
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