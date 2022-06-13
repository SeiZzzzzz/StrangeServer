using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StrangeServerCSharp
{
    public class Prog
    {
        public int id { get; set; }
        public string title { get; set; }
        public string source { get; set; }
        [JsonIgnore]
        public Player player { get; set; }

    }
}
