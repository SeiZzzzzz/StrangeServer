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
        public int isca { get; set; }
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
            p.connection.Send("#S",
                "#cc#10#snd#0#mus#0#isca#" + p.settings.isca + "#tsca#" + p.settings.isca + "#mous#" + p.settings.mous +
                "#pot#" + p.settings.pot + "#frc#" + p.settings.frc + "#ctrl#" + p.settings.ctrl + "#mof#" +
                p.settings.mof);
        }

        public void Open(string tab, Player p)
        {
            var builder = new HorbBuilder();
            if (tab == winid)
            {
                builder.SetTitle("НАСТРОЙКИ")
                    .AddButton("СОХРАНИТЬ", "change_settings:%R%");
                if (p.clanid == 0)
                {
                    builder.AddButton("СОЗДАТЬ КЛАН", "create_сlan");
                }

                var l = RichListGenerator
                    .Drop("Масштаб интерфейса", "isca", new string[] { "МЕЛКО", "КРУПНО" }, isca).ToList()
                    .Concat(RichListGenerator
                        .Drop("Масштаб территории", "tsca", new string[] { "МЕЛКО", "КРУПНО" }, tsca).ToList())
                    .Concat(RichListGenerator.Bool("Включить управление мышкой", "mous", mous == 0 ? false : true)
                        .ToList())
                    .Concat(
                        RichListGenerator.Bool("Упрощенный режим графики", "pot", pot == 0 ? false : true).ToList())
                    .Concat(RichListGenerator.Bool("Принудительно обновлять породы (увеличит потр. CPU)", "frc",
                        pot == 0 ? false : true).ToList())
                    .Concat(RichListGenerator.Bool("CTRL переключает скорость робота (вместо удерживания)", "ctrl",
                        ctrl == 0 ? false : true).ToList())
                    .Concat(RichListGenerator.Bool("Отключить ближайшие звуки", "mof", mof == 0 ? false : true)
                        .ToList()).ToArray();
                builder.AddButton("ВЫЙТИ", "exit")
                    .AddTab("ГОРЯЧИЕ КЛАВИШИ", "hots")
                    .AddTab("КОНФИГ", "config")
                    .SetRichList(l);
            }
            else if (tab == "!!settings.create_сlan")
            {
                builder.SetTitle("СОЗДАНИЕ НОВОГО КЛАНА")
                    .SetText(
                        "@@\nУра! Вы собираетесь создать новый клан. После создания клана вы сможете\nвыполнять клановые квесты, создавать свои фермы, вести войны с другими\nкланами, защищать и отбивать территории, и многое другое.\n\nСоздание клана - ответственное действие, значок и название клана нельзя\nбудет изменить позже. Поэтому внимательно подумайте над тем, как будет\nзвучать и выглядеть ваш клан в игре.\n\nСоздание клана требует залога в 1000 кредитов.\n")
                    .AddButton("ВЫБРАТЬ ЗНАЧОК КЛАНА", "@create_сlan2")
                    .AddButton("ВЫХОД", "exit");
            }
            else if (tab == "!!settings.create_сlan2")
            {
                builder.SetTitle("ВЫБОР ЗНАЧКА КЛАНА")
                    .SetText(
                        "@@Выберите значок клана. Всего значков больше сотни. Для удобства мы\nпоказываем их небольшими порциями. Нажмите ДРУГИЕ, чтобы посмотреть еще.\nДля выбора значка - кликните на него.\n\nВнимание! Значок клана нельзя будет изменить после создания.\n")
                    .SetInv(string.Join(":", Clan.GetAvlClanIcon().Select(ic => 200 + ic.id + ":0")))
                    .AddButton("ДРУГИЕ", "@create_сlan2")
                    .AddButton("НАЗАД", "<create_сlan");
            }
            else if (tab.StartsWith("!!settings.create_сlan3"))
            {
                var id = int.Parse(tab.Split(':')[1]);
                builder.SetTitle("СОЗДАНИЯ АББРЕВИАТУРЫ")
                    .SetText(
                        "@@\nВыберите краткое имя клана, заглавными латинскими буквами.\n1-3 буквы. Оно используется в списках, командах консоли и пр.\n\nНапример, Хр@нители - HRA, Герои Меча - GRM\nВыберите сокращение, по которому легко узнать ваш клан.\n")
                    .AddButton("ДАЛЕЕ", $"create_сlan4:{id}#{tab.Split(':')[2]}#%I%")
                    .AddCard("c", id.ToString(), $"\n<color=white>{tab.Split(':')[2]}</color>\n")
                    .AddIConsolePlace("Аббревиатура клана")
                    .AddButton("ВЫХОД", "exit");
            }
            else if (tab.StartsWith("!!settings.create_сlan4"))
            {
                var id = int.Parse(tab.Split(':')[1].Split('#')[0]);
                builder.SetTitle("ЗАВЕРШЕНИЕ СОЗДАНИЯ КЛАНА")
                    .SetText(
                        "@@\nВсе готово для создания клана. Остался последний этап.\n\n<color=#ff8888ff>Условия:</color>\n1. При создании спишется залог 1000 кредитов.\n2. При удалении клана 90% залога возвращается.\n3. При неактивности игроков в течение 2 месяцев клан удаляется.\n4. Мультоводство в игре запрещено. Использование нескольких\nаккаунтов одним человеком может повлечь штраф и санкции вплоть\nдо бана аккаунтов и удаления клана.\n")
                    .AddButton("<color=#ff8888ff>ПРИНИМАЮ УСЛОВИЯ</color>",
                        $"create_сlan5:{id}#{tab.Split('#')[1]}#{tab.Split('#')[2]}")
                    .AddCard("c", id.ToString(), $"\n<color=white>{tab.Split('#')[1]} [{tab.Split('#')[2]}]</color>\n")
                    .AddButton("ВЫХОД", "exit");
            }
            else if (tab == "!!settings.create_сlan5")
            {
                builder.SetTitle("КЛАН СОЗДАН")
                    .AddButton("ВЫХОД", "exit");
            }
            else if (tab.StartsWith("!!settings.choose"))
            {
                var id = int.Parse(tab.Split(':')[1]) - 200;
                builder.SetTitle("ВЫБОР НАЗВАНИЯ КЛАНА")
                    .SetText(
                        "@@\nВыберите название клана.\nВ игре есть модерация, оскорбительные кланы могут быть удалены!\n\nВнимание! Название клана нельзя будет изменить после создания.\n")
                    .AddButton("ПРОДОЛЖИТЬ", $"create_сlan3:{id.ToString()}:%I%")
                    .AddCard("c", id.ToString(), "\n < - - значок вашего клана")
                    .AddIConsolePlace("Название вашего клана")
                    .AddButton("ВЫХОД", "exit");
            }

            p.win = winid + "." + tab;
            builder.Send(p.win, p);
        }
    }
}