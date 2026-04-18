namespace TransportTycoon.MapData
{
    public abstract class Goods : Load
    {
        public static int Tax { get; protected set; }

        protected int Value()
        {
            return Tax * Price;
        }

        public static void SetGlobalTax(int tax)
        {
            Tax = tax;
        }
    }

    public sealed class Wheat : Goods
    {
        public Wheat()
        {
            Price = 150;
            LoadType = LoadType.Wheat;
        }
    }

    public sealed class Oil : Goods
    {
        public Oil()
        {
            Price = 200;
            LoadType = LoadType.Oil;
        }
    }

    public sealed class Wood : Goods
    {
        public Wood()
        {
            Price = 130;
            LoadType = LoadType.Wood;
        }
    }

    public sealed class Flour : Goods
    {
        public Flour()
        {
            Price = 400;
            LoadType = LoadType.Flour;
        }
    }

    public sealed class Rubber : Goods
    {
        public Rubber()
        {
            Price = 260;
            LoadType = LoadType.Rubber;
        }
    }

    public sealed class Paper : Goods
    {
        public Paper()
        {
            Price = 300;
            LoadType = LoadType.Paper;
        }
    }
}
