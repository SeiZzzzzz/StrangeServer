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
            tabs = new List<string>();
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
        public void SetText(string text)
        {
            rhorb.text = text;
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
        public void SendToSess(Session to)
        {
            to.Send("GU", this.Result);
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
            rhorb.tabs = tabs.ToArray();
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
        public new string ToString()
        {
            return "\"" + string.Join("\",\"", this.ToList()) + "\"";
        }

        public string Text;

        public string Type;

        public string Values;

        public string InitialValue;

        public string Action;
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
            string text2 = "";
            for (int i = 0; i < values.Length; i++)
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


        public static RichListEntry Fill(string text, string barText, int percent, int crystalType, string action100, string action1000, string actionMax)
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


        public static RichListEntry Fill(string text, int current, int max, int crystalType, string action100, string action1000, string actionMax)
        {
            int num = (current < 0) ? 0 : ((current > max) ? max : current);
            return new RichListEntry
            {
                Text = text,
                Type = "fill",
                Values = string.Concat(new object[]
                {
                    Math.Round((double)((float)max / 100f * (float)num)),
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
                string t = this.Title.Contains("&") ? this.Title.Substring(0, this.Title.IndexOf('&')) : this.Title;
                string unk = this.IDK.Contains("&") ? this.IDK.Substring(0, this.IDK.IndexOf('&')) : this.IDK;
                string act = this.Action.Contains("&") ? this.Action.Substring(0, this.Action.IndexOf('&')) : this.Action;
                string uri = this.URI.Contains("&") ? this.URI.Substring(0, this.URI.IndexOf('&')) : this.URI;
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
                    string text = "";
                    for (int i = 0; i < this.image.Count; i++)
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
    
}
