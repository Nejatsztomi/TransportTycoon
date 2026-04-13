namespace TransportTycoon.MapData.Buildings
{
    public abstract class BuildingEntity
    {
        #region Properties
        /// <summary>
        /// Mennyit tud tárolni
        /// </summary>
        public int Capacity { protected set; get; } = 1000;

        /// <summary>
        /// Jelenleg mennyit termelt
        /// </summary>
        public int Occupancy { protected set; get; } = 0;

        /// <summary>
        /// Milyen mennyiséggel termel
        /// </summary>
        public int Productivity { protected set; get; } = 1;

        /// <summary>
        /// Melyik telephely milyen szorzóval termel
        /// </summary>
        public int Scaler { protected set; get; }
        public int Offset { protected set; get; }

        /// <summary>
        /// Stores the coordinates of the building on map
        /// </summary>
        public Dictionary<(int X, int Y), Field> MapPoints { protected set; get; } = [];

        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Gets the coordinates of the top-left point of the building
        /// </summary>
        public (int X, int Y) TopLeftPoints => MapPoints.Keys.OrderBy(p => p.X).ThenBy(p => p.Y).FirstOrDefault();
        
        #endregion

        #region Constructors
        protected BuildingEntity(int width = 2, int height = 2)
        {
            Width = width;
            Height = height;
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// Returns the facility's consume load type
        /// </summary>
        /// <returns>The load type</returns>
        public abstract LoadType GetConsumeLoad();
        /// <summary>
        /// Returns the facility's provided load type
        /// </summary>
        /// <returns>The load type</returns>
        public abstract LoadType GetProvideLoad();
        public abstract void GenerateBuildingPoints(int startX, int startY);
        #endregion

        #region Virtual methods
        /// <summary>
        /// The production itself
        /// </summary>
        protected virtual void Production()
        {
            int production = (int)Math.Round(Scaler * Productivity * GetMultiplier());

            Occupancy = Math.Min(Occupancy + production, Capacity);
        }
        #endregion

        #region Protected methods
        protected double GetMultiplier()
        {
            double period = 300;
            double time = DateTime.Now.TimeOfDay.Seconds;

            // sin()->[-1,1]
            // 0.5*sin() ->[-0.5, 0.5]
            // 1.5 + 0.5*sin() ->[1.0, 2.0]

            return 1.5 + 0.5 * Math.Sin((2 * Math.PI * (time + Offset)) / period);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <returns>The maximum what the factory can give</returns>
        public int Unload(int q)
        {
            Occupancy = Math.Max(Occupancy - q, 0);
            return Occupancy;
        }
        public void SetOccupancy(int occupancy)
        {
            if (0 <= occupancy && occupancy <= Capacity)
            {
                Occupancy = occupancy;
            }
        }
        #endregion
    }

    public class CityEntity : BuildingEntity
    {
        #region Constructors
        public CityEntity(int width, int height) : base(width, height)
        {
            Offset = 10;
            Scaler = 1;
        }
        #endregion

        #region Public methods
        #region Overrides
        public override LoadType GetConsumeLoad() => LoadType.None;
        public override LoadType GetProvideLoad() => LoadType.People;
        public override void GenerateBuildingPoints(int startX, int startY)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    MapPoints.Add((startX + i, startY + j), new House(startX + i, startY + j, this));
                }
            }
        }
        #endregion
        #endregion
    }

    public abstract class SiteEntity : BuildingEntity
    {
        #region Constructors
        protected SiteEntity()
        {
            Scaler = 1;
        }
        #endregion
    }

    public class LumberCampEntity : SiteEntity
    {
        #region Constructors
        public LumberCampEntity()
        {
            Offset = 20;
        }
        #endregion

        #region Public methods
        public override LoadType GetConsumeLoad() => LoadType.None;
        public override LoadType GetProvideLoad() => LoadType.Wood;
        public override void GenerateBuildingPoints(int startX, int startY)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    MapPoints.Add((startX + i, startY + j), new LumberCamp(startX + i, startY + j, this));
                }
            }
        }
        #endregion
    }

    public class MineEntity : SiteEntity
    {
        #region Constructors
        public MineEntity()
        {
            Offset = 30;
        }
        #endregion

        #region Public methods
        public override LoadType GetConsumeLoad() => LoadType.None;
        public override LoadType GetProvideLoad() => LoadType.Oil;
        public override void GenerateBuildingPoints(int startX, int startY)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    MapPoints.Add((startX + i, startY + j), new Mine(startX + i, startY + j, this));
                }
            }
        }
        #endregion
    }

    public class FarmEntity : SiteEntity
    {
        #region Constructors
        public FarmEntity()
        {
            Offset = 40;
        }
        #endregion

        #region Public methods
        public override LoadType GetConsumeLoad() => LoadType.None;
        public override LoadType GetProvideLoad() => LoadType.Wheat;
        public override void GenerateBuildingPoints(int startX, int startY)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    MapPoints.Add((startX + i, startY + j), new Farm(startX + i, startY + j, this));
                }
            }
        }
        #endregion
    }

    public abstract class IndustryEntity : BuildingEntity
    {
        #region Properties
        public int ConsumeOccupancy { protected set; get; }
        #endregion

        #region Constructors
        protected IndustryEntity()
        {
            Scaler = 2;
        }
        #endregion

        #region Protected methods
        protected override void Production()
        {
            double multiplier = GetMultiplier();
            int production = (int)Math.Round(Scaler * ConsumeOccupancy * Productivity * multiplier);

            if (Occupancy + production > Capacity)
            {
                Occupancy = Capacity;
                ConsumeOccupancy = (int)Math.Round((production / Scaler) / (Productivity * multiplier));
            }
            else
            {
                Occupancy += production;
                ConsumeOccupancy = 0;
            }
        }
        #endregion
    }

    public class MillEntity : IndustryEntity
    {
        #region Constructors
        public MillEntity()
        {
            Offset = 70;
        }
        #endregion
        #region Public methods
        public override LoadType GetConsumeLoad() => LoadType.Wheat;
        public override LoadType GetProvideLoad() => LoadType.Flour;
        public override void GenerateBuildingPoints(int startX, int startY)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    MapPoints.Add((startX + i, startY + j), new Mill(startX + i, startY + j, this));
                }
            }
        }
        #endregion
    }

    public class PlantEntity : IndustryEntity
    {
        #region Constructors
        public PlantEntity()
        {
            Offset = 60;
        }
        #endregion

        #region Public methods
        public override LoadType GetConsumeLoad() => LoadType.Wood;
        public override LoadType GetProvideLoad() => LoadType.Paper;
        public override void GenerateBuildingPoints(int startX, int startY)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    MapPoints.Add((startX + i, startY + j), new Plant(startX + i, startY + j, this));
                }
            }
        }
        #endregion
    }

    public class FactoryEntity : IndustryEntity
    {
        #region Constructors
        public FactoryEntity()
        {
            Offset = 50;
        }
        #endregion

        #region Public methods
        public override LoadType GetConsumeLoad() => LoadType.Oil;
        public override LoadType GetProvideLoad() => LoadType.Rubber;
        public override void GenerateBuildingPoints(int startX, int startY)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    MapPoints.Add((startX + i, startY + j), new Factory(startX + i, startY + j, this));
                }
            }
        }
        #endregion
    }
}
