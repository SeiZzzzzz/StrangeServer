namespace StrangeServerCSharp
{
    public class Box
    {
        public static void BuildBox(uint x, uint y, long[] cry)
        {
            if (!(World.THIS.GetCellConst(x, y).is_empty && World.THIS.GetCellConst(x, y).can_build_over))
            {
                return;
            }
            World.boxmap[x + y * World.height] = new Box(cry);
            World.THIS.SetCell(x, y, 90);
        }
        public Box(long[] boxcrys)
        {
            bxcry = boxcrys.Clone() as long[];
        }
        public long AllCrys
        {
            get
            {
                return bxcry.Sum();
            }
        }
        public static void CollectBox(uint x, uint y, Player p)
        {
            Box b = World.boxmap[x + y * World.height];
            p.crys.Boxcrys(b.bxcry);
            p.crys.UpdateBasket();
            World.THIS.DestroyCell(x, y);

        }
        public long[] bxcry;
    }
}
