namespace StrangeServerCSharp
{
    public class Phys
    {
        public static Phys THIS;
        public Phys() { THIS = this; makedirt = new bool[World.width * World.height]; }
        public const float TPS = 3f;
        public Random rand = new Random();
        public static long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        public bool[] makedirt;
        public static async void Start()
        {
            await Task.Run(delegate ()
            {
                for (; ; )
                {
                    int ticksToProcess = (int)((DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastTick) / 1000f * TPS);
                    if (ticksToProcess > 0)
                    {

                        while (ticksToProcess-- > 0) { Update(); Thread.Sleep(10); }
                        lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    }
                }
            });
        }
        public static void Update()
        {
            for (uint y = 0; y < World.height; y++)
            {
                for (uint x = 0; x < World.width; x++)
                {
                    var cell = World.THIS.GetCell(x, y);
                    if (World.cellps[cell].is_falling && !THIS.makedirt[x + y * World.height])
                    {
                        THIS.Fall(x, y, cell);
                    }
                    else if (THIS.makedirt[x + y * World.height])
                    {
                        THIS.makedirt[x + y * World.height] = false;
                    }
                }
            }
        }
        public void TryToActivatec117(uint x, uint y)
        {
            if (v(x + 1, y) && rand.NextDouble() < 0.01)
            {

                World.THIS.SetCell(x + 1, y, 107, 10);

            }
            else if (v(x - 1, y) && rand.NextDouble() < 0.01)
            {
                World.THIS.SetCell(x - 1, y, 107, 10);
            }
            else if (v(x, y + 1) && rand.NextDouble() < 0.01)
            {

                World.THIS.SetCell(x, y + 1, 107, 10);

            }
            else if (v(x, y - 1) && rand.NextDouble() < 0.01)
            {

                World.THIS.SetCell(x, y - 1, 107, 10);

            }
        }
        public void TryToActivate(uint x, uint y, byte cell)
        {
            if (World.cellps[cell].is_alive)
            {
                Alive(x, y, cell);
            }
        }
        public List<int> alive = new List<int>()
        {
            50,
            51,
            52,
            53,
            54,
            55,
            116
        };
        public void CheckStruct(uint x, uint y, byte cell)
        {
            if (cell == 116) //синь
            {
            }
            else if (cell == 50) //голь
            {
            }
            else if (cell == 51) //крась
            {
                if (isStruct(x, y, cell))
                {
                    for (int cx = -1; cx <= 1; cx++)
                    {
                        for (int cy = -1; cy <= 1; cy++)
                        {
                            World.THIS.SetCell((uint)(x + cx), (uint)(y + cy), 91);
                        }
                    }
                }
            }
            else if (cell == 52) //фиоль
            {
            }
            else if (cell == 53) //чурка
            {
                if (isStructCher(x, y))
                {
                    World.THIS.SetCell(x, y, 115);
                }
            }
            else if (cell == 54) //бель
            {
            }
        }
        public void Alive(uint x, uint y, byte cell)
        {
            CheckStruct(x, y, cell);
            if (cell == 116) //синь
            {
                if (v(x + 1, y) && rand.Next(0, 100) < 20)
                {

                    World.THIS.SetCell(x + 1, y, 116);
                    World.THIS.SetCell(x, y, 109, 21);

                }
                else if (v(x - 1, y) && rand.Next(0, 100) < 20)
                {
                    World.THIS.SetCell(x - 1, y, 116);
                    World.THIS.SetCell(x, y, 109, 21);
                }
                else if (v(x, y + 1) && rand.Next(0, 100) < 20)
                {

                    World.THIS.SetCell(x, y + 1, 116);
                    World.THIS.SetCell(x, y, 109, 21);

                }
                else if (v(x, y - 1) && rand.Next(0, 100) < 20)
                {

                    World.THIS.SetCell(x, y - 1, 116);
                    World.THIS.SetCell(x, y, 109, 21);

                }
            }
            else if (cell == 50) //голь
            {
                if (v(x + 1, y))
                {

                    World.THIS.SetCell(x + 1, y, 112);

                }
                if (v(x - 1, y))
                {
                    World.THIS.SetCell(x - 1, y, 112);
                }
                if (v(x, y + 1))
                {

                    World.THIS.SetCell(x, y + 1, 112);

                }
                if (v(x, y - 1))
                {

                    World.THIS.SetCell(x, y - 1, 112);

                }
            }
            else if (cell == 51) //крась
            {
                bool plod = false;
                for (int cx = -1; cx <= 1; cx++)
                {
                    for (int cy = -1; cy <= 1; cy++)
                    {
                        if (isChs((uint)(x + cx), (uint)(y + cy)))
                        {
                            plod = true; break;
                        }
                    }
                }
                if (!plod)
                {
                    return;
                }
                if (v(x + 1, y))
                {

                    World.THIS.SetCell(x + 1, y, 108, 4);

                }
                if (v(x - 1, y))
                {
                    World.THIS.SetCell(x - 1, y, 108, 4);
                }
                if (v(x, y + 1))
                {

                    World.THIS.SetCell(x, y + 1, 108, 4);

                }
                if (v(x, y - 1))
                {

                    World.THIS.SetCell(x, y - 1, 108, 4);

                }
            }
            else if (cell == 52) //фиоль
            {
                bool plod = false;
                for (int cx = -1; cx <= 1; cx++)
                {
                    for (int cy = -1; cy <= 1; cy++)
                    {
                        if (isChs((uint)(x + cx), (uint)(y + cy)))
                        {
                            plod = true; break;
                        }
                    }
                }
                if (!plod)
                {
                    return;
                }
                if (v(x + 1, y))
                {

                    World.THIS.SetCell(x + 1, y, 110);

                }
                if (v(x - 1, y))
                {
                    World.THIS.SetCell(x - 1, y, 110);
                }
                if (v(x, y + 1))
                {

                    World.THIS.SetCell(x, y + 1, 110);

                }
                if (v(x, y - 1))
                {

                    World.THIS.SetCell(x, y - 1, 110);

                }
            }
            else if (cell == 53) //чурка
            {
                PlodChurk(x, y);
            }
            else if (cell == 54) //бель
            {
                PlodBel(x, y);
            }
            else if (cell == 55) //радужка
            {
                if (v(x + 1, y))
                {
                    byte al = GetRandomAlive();
                    if (al != 0)
                    {
                        World.THIS.SetCell(x + 1, y, al);
                    }
                }
                if (v(x - 1, y))
                {
                    byte al = GetRandomAlive();
                    if (al != 0)
                    {
                        World.THIS.SetCell(x - 1, y, al);
                    }
                }
                if (v(x, y + 1))
                {
                    byte al = GetRandomAlive();
                    if (al != 0)
                    {
                        World.THIS.SetCell(x, y + 1, al);
                    }
                }
                if (v(x, y - 1))
                {
                    byte al = GetRandomAlive();
                    if (al != 0)
                    {
                        World.THIS.SetCell(x, y - 1, al);
                    }
                }
            }
        }
        public byte GetRandomAlive()
        {
            byte cell = 0;


            if (rand.Next(0, 100) < 5)
            {
                cell = 55;
            }
            else if (rand.Next(0, 100) < 10)
            {
                cell = 50;
            }
            else if (rand.Next(0, 100) < 15)
            {
                cell = 51;
            }
            else if (rand.Next(0, 100) < 15)
            {
                cell = 52;
            }
            else if (rand.Next(0, 100) < 10)
            {
                cell = 53;
            }
            else if (rand.Next(0, 100) < 15)
            {
                cell = 54;
            }
            else if (rand.Next(0, 100) < 15)
            {
                cell = 116;
            }
            return cell;
        }
        public bool isCherk(uint x, uint y)
        {
            if (!World.THIS.ValidCoord(x, y))
            {
                return false;
            }
            return GetPres(x, y).id == 53;
        }
        public int ClearForFall(uint x, uint y)
        {

            if (vf((uint)(x), (y + 1)) && !THIS.makedirt[x + (y + 1) * World.height])
            {
                return 1;
            }
            else if (!THIS.makedirt[x + (y + 1) * World.height] && GetPres(x, y + 1).is_falling)
            {
                if (vf(x + 1, y + 1) && !THIS.makedirt[(x + 1) + (y + 1) * World.height])
                {
                    return 2;
                }
                else if (vf(x - 1, y + 1) && !THIS.makedirt[(x - 1) + (y + 1) * World.height])
                {
                    return 3;
                }
            }

            return 0;
        }
        public bool isChs(uint x, uint y)
        {
            if (!World.THIS.ValidCoord(x, y))
            {
                return false;
            }
            return GetPres(x, y).id == 114 || GetPres(x, y).id == 115;
        }
        public bool isStruct(uint x, uint y, byte cell)
        {
            for (int cx = -1; cx <= 1; cx++)
            {
                for (int cy = -1; cy <= 1; cy++)
                {
                    if (!World.THIS.ValidCoord((uint)(x + cx), (uint)(y + cy)))
                    {
                        return false;
                    }
                    if (GetPres((uint)(x + cx), (uint)(y + cy)).id != cell)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool isStructCher(uint x, uint y)
        {
            int c = 0;
            for (int cx = -1; cx <= 1; cx++)
            {
                for (int cy = -1; cy <= 1; cy++)
                {
                    if (World.THIS.ValidCoord((uint)(x + cx), (uint)(y + cy)))
                    {
                        if (GetPres((uint)(x + cx), (uint)(y + cy)).id == 53)
                        {
                            c++;
                        }
                    }
                }
            }
            if (c > 6)
            {
                return true;
            }
            return false;
        }
        public bool v(uint x, uint y)
        {
            if (World.ContPlayers(x, y).Count > 0)
            {
                return false;
            }
            if (!World.THIS.ValidForB(x, y))
            {
                return false;
            }
            return GetPres(x, y).is_empty;
        }
        public bool vf(uint x, uint y)
        {
            if (!World.THIS.ValidForB(x, y))
            {
                return false;
            }
            if (GetPres(x, y).id == 39)
            {
                return false;
            }
            return GetPres(x, y).is_empty;
        }
        public World.Cell GetPres(uint x, uint y)
        {
            return World.cellps[World.THIS.GetCell(x, y)];
        }
        public void Fall(uint x, uint y, byte cell)
        {
            var canf = ClearForFall(x, y);
            if (canf != 0)
            {
                World.THIS.DestroyCell(x, y);
                if (canf == 1 && vf(x, y + 1))
                {
                    THIS.makedirt[x + (y + 1) * World.height] = true;
                    World.THIS.SetCell((uint)(x), y + 1, cell);
                }
                else if (canf == 3 && vf(x - 1, y + 1))
                {
                    THIS.makedirt[(x - 1) + (y + 1) * World.height] = true;
                    World.THIS.SetCell((uint)(x - 1), y + 1, cell);
                }
                else if (canf == 2 && vf(x + 1, y + 1))
                {
                    THIS.makedirt[(x + 1) + (y + 1) * World.height] = true;
                    World.THIS.SetCell((uint)(x + 1), y + 1, cell);
                }

            }

        }
        public byte randcell()
        {
            var r = rand.Next(0, 100);
            if (r < 40)
            {
                return 112;
            }
            else if (r < 80)
            {
                return 108;
            }
            else
            {
                return 0;
            }



        }
        public void PlodBel(uint x, uint y)
        {

            var canplod = false;
            for (int bx = -1; bx <= 1; bx++)
            {
                for (int by = -1; by <= 1; by++)
                {
                    if (v((uint)(x + bx), (uint)(y + by)) && (y + by) != y - 1)
                    {
                        canplod = true;
                    }
                }
            }
            if (!canplod)
            {
                return;
            }
            var r = rand.Next(0, 100) < 20;
            if (!World.THIS.ValidCoord(x, y - 1))
            {
                return;
            }
            if (World.THIS.GetCell(x, y - 1) == 91)
            {
                if (r)
                {
                    World.THIS.DestroyCell(x, y - 1);
                }
            }
            else
            {
                return;
            }
            for (int bx = -1; bx <= 1; bx++)
            {
                for (int by = -1; by <= 1; by++)
                {
                    if (v((uint)(x + bx), (uint)(y + by)) && (y + by) != y - 1)
                    {
                        World.THIS.SetCell((uint)(x + bx), (uint)(y + by), 111);
                    }
                }
            }
        }
        public void PlodChurk(uint x, uint y)
        {
            if (isCherk(x, y + 1))
            {
                if (v(x, y + 2))
                {
                    World.THIS.SetCell(x, y + 2, randcell());
                }
                if (v(x, y - 1))
                {
                    World.THIS.SetCell(x, y - 1, randcell());
                }
            }
            else if (isCherk(x, y - 1))
            {
                if (v(x, y - 2))
                {
                    World.THIS.SetCell(x, y - 2, randcell());
                }
                if (v(x, y + 1))
                {
                    World.THIS.SetCell(x, y + 1, randcell());
                }
            }
            else if (isCherk(x + 1, y))
            {
                if (v(x + 2, y))
                {
                    World.THIS.SetCell(x + 2, y, randcell());
                }
                if (v(x - 1, y))
                {
                    World.THIS.SetCell(x - 1, y, randcell());
                }
            }
            else if (isCherk(x - 1, y))
            {
                if (v(x - 2, y))
                {
                    World.THIS.SetCell(x - 2, y, randcell());
                }
                if (v(x + 1, y))
                {
                    World.THIS.SetCell(x + 1, y, randcell());
                }
            }
        }
    }
}
