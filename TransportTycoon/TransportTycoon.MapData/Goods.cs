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
        }
    }

    public sealed class Oil : Goods
    {
        public Oil()
        {
            Price = 200;
        }
    }

    public sealed class Wood : Goods
    {
        public Wood()
        {
            Price = 130;
        }
    }

    public sealed class Flour : Goods
    {
        public Flour()
        {
            Price = 400;

        }
    }

    public sealed class Rubber : Goods
    {
        public Rubber()
        {
            Price = 260;
        }
    }

    public sealed class Paper : Goods
    {
        public Paper()
        {
            Price = 300;
        }
    }
}
