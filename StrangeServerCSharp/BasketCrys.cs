namespace StrangeServerCSharp
{
    public class BasketCrys
    {
        public Player player;
        public BasketCrys(Player pl)
        {
            player = pl;
        }
        public void AddCrys(int index, long val)
        {
            this.cry[index] += val;
            player.connection.Send("@B", GetCry);
        }
        public void UpdateBasket()
        {
            player.connection.Send("@B", GetCry);
        }
        public void ClearCrys()
        {
            for (int i = 0; i < this.cry.Length; i++)
            {
                this.cry[i] = 0;
            }
            player.connection.Send("@B", GetCry);
        }
        public void Boxcrys(long[] crys)
        {
            for (int i = 0; i < this.cry.Length; i++)
            {
                cry[i] += crys[i];
            }
            player.connection.Send("@B", GetCry);
        }
        public bool RemoveCrys(int index, long val)
        {
            if (val < 0)
            {
                return false;
            }
            if ((this.cry[index] - val) >= 0)
            {
                this.cry[index] -= val;
                player.connection.Send("@B", GetCry);
                return true;
            }
            player.connection.Send("@B", GetCry);
            return false;

        }
        public bool BuyCrys(int index, long val)
        {
            if ((val < 0) || (player.money <= 0))
            {
                return false;
            }
            if (player.money - (val * World.costsbuy[index]) >= 0)
            {
                player.money -= (val * World.costsbuy[index]);
                this.cry[index] += val;
                player.connection.Send("@B", GetCry);
                return true;
            }
            player.connection.Send("@B", GetCry);
            return false;

        }
        public void BuildBox(HorbConst c)
        {
            c.AddTitle("Создание бокса");
            c.AddTextLine("\nИспользуйте полосы прокрутки, чтобы выбрать сколько положить в бокс");
            c.AddTextLine("ВНИМАНИЕ! При создании бокса теряется нихуя кристаллов\n");
            c.AddCrysLeft("  останется");
            c.AddCrysRight("будет в боксе");
            c.AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[0], value = 0, descText = "" });
            c.AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[1], value = 0, descText = "" });
            c.AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[2], value = 0, descText = "" });
            c.AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[3], value = 0, descText = "" });
            c.AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[4], value = 0, descText = "" });
            c.AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[5], value = 0, descText = "" });
            c.AddButton("<color=green>В БОКС</color>", "dropbox:%M%");
            c.AddButton("ДО 50% ГРУЗА", "drophalf");
            c.AddButton("ДО ПЕРЕГРУЗА", "dropcap");
            c.AddButton("100% В БОКС", "dropall");
            c.AddButton("ВЫЙТИ", "exit");
        }
        public long AllCry
        {
            get
            {
                long j = 0;
                for (int i = 0; i < this.cry.Length; i++)
                {
                    j += cry[i];
                }
                return j;
            }
        }
        public string GetCry
        {
            get
            {
                string text = "";
                for (int i = 0; i < this.cry.Length; i++)
                {
                    text += this.cry[i] + ":";
                }
                return text + cap;
            }
        }
        public long[] cry =
        {
            0,
            0,
            0,
            0,
            0,
            0
        };
        public int cap = 0;
    }
}
