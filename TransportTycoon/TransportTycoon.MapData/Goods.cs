namespace TransportTycoon.MapData
{
    public abstract class Goods : Load
    {
        public static int Tax { get; protected set; }

        protected int Value()
        {
            return Tax * Price;
        }

        //nincs benne az osztálydiagrammba
        public static void SetGlobalTax(int tax)
        {
            Tax = tax;
        }
    }

    public class Wheat : Goods
    {
        public Wheat()
        {
            Price = 150;
        }
    }

    public class Oil : Goods
    {
        public Oil()
        {
            Price = 200;
        }
    }

    public class Wood : Goods
    {
        public Wood()
        {
            Price = 130;
        }
    }

    public class Flour : Goods
    {
        public Flour()
        {
            Price = 400;

        }
    }

    public class Rubber : Goods
    {
        public Rubber()
        {
            Price = 260;
        }
    }

    public class Paper : Goods
    {
        public Paper()
        {
            Price = 300;
        }
    }
}
