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
        public int Price { get; protected set; }
        public LoadType LoadType { get; set; }
        #endregion

        public sealed class People : Load
        {
            #region Constructors
            public People()
            {
                Price = 120;
                LoadType = LoadType.People;
            }
            #endregion
        }
    }
}
