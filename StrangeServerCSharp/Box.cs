namespace StrangeServerCSharp
{
    public class Box
    {
        public int id { get; set; }
        public static void BuildBox(uint x, uint y, long[] cry)
{
  BuildBox(x,y,cry,null);
}
        public static void BuildBox(uint x, uint y, long[] cry, Player p)
        {
            if (!(World.THIS.GetCellConst(x, y).is_empty && World.THIS.GetCellConst(x, y).can_build_over))
            {
                return;
            }
            if (!World.THIS.ValidForB(x, y))
            {
                return;
            }
            long[] bbox = new long[6];
            for (int i = 0; i < 6; i++)
            {
                long remcry = cry[i];
                if (p == null)
                {
                    bbox[i] = remcry;
                }
                else if (p.crys.RemoveCrys(i, remcry))
                {
                    bbox[i] = remcry;
                }
            }
            if (bbox.Sum() <= 0)
            {
                return;
            }
            var box = new Box() { bxcry = bbox.Clone() as long[], x = x, y = y };
            using var db = new BDClass();
            db.boxes.Add(box);
            db.SaveChanges();
            World.boxmap[x + y * World.height] = box;
            World.THIS.SetCell(x, y, 90);
        }

        public Box()
        {
        }

        public long AllCrys => bxcry.Sum();

        public static void CollectBox(uint x, uint y, Player p)
        {
            var b = World.boxmap[x + y * World.height];
            p.crys.Boxcrys(b.bxcry);
            p.crys.UpdateBasket();
            World.THIS.DestroyCell(x, y);
            World.boxmap[x + y * World.height] = null;
        }

        public long[] bxcry = new long[6];

        public long ze
        {
            get { return bxcry[0]; }
            set { bxcry[0] = value; }
        }

        public long cr
        {
            get { return bxcry[1]; }
            set { bxcry[1] = value; }
        }

        public long si
        {
            get { return bxcry[2]; }
            set { bxcry[2] = value; }
        }

        public long be
        {
            get { return bxcry[3]; }
            set { bxcry[3] = value; }
        }

        public long fi
        {
            get { return bxcry[4]; }
            set { bxcry[4] = value; }
        }

        public long go
        {
            get { return bxcry[5]; }
            set { bxcry[5] = value; }
        }

        public uint x { get; set; }
        public uint y { get; set; }
    }
}