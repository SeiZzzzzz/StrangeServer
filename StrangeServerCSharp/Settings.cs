using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeServerCSharp
{
    public class Settings
    {
        public int Id { get; set; }
        public void Open()
        {
            var c = new HorbConst();
            c.AddTitle("НАСТРОЙКИ");
            c.AddButton("СОХРАНИТЬ", "change_settings:%R%");
            c.AddButton("СОЗДАТЬ КЛАН", "create_сlan");
            c.AddButton("ВЫЙТИ", "exit");
        }
    }
}
