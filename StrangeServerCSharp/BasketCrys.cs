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
            if (cry[index] < 0)
            {
                cry[index] = long.MaxValue;
            }
            player.connection.Send("@B", GetCry);
        }

        public void UpdateBasket()
        {
            player.connection.Send("@B", GetCry);
        }

        public void ClearCrys()
        {
            for (var i = 0; i < this.cry.Length; i++)
            {
                this.cry[i] = 0;
            }

            player.connection.Send("@B", GetCry);
        }

        public void Boxcrys(long[] crys)
        {
            for (var i = 0; i < this.cry.Length; i++)
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

        public HorbBuilder BuildBox()
        {
            return new HorbBuilder()
                .SetTitle("Создание бокса")
                .AddTextLines("\nИспользуйте полосы прокрутки, чтобы выбрать сколько положить в бокс",
                    "ВНИМАНИЕ! При создании бокса теряется нихуя кристаллов\n")
                .AddCrysLeft("  останется")
                .AddCrysRight("будет в боксе")
                .AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[0], value = 0, descText = "" })
                .AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[1], value = 0, descText = "" })
                .AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[2], value = 0, descText = "" })
                .AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[3], value = 0, descText = "" })
                .AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[4], value = 0, descText = "" })
                .AddCrysLine(new CrysLine { leftMin = 0, rightMin = 0, d = cry[5], value = 0, descText = "" })
                .AddButton("<color=green>В БОКС</color>", "dropbox:%M%")
                .AddButton("ДО 50% ГРУЗА", "drophalf")
                .AddButton("ДО ПЕРЕГРУЗА", "dropcap")
                .AddButton("100% В БОКС", "dropall")
                .AddButton("ВЫЙТИ", "exit");
        }

        public long AllCry => this.cry.Select((t, i) => cry[i]).Sum();

        public string GetCry => this.cry.Aggregate("", (current, t) => current + (t + ":")) + cap;

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