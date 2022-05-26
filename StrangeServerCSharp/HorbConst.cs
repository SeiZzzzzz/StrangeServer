using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace StrangeServerCSharp
{
    public class HorbConst
    {
        public HorbConst()
        {
            this.rhorb = default(HORB);
            cryslines = new List<string>();
            buttons = new List<string>();
        }
        public void AddTitle(string title)
        {
            rhorb.title = title;
        }
        public void AddCss(string css)
        {
            rhorb.css = css;
        }
        public void AddTextLine(string text)
        {
            rhorb.text += text + "\n";
        }
        public void AddIConsole()
        {
            rhorb.input_console = true;
        }
        public void AddIConsolePlace(string text)
        {
            rhorb.input_place = text;
        }
        public void AddCrysRight(string text)
        {
            rhorb.crys_right = text;
        }
        public void AddCrysLeft(string text)
        {
            rhorb.crys_left = text;
        }
        public void AddCrysBuy()
        {
            rhorb.crys_buy = true;
        }
        public void Send(string type, Player to)
        {
            to.connection.Send("GU", this.Result);
            to.win = type;
        }
        public void AddCrysLine(CrysLine line)
        {
            cryslines.Add(line.ToString());
            rhorb.crys_lines = cryslines.ToArray();
        }
        public void AddButton(string text, string command)
        {
            buttons.Add(text);
            buttons.Add(command);
            rhorb.buttons = buttons.ToArray();
        }
        public void AddTab(string text, string command)
        {
            tabs.Add(text);
            tabs.Add(command);
            rhorb.buttons = tabs.ToArray();
        }
        public void Admin()
        {
            rhorb.admin = true;
        }
        public List<string> cryslines = new List<string>();
        public List<string> buttons = new List<string>();
        public List<string> tabs = new List<string>();
        public HORB rhorb;
        public string Result
        {
            get
            {
                var j = JToken.Parse(rhorb.ToString());
                RemoveNullNodes(j);
                return "horb:" + j.ToString(Formatting.None);
            }
        }
        private void RemoveNullNodes(JToken root)
        {
            if (root is JValue)
            {
                if (((JValue)root).Value == null)
                {
                    ((JValue)root).Parent.Remove();
                }
            }
            else if (root is JArray)
            {
                ((JArray)root).ToList().ForEach(n => RemoveNullNodes(n));
                if (!(((JArray)root)).HasValues)
                {
                    root.Parent.Remove();
                }
            }
            else if (root is JProperty)
            {
                RemoveNullNodes(((JProperty)root).Value);
            }
            else
            {
                var children = ((JObject)root).Properties().ToList();
                children.ForEach(n => RemoveNullNodes(n));

                if (!((JObject)root).HasValues)
                {
                    if (((JObject)root).Parent is JArray)
                    {
                        ((JArray)root.Parent).Where(x => !x.HasValues).ToList().ForEach(n => n.Remove());
                    }
                    else
                    {
                        var propertyParent = ((JObject)root).Parent;
                        while (!(propertyParent is JProperty))
                        {
                            propertyParent = propertyParent.Parent;
                        }
                        propertyParent.Remove();
                    }
                }
            }
        }

    }
    public struct CrysLine
    {
        public override string ToString()
        {
            return leftMin.ToString() + ":" + rightMin.ToString() + ":" + d.ToString() + ":" + value.ToString() + ":" + descText;
        }
        public long leftMin;
        public long rightMin;
        public long d;
        public long value;
        public string descText;
    }
    public struct HORB
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public string css { get; set; }

        public string crys_right { get; set; }

        public string crys_left { get; set; }

        public string[] crys_lines { get; set; }

        public string[] tabs { get; set; }

        public string title { get; set; }

        public string text { get; set; }

        public string card { get; set; }

        public string inv { get; set; }

        public string input_place { get; set; }

        public int input_len { get; set; }

        public string[] buttons { get; set; }

        public string[] clanlist { get; set; }

        public string[] list { get; set; }

        public string[] richList { get; set; }

        public string[] canvas { get; set; }

        public bool rich_no_scroll { get; set; }

        public bool back { get; set; }

        public bool crys_buy { get; set; }

        public bool admin { get; set; }

        public bool paint { get; set; }

        public bool input_console { get; set; }
    }
    public static class HorbDecoder
    {
        public static void Decode(string c, Player p)
        {
            JObject jo = null;
            try
            {
                jo = JObject.Parse(c);
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return;
            }
            var button = jo["b"];
            if (button != null)
            {
                if (p.win == "")
                {
                    return;
                }
                if (p.win == "box")
                {
                    box((string)button, p);
                }
                else if (p.win == "console")
                {
                    Console((string)button, p);
                }

                if (button != null && (string)button == "exit")
                {
                    Exit((string)button, p);
                }
            }
        }
        private static void Exit(string text, Player p)
        {
            if (text == "exit")
            {
                p.connection.Send("Gu", "");
                p.win = "";
            }
        }
        public static void Console(string text, Player p)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                p.AddConsoleLine(text);
                if (text.StartsWith("newnick"))
                {
                    if (!string.IsNullOrWhiteSpace(text.Split(" ")[1]))
                    {
                        p.SetNick(text.Split(" ")[1]);
                        p.AddConsoleLine("новый ник:" + p.name);
                    }
                }
                if (text.StartsWith("setcell"))
                {
                    if (!string.IsNullOrWhiteSpace(text.Split(" ")[1]))
                    {
                        p.cellg = byte.Parse(text.Split(" ")[1]);
                        p.AddConsoleLine("cell:" + p.cellg);
                    }
                }
            }
            p.ShowConsole();
        }
        private static void box(string text, Player p)
        {
            if (text.StartsWith("dropbox"))
            {
                string[] cry = text.Split(":");
                long[] box = new long[6];
                uint x = (uint)p.GetDirCord().X;
                uint y = (uint)p.GetDirCord().Y;
                if (!World.THIS.ValidCoord(x, y) || !(World.THIS.GetCellConst(x, y).is_empty && World.THIS.GetCellConst(x, y).can_build_over))
                {
                    return;
                }
                for (int i = 0; i < 6; i++)
                {
                    long remcry = long.Parse(cry[i + 1]);
                    if (p.crys.RemoveCrys(i, remcry))
                    {
                        box[i] = remcry;
                    }
                }
                if (box.Sum() <= 0)
                {
                    return;
                }
                Box.BuildBox(x, y, box);
                p.connection.Send("Gu", "");
                p.win = "";
            }
        }
    }
}
