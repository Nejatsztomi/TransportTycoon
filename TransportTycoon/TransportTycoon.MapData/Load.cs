namespace TransportTycoon.MapData
{
    public enum LoadType
    {
        None,
        Wheat,
        Oil,
        Wood,
        Flour,
        Rubber,
        Paper,
        People
    }

    public abstract class Load
    {
        #region Properties
        public int Price { protected get; set; }
        #endregion
    }

    public class People : Load
    {
        #region Constructors
        public People()
        {
            Price = 120;
        }
        #endregion
    }
}
