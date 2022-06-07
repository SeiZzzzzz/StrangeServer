using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StrangeServerCSharp
{
    public class Clan
    {

        public int id { get; set; }
        public ClanMemberList memberList { get; set; }
    }
    public class ClanMemberList : List<int>
    {
        public int id { get; set; }
    }
}
