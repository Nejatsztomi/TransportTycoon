namespace TransportTycoon.MapData
{
    public abstract class Goods : Load
    {
        public static int Tax { get; protected set; }


        /// <summary>
        /// Calculates the total value by multiplying the tax rate by the price.
        /// </summary>
        /// <returns>The product of the tax rate and the price.</returns>
        protected int Value()
        {
            return Tax * Price;
        }

        /// <summary>
        /// Sets the global tax value that is used to calculate the value of goods. The value of goods is calculated by multiplying the price of the good by the global tax. This method can be used to change the global tax value when needed, for example, when the player wants to increase or decrease the tax rate in the game.
        /// </summary>
        /// <param name="tax"></param>
        public static void SetGlobalTax(int tax)
        {
            Tax = tax;
        }
    }

    public sealed class Wheat : Goods
    {
        public Wheat()
        {
            Price = 25;
            LoadType = LoadType.Wheat;
        }
    }

    public sealed class Oil : Goods
    {
        public Oil()
        {
            Price = 45;
            LoadType = LoadType.Oil;
        }
    }

    public sealed class Wood : Goods
    {
        public Wood()
        {
            Price = 30;
            LoadType = LoadType.Wood;
        }
    }

    public sealed class Flour : Goods
    {
        public Flour()
        {
            Price = 65;
            LoadType = LoadType.Flour;
        }
    }

    public sealed class Rubber : Goods
    {
        public Rubber()
        {
            Price = 110;
            LoadType = LoadType.Rubber;
        }
    }

    public sealed class Paper : Goods
    {
        public Paper()
        {
            Price = 75;
            LoadType = LoadType.Paper;
        }
    }
}
