namespace TransportTycoon.MapData
{
    /// <summary>
    /// Represents an abstract base class for goods that includes tax calculation functionality.
    /// </summary>
    /// <remarks>The Goods class provides a static tax value shared across all instances and methods for
    /// calculating and updating the tax applied to goods. Inherit from this class to implement specific types of goods
    /// that require tax-based value calculations.</remarks>
    public abstract class Goods : Load
    {
        /// <summary>
        /// Gets or sets the tax value used in calculations.
        /// </summary>
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

    /// <summary>
    /// Represents wheat as a type of goods with predefined price and load type.
    /// </summary>
    /// <remarks>This class is sealed and cannot be inherited. It is typically used to model wheat in
    /// inventory, trading, or logistics scenarios where goods are categorized by type.</remarks>
    public sealed class Wheat : Goods
    {
        /// <summary>
        /// Initializes a new instance of the Wheat class with default values for price and load type.
        /// </summary>
        public Wheat()
        {
            Price = 25;
            LoadType = LoadType.Wheat;
        }
    }

    /// <summary>
    /// Represents oil as a type of goods with predefined price and load type.
    /// </summary>
    /// <remarks>This class is sealed and cannot be inherited. It sets the price and load type specific to oil
    /// upon initialization.</remarks>
    public sealed class Oil : Goods
    {
        /// <summary>
        /// Initializes a new instance of the Oil class with default values for price and load type.
        /// </summary>
        public Oil()
        {
            Price = 45;
            LoadType = LoadType.Oil;
        }
    }

    /// <summary>
    /// Represents a goods item of type wood with a predefined price and load type.
    /// </summary>
    public sealed class Wood : Goods
    {
        /// <summary>
        /// Initializes a new instance of the Wood class with default values for price and load type.
        /// </summary>
        public Wood()
        {
            Price = 30;
            LoadType = LoadType.Wood;
        }
    }

    /// <summary>
    /// Represents flour as a type of goods with predefined price and load type.
    /// </summary>
    /// <remarks>This class is sealed and cannot be inherited. It initializes the price and load type specific
    /// to flour upon creation.</remarks>
    public sealed class Flour : Goods
    {
        /// <summary>
        /// Initializes a new instance of the Flour class with default values.
        /// </summary>
        public Flour()
        {
            Price = 65;
            LoadType = LoadType.Flour;
        }
    }

    /// <summary>
    /// Represents goods of type rubber with predefined price and load type settings.
    /// </summary>
    public sealed class Rubber : Goods
    {
        /// <summary>
        /// Initializes a new instance of the Rubber class with default values for price and load type.
        /// </summary>
        public Rubber()
        {
            Price = 110;
            LoadType = LoadType.Rubber;
        }
    }

    /// <summary>
    /// Represents a goods item of type paper with predefined price and load type.
    /// </summary>
    public sealed class Paper : Goods
    {
        /// <summary>
        /// Initializes a new instance of the Paper class with default values.
        /// </summary>
        public Paper()
        {
            Price = 75;
            LoadType = LoadType.Paper;
        }
    }
}
