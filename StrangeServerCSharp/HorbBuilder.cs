using System.Dynamic;
using Newtonsoft.Json;

namespace StrangeServerCSharp
{
    public class HorbBuilder
    {
        private dynamic _horb;

        public HorbBuilder()
        {
            this._horb = new ExpandoObject();
        }

        public string Build()
        {
            return "horb:" + JsonConvert.SerializeObject(_horb, Formatting.None);
        }

        public HorbBuilder AddTitle(string title)
        {
            _horb.title = title;
            return this;
        }

        public HorbBuilder SetInv(string inv)
        {
            _horb.inv = inv;
            return this;
        }

        public HorbBuilder AddTextLine(string textLine)
        {
            if (!FieldExists("text"))
            {
                _horb.text = string.Empty;
            }

            _horb.text += textLine + "\n";
            return this;
        }

        public HorbBuilder AddTextLines(params string[] textLines)
        {
            foreach (var textLine in textLines)
            {
                AddTextLine(textLine);
            }

            return this;
        }

        public HorbBuilder AddMarketCard(string miscid, string title)
        {
            _horb.card = $"i{miscid}:{title}";
            return this;
        }

        public HorbBuilder AddCard(string id, string miscid, string title)
        {
            _horb.card = $"{id}{miscid}:{title}";
            return this;
        }

        public HorbBuilder AddClanList()
        {
            _horb.clanlist = new string[] { };
            return this;
        }

        public HorbBuilder AddClanListLine(string id, string text, string nexttext, string action)
        {
            _horb.clanlist = _horb.clanlist.Concat(new string[] { id, text, nexttext, action }).ToArray();
            return this;
        }


        public HorbBuilder SetText(string text)
        {
            _horb.text = text;
            return this;
        }

        public HorbBuilder AddListLine(string text, string nexttext, string action)
        {
            if (!FieldExists("list"))
            {
                _horb.list = new List<string>();
                var list = new List<string>();
            }

            _horb.list.AddRange(new string[] { text, nexttext, action });
            return this;
        }

        public HorbBuilder AddIConsole()
        {
            _horb.input_console = true;
            return this;
        }

        public HorbBuilder AddIConsolePlace(string text)
        {
            _horb.input_place = text;
            return this;
        }

        public HorbBuilder AddCrysRight(string text)
        {
            _horb.crys_right = text;
            return this;
        }

        public HorbBuilder AddCrysLeft(string text)
        {
            _horb.crys_left = text;
            return this;
        }

        public HorbBuilder AddCrysBuy()
        {
            _horb.crys_buy = true;
            return this;
        }

        public HorbBuilder Send(string type, Player to)
        {
            to.connection.Send("GU", Build());
            to.win = type;
            return this;
        }

        public HorbBuilder SendToSess(Session to)
        {
            to.Send("GU", Build());
            return this;
        }

        public HorbBuilder AddCrysLine(CrysLine line)
        {
            if (!FieldExists("crys_lines"))
            {
                _horb.crys_lines = new List<string>();
            }

            _horb.crys_lines.Add(line.ToString());
            return this;
        }

        public HorbBuilder AddCrysLines(params CrysLine[] lines)
        {
            foreach (var line in lines)
            {
                AddCrysLine(line);
            }

            return this;
        }

        public HorbBuilder AddCss(float ch = 0, float w = 0, float h = 0, string invb = "misc")
        {
            var c = new css();
            c.ch = ch;
            c.w = w;
            c.h = h;
            c.InvButton = invb;
            _horb.css = c.ToString();
            return this;
        }

        public HorbBuilder AddButton(string text, string command)
        {
            if (!FieldExists("buttons"))
            {
                _horb.buttons = new List<string>();
            }

            _horb.buttons.Add(text);
            _horb.buttons.Add(command);
            return this;
        }

        public HorbBuilder AddTab(string text, string command)
        {
            if (!FieldExists("tabs"))
            {
                _horb.tabs = new List<string>();
            }

            _horb.tabs.Add(text);
            _horb.tabs.Add(command);
            return this;
        }

        public HorbBuilder Admin()
        {
            _horb.admin = true;
            return this;
        }

        public HorbBuilder SetRichList(params string[] richList)
        {
            _horb.richList = richList;
            return this;
        }

        private bool FieldExists(string field)
        {
            return (_horb as IDictionary<string, object>).ContainsKey(field);
        }
    }

    public struct CrysLine
    {
        public override string ToString()
        {
            return leftMin.ToString() + ":" + rightMin.ToString() + ":" + d.ToString() + ":" + value.ToString() + ":" +
                   descText;
        }

        public long leftMin;
        public long rightMin;
        public long d;
        public long value;
        public string descText;
    }

    public struct RichListEntry
    {
        public string[] ToList()
        {
            return new string[]
            {
                this.Text,
                this.Type,
                this.Values,
                this.Action,
                this.InitialValue
            };
        }

        public override string ToString()
        {
            return "\"" + string.Join("\",\"", this.ToList()) + "\"";
        }

        public string Text;

        public string Type;

        public string Values;

        public string InitialValue;

        public string Action;
    }

    public class InvGen
    {
        public static string getmarket(string[] mp)
        {
            var s = "";
            foreach (var i in mp)
            {
                s += i;
            }

            return s;
        }
    }

    //Darkar25 richlistgen
    public static class RichListGenerator
    {
        public static RichListEntry Text(string text)
        {
            return new RichListEntry
            {
                Text = text,
                Type = "text",
                Values = "",
                Action = "",
                InitialValue = ""
            };
        }


        public static RichListEntry Bool(string text, string id, bool value)
        {
            return new RichListEntry
            {
                Text = text,
                Type = "bool",
                Values = "0",
                Action = id,
                InitialValue = (value ? "1" : "0")
            };
        }


        public static RichListEntry UInt(string text, string id, uint value)
        {
            return new RichListEntry
            {
                Text = text,
                Type = "uint",
                Values = "0",
                Action = id,
                InitialValue = value.ToString()
            };
        }


        public static RichListEntry Drop(string text, string id, string[] values, int index)
        {
            var text2 = "";
            for (var i = 0; i < values.Length; i++)
            {
                text2 = string.Concat(new object[]
                {
                    text2,
                    i,
                    ":",
                    values[i].ToString(),
                    "#"
                });
            }

            return new RichListEntry
            {
                Text = text,
                Type = "drop",
                Values = text2,
                Action = id,
                InitialValue = index.ToString()
            };
        }


        public static RichListEntry Fill(string text, string barText, int percent, int crystalType, string action100,
            string action1000, string actionMax)
        {
            return new RichListEntry
            {
                Text = text,
                Type = "fill",
                Values = string.Concat(new object[]
                {
                    (percent < 0) ? 0 : ((percent > 100) ? 100 : percent),
                    "#",
                    barText,
                    "#",
                    (crystalType < 0) ? 0 : ((crystalType > 5) ? 5 : crystalType),
                    "#",
                    action100,
                    "#",
                    action1000,
                    "#",
                    actionMax
                }),
                Action = "",
                InitialValue = ""
            };
        }


        public static RichListEntry Fill(string text, int current, int max, int crystalType, string action100,
            string action1000, string actionMax)
        {
            var num = (current < 0) ? 0 : ((current > max) ? max : current);
            var per = num > 0 ? Math.Round((decimal)num / (max / 100)) : 0;
            return new RichListEntry
            {
                Text = text,
                Type = "fill",
                Values = string.Concat(new object[]
                {
                    per.ToString(),
                    "#",
                    num,
                    "/",
                    max,
                    "#",
                    (crystalType < 0) ? 0 : ((crystalType > 5) ? 5 : crystalType),
                    "#",
                    action100,
                    "#",
                    action1000,
                    "#",
                    actionMax
                }),
                Action = "",
                InitialValue = ""
            };
        }

        public static RichListEntry Button(string text, string action, string buttonText)
        {
            return new RichListEntry
            {
                Text = text,
                Type = "button",
                Values = buttonText,
                Action = action,
                InitialValue = ""
            };
        }
    }

    public class CardBuilder
    {
        public CardBuilder()
        {
            this.Reset();
        }


        public void SetTitle(string title)
        {
            this.Title = title;
        }


        public void SetUnknown(string unk)
        {
            this.IDK = unk;
        }


        public void SetAction(string act)
        {
            this.Action = act;
        }

        public void SetURI(string uri)
        {
            this.URI = uri;
        }


        public void FlushLine()
        {
            var t = this.Title.Contains("&") ? this.Title.Substring(0, this.Title.IndexOf('&')) : this.Title;
            var unk = this.IDK.Contains("&") ? this.IDK.Substring(0, this.IDK.IndexOf('&')) : this.IDK;
            var act = this.Action.Contains("&") ? this.Action.Substring(0, this.Action.IndexOf('&')) : this.Action;
            var uri = this.URI.Contains("&") ? this.URI.Substring(0, this.URI.IndexOf('&')) : this.URI;
            this.UnsafeFlushLine(t, unk, act, uri, this.Width, this.Height);
        }

        public void Reset()
        {
            this.Title = "";
            this.IDK = "";
            this.Action = "";
            this.URI = "";
            this.Width = 8U;
            this.Height = 8U;
        }


        public RichListEntry Result
        {
            get
            {
                var text = "";
                for (var i = 0; i < this.image.Count; i++)
                {
                    if (text != "")
                    {
                        text += "&";
                    }

                    text = string.Concat(new object[]
                    {
                        text,
                        this.image[i],
                        "%",
                        this.width[i],
                        "%",
                        this.height[i]
                    });
                }

                return new RichListEntry
                {
                    Text = string.Join("&", this.title),
                    Type = "3card",
                    Values = string.Join("&", this.somethin),
                    Action = string.Join("&", this.action),
                    InitialValue = text
                };
            }
        }


        public new string ToString()
        {
            return this.Result.ToString();
        }


        public string Title { get; set; }


        public string IDK { get; set; }


        public string Action { get; set; }


        public string URI { get; set; }

        public void SetWidth(uint width)
        {
            this.Width = width;
        }


        public void SetHeight(uint height)
        {
            this.Height = height;
        }


        public void SetImageURL(string url, uint width, uint height)
        {
            this.SetURI(url);
            this.SetWidth(width);
            this.SetHeight(height);
        }


        public void UnsafeFlushLine(string t, string unk, string act, string uri, uint w, uint h)
        {
            this.title.Add(t);
            this.somethin.Add(unk);
            this.action.Add(act);
            this.image.Add(uri);
            this.width.Add(w);
            this.height.Add(h);
            this.Reset();
        }


        public uint Width { get; set; }


        public uint Height { get; set; }


        public void SetIngameImage(string spriteGroup, uint index, uint scale)
        {
            this.SetURI(string.Concat(new object[]
            {
                "inner:",
                spriteGroup.ToUpper(),
                ":",
                index
            }));
            this.SetWidth(scale);
            this.SetHeight(0U);
        }

        private List<string> title = new List<string>();
        private List<string> somethin = new List<string>();


        private List<string> action = new List<string>();


        private List<string> image = new List<string>();


        private List<uint> width = new List<uint>();


        private List<uint> height = new List<uint>();
    }

    public struct css
    {
        public override string ToString()
        {
            return "invButton=" + InvButton + ";inv-w=" + w.ToString() + ";inv-ch=" + ch.ToString();
        }

        public string InvButton { get; set; }
        public float ch { get; set; }
        public float w { get; set; }
        public float h { get; set; }
        public bool fixScroll { get; set; }
        public string fixTextScroll { get; set; }
        public bool biginput { get; set; }
        public bool disableKeyboard { get; set; }
        public float space { get; set; }
        public float scrollH { get; set; }
    }
}