using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StrangeServerCSharp
{
    public class Settings
    {
        public int id { get; set; }
        public string winid = "!!settings";
        public int isca{ get; set; }
        public int tsca { get; set; }
        public int mous { get; set; }
        public int pot { get; set; }
        public int frc { get; set; }
        public int ctrl { get; set; }
        public int mof { get; set; }
        public Settings()
        {
            isca = 0;
        }
    public void Open(Player p)
        {
            var c = new HorbConst();
            c.AddTitle("НАСТРОЙКИ");
            c.AddButton("СОХРАНИТЬ", "change_settings:%R%");
            if (p.clanid == 0)
            {
                c.AddButton("СОЗДАТЬ КЛАН", "create_сlan");
            }
            c.AddButton("ВЫЙТИ", "exit");
            c.AddTab("ГОРЯЧИЕ КЛАВИШИ", "hots");
            c.AddTab("КОНФИГ", "config");
            string[] l = new string[0];
            l = l.Concat(RichListGenerator.Drop("Масштаб интерфейса", "isca", new string[] { "МЕЛКО", "КРУПНО" }, isca).ToList()).ToArray();
            l = l.Concat(RichListGenerator.Drop("Масштаб территории", "tsca", new string[] { "МЕЛКО", "КРУПНО" }, tsca).ToList()).ToArray();
            l = l.Concat(RichListGenerator.Bool("Включить управление мышкой", "mous", mous == 0 ? false : true).ToList()).ToArray();
            l = l.Concat(RichListGenerator.Bool("Упрощенный режим графики", "pot", pot == 0 ? false : true).ToList()).ToArray();
            l = l.Concat(RichListGenerator.Bool("Принудительно обновлять породы (увеличит потр. CPU)", "frc", pot == 0 ? false : true).ToList()).ToArray();
            l = l.Concat(RichListGenerator.Bool("CTRL переключает скорость робота (вместо удерживания)", "ctrl", ctrl == 0 ? false : true).ToList()).ToArray();
            l = l.Concat(RichListGenerator.Bool("Отключить ближайшие звуки", "mof", mof == 0 ? false : true).ToList()).ToArray();
            c.rhorb.richList = l;
            p.win = winid;
            c.Send(p.win, p);
        }
    }
}
