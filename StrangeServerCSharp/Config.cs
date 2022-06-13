using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeServerCSharp
{
    public class Config
    {
        public static Config THIS;
        public string DBType { get; set; }

        public Config()
        {
            DBType = "localdb";
        }
    }
}
