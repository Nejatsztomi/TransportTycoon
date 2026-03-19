namespace TransportTycoon.MapData
{
    public abstract class BuildingBlocks : Field
    {
        #region Fields
        #endregion

        #region Properties
        //mennyit tud tárolni
        public int Capacity { protected set; get; } = 1000;
        //jeleneleg mennyit termelt
        public int Occupancy { protected set; get; }
        //milyen mennyiseggel termel
        public int Productivity { protected set; get; } = 1;
        //melyik telephely milyen szorzoval termel
        public int Scaler { protected set; get; }
        public int Offset { protected set; get; }
        public (int X, int Y) Id { protected set; get; }
        public (int X, int Y) Pointer { protected set; get; }
        #endregion

        #region Constructors
        protected BuildingBlocks(int x, int y)
        {
            X = x;
            Y = y;
            Height = -1;
            Modifiable = false;
            Occupancy = 0;
            Id = (x, y);
            Pointer = (x, y);
        }
        #endregion

        #region Public Methods
        protected double GetMultiplier()
        {
            double period = 300;
            double time = DateTime.Now.TimeOfDay.Seconds;

            //sin()->[-1,1]
            //0.5*sin() ->[-0.5, 0.5]
            //1.5 + 0.5*sin() ->[1.0, 2.0]

            double multiplier = 1.5 + 0.5 * Math.Sin((2 * Math.PI * (time + Offset)) / period);

            return multiplier;
        }

        //the production itself
        protected virtual void Production()
        {
            double multiplier = GetMultiplier();
            int production = Scaler * Convert.ToInt32((double)Productivity * multiplier);

            if (Occupancy + production > Capacity)
            {
                Occupancy = Capacity;
            }
            else
            {
                Occupancy += production;
            }
        }


        public bool IsMain()
        {
            return Id.X == Pointer.X && Id.Y == Pointer.Y;
        }

        //Returns the facility's load  
        public abstract LoadType GetLoad();


        public int Unload(int q) //returns the maximum what the factory can give
        {
            if (q >= Occupancy)
            {
                Occupancy = 0;
            }
            else
            {
                Occupancy -= q;
            }

            return Occupancy;
        }
        #endregion

        #region Private Methods
        #endregion
    }

    public class House : BuildingBlocks
    {
        #region Constructors
        public House(int x, int y) : base(x, y)
        {
            Offset = 10;
            Scaler = 1;
        }
        #endregion

        #region Methods
        public override LoadType GetLoad()
        {
            return LoadType.People;
        }
        #endregion
    }
}
