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
        }
        public void SendSett(Player p)
        {
            p.connection.Send("#S", "#cc#10#snd#0#mus#0#isca#" + p.settings.isca + "#tsca#" + p.settings.isca + "#mous#" + p.settings.mous + "#pot#" + p.settings.pot + "#frc#" + p.settings.frc + "#ctrl#" + p.settings.ctrl + "#mof#" + p.settings.mof);
        }
    public void Open(string tab,Player p)
        {
            var c = new HorbConst();
            if (tab == winid)
            {
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
            }
            else if (tab == "!!settings.create_сlan")
            {
                c.AddTitle("СОЗДАНИЕ НОВОГО КЛАНА");
                c.SetText("@@\nУра! Вы собираетесь создать новый клан. После создания клана вы сможете\nвыполнять клановые квесты, создавать свои фермы, вести войны с другими\nкланами, защищать и отбивать территории, и многое другое.\n\nСоздание клана - ответственное действие, значок и название клана нельзя\nбудет изменить позже. Поэтому внимательно подумайте над тем, как будет\nзвучать и выглядеть ваш клан в игре.\n\nСоздание клана требует залога в 1000 кредитов.\n");
                c.AddButton("ВЫБРАТЬ ЗНАЧОК КЛАНА", "@create_сlan2");
                c.AddButton("ВЫХОД", "exit");
            }
            else if (tab == "!!settings.create_сlan2")
            {
                c.AddTitle("ВЫБОР ЗНАЧКА КЛАНА");
                c.SetText("@@Выберите значок клана. Всего значков больше сотни. Для удобства мы\nпоказываем их небольшими порциями. Нажмите ДРУГИЕ, чтобы посмотреть еще.\nДля выбора значка - кликните на него.\n\nВнимание! Значок клана нельзя будет изменить после создания.\n");
                var icons = Clan.GetAvlClanIcon();
                var invstring = "";
                foreach (var ic in icons)
                {
                    invstring += 200 + ic.id + ":0:";
                }
                c.SetInv(invstring.Substring(0, invstring.Length - 1));
                c.AddButton("ДРУГИЕ", "@create_сlan2");
                c.AddButton("НАЗАД", "<create_сlan");
            }
            else if (tab.StartsWith("!!settings.create_сlan3"))
            {
                int id = int.Parse(tab.Split(':')[1]);
                c.AddTitle("СОЗДАНИЯ АББРЕВИАТУРЫ");
                c.SetText("@@\nВыберите краткое имя клана, заглавными латинскими буквами.\n1-3 буквы. Оно используется в списках, командах консоли и пр.\n\nНапример, Хр@нители - HRA, Герои Меча - GRM\nВыберите сокращение, по которому легко узнать ваш клан.\n");
                c.AddButton("ДАЛЕЕ", $"create_сlan4:{id}#{tab.Split(':')[2]}#%I%");
                c.AddCard("c", id.ToString(), $"\n<color=white>{tab.Split(':')[2]}</color>\n");
                c.AddIConsolePlace("Аббревиатура клана");
                c.AddButton("ВЫХОД", "exit");
            }
            else if (tab.StartsWith("!!settings.create_сlan4"))
            {
                int id = int.Parse(tab.Split(':')[1].Split('#')[0]);
                c.AddTitle("ЗАВЕРШЕНИЕ СОЗДАНИЯ КЛАНА");
                c.SetText("@@\nВсе готово для создания клана. Остался последний этап.\n\n<color=#ff8888ff>Условия:</color>\n1. При создании спишется залог 1000 кредитов.\n2. При удалении клана 90% залога возвращается.\n3. При неактивности игроков в течение 2 месяцев клан удаляется.\n4. Мультоводство в игре запрещено. Использование нескольких\nаккаунтов одним человеком может повлечь штраф и санкции вплоть\nдо бана аккаунтов и удаления клана.\n");
                c.AddButton("<color=#ff8888ff>ПРИНИМАЮ УСЛОВИЯ</color>", $"create_сlan5:{id}#{tab.Split('#')[1]}#{tab.Split('#')[2]}");
                c.AddCard("c", id.ToString(), $"\n<color=white>{tab.Split('#')[1]} [{tab.Split('#')[2]}]</color>\n");
                c.AddButton("ВЫХОД", "exit");
            }
            else if (tab == "!!settings.create_сlan5")
            {
                c.AddTitle("КЛАН СОЗДАН");

                c.AddButton("ВЫХОД", "exit");
            }
            else if (tab.StartsWith("!!settings.choose"))
            {
                int id = int.Parse(tab.Split(':')[1]) - 200;
                c.AddTitle("ВЫБОР НАЗВАНИЯ КЛАНА");
                c.SetText("@@\nВыберите название клана.\nВ игре есть модерация, оскорбительные кланы могут быть удалены!\n\nВнимание! Название клана нельзя будет изменить после создания.\n");
                c.AddButton("ПРОДОЛЖИТЬ", $"create_сlan3:{id.ToString()}:%I%");
                c.AddCard("c", id.ToString(), "\n < - - значок вашего клана");
                c.AddIConsolePlace("Название вашего клана");
                c.AddButton("ВЫХОД", "exit");
            }
            p.win = winid + "."+ tab;
            c.Send(p.win, p);
        }
    }
}
