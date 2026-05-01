namespace TransportTycoon.MapData.Buildings

{
    public abstract class BuildingEntity
    {
        #region Properties
        public int MaxCapacity { private set; get; } = 1000;

        public double CurrentCapacity { set; get; } = 0;

        private double _productivity = 1;

        public double Productivity
        {
            get
            {
                return _productivity * GetMultiplier();
            }
            set
            {
                _productivity = value;
            }
        }

        /// <summary>
        /// factor that determines the production rate of the building. The actual production is calculated as Scaler * Productivity.
        /// </summary>
        public int Scaler { protected set; get; }
        public int Offset { protected set; get; }

        /// <summary>
        /// Stores the coordinates of the building on map
        /// </summary>
        public Dictionary<(int X, int Y), IField> MapPoints { protected set; get; } = [];

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
        public abstract Load? GetConsumeLoad();
        /// <summary>
        /// Returns the facility's provided load type
        /// </summary>
        /// <returns>The load type</returns>
        public abstract Load GetProvideLoad();
        public abstract void GenerateBuildingPoints(int startX, int startY, int[,] heightMap);
        #endregion

        #region Virtual methods
        /// <summary>
        /// The production itself
        /// </summary>
        public virtual void Production()
        {
            if (CurrentCapacity < MaxCapacity)
            {
                double production = Math.Round(Scaler * Productivity);
                CurrentCapacity = Math.Min(CurrentCapacity + production, MaxCapacity);
            }
        }
        #endregion

        #region Protected methods
        protected double GetMultiplier()
        {
            double period = 300;
            double time = DateTime.Now.TimeOfDay.TotalSeconds;

            // sin()->[-1,1]
            // 0.5*sin() ->[-0.5, 0.5]
            // 1.5 + 0.5*sin() ->[1.0, 2.0]

            return 1.5 + 0.5 * Math.Sin((2 * Math.PI * (time + Offset)) / period);
        }
        #endregion

        #region Public methods
        public void SetCurrentCapacity(int currentCapacity)
        {
            if (0 <= currentCapacity && currentCapacity <= MaxCapacity)
            {
                CurrentCapacity = currentCapacity;
            }
        }
        #endregion
    }

    public sealed class CityEntity : BuildingEntity
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
        public override Load? GetConsumeLoad() => null;
        public override Load GetProvideLoad() => new People();
        public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    House tile = new(startX + i, startY + j, this)
                    {
                        Height = heightMap[startX + i, startY + j]
                    };
                    MapPoints.Add((startX + i, startY + j), tile);

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

    public sealed class LumberCampEntity : SiteEntity
    {
        #region Constructors
        public LumberCampEntity()
        {
            Offset = 20;
        }
        #endregion

        #region Public methods
        public override Load? GetConsumeLoad() => null;
        public override Load GetProvideLoad() => new Wood();
        public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    LumberCamp tile = new(startX + i, startY + j, this)
                    {
                        Height = heightMap[startX + i, startY + j]
                    };
                    MapPoints.Add((startX + i, startY + j), tile);
                }
            }
        }
        #endregion
    }

    public sealed class MineEntity : SiteEntity
    {
        #region Constructors
        public MineEntity()
        {
            Offset = 30;
        }
        #endregion

        #region Public methods
        public override Load? GetConsumeLoad() => null;
        public override Load GetProvideLoad() => new Oil();
        public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Mine tile = new(startX + i, startY + j, this)
                    {
                        Height = heightMap[startX + i, startY + j]
                    };
                    MapPoints.Add((startX + i, startY + j), tile);
                }
            }
        }
        #endregion
    }

    public sealed class FarmEntity : SiteEntity
    {
        #region Constructors
        public FarmEntity()
        {
            Offset = 40;
        }
        #endregion

        #region Public methods
        public override Load? GetConsumeLoad() => null;
        public override Load GetProvideLoad() => new Wheat();
        public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Farm tile = new(startX + i, startY + j, this)
                    {
                        Height = heightMap[startX + i, startY + j]
                    };
                    MapPoints.Add((startX + i, startY + j), tile);
                }
            }
        }
        #endregion
    }

    public abstract class IndustryEntity : BuildingEntity
    {
        #region Properties
        public int MaxConsumeCapacity { private set; get; } = 1000;
        public double ConsumeCapacity { private set; get; } = 0;
        #endregion

        #region Constructors
        protected IndustryEntity()
        {
            Scaler = 2;
        }
        #endregion

        #region Protected methods
        public override void Production()
        {
            if ((int)ConsumeCapacity == 0 || (int)CurrentCapacity == (int)MaxCapacity) return;

            double production = Math.Round(Scaler * Productivity);

            if (ConsumeCapacity < production) production = ConsumeCapacity;

            if (CurrentCapacity + production > MaxCapacity)
            {
                ConsumeCapacity -= MaxCapacity - CurrentCapacity;
                CurrentCapacity = MaxCapacity;
            }
            else
            {
                ConsumeCapacity -= production;
                CurrentCapacity += production;
            }
        }
        public void SetConsumeCapacity(int value)
        {
            if (value >= 0)
            {
                ConsumeCapacity = value;
            }

        }
        #endregion
    }

    public sealed class MillEntity : IndustryEntity
    {
        #region Constructors
        public MillEntity()
        {
            Offset = 70;
        }
        #endregion

        #region Public methods
        public override Load? GetConsumeLoad() => new Wheat();
        public override Load GetProvideLoad() => new Flour();
        public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Mill tile = new(startX + i, startY + j, this)
                    {
                        Height = heightMap[startX + i, startY + j]
                    };
                    MapPoints.Add((startX + i, startY + j), tile);
                }
            }
        }
        #endregion
    }

    public sealed class PlantEntity : IndustryEntity
    {
        #region Constructors
        public PlantEntity()
        {
            Offset = 60;
        }
        #endregion

        #region Public methods
        public override Load? GetConsumeLoad() => new Oil();
        public override Load GetProvideLoad() => new Rubber();
        public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Plant tile = new(startX + i, startY + j, this)
                    {
                        Height = heightMap[startX + i, startY + j]
                    };
                    MapPoints.Add((startX + i, startY + j), tile);
                }
            }
        }
        #endregion
    }

    public sealed class FactoryEntity : IndustryEntity
    {
        #region Constructors
        public FactoryEntity()
        {
            Offset = 50;
        }
        #endregion

        #region Public methods
        public override Load? GetConsumeLoad() => new Wood();
        public override Load GetProvideLoad() => new Paper();
        public override void GenerateBuildingPoints(int startX, int startY, int[,] heightMap)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Factory tile = new(startX + i, startY + j, this)
                    {
                        Height = heightMap[startX + i, startY + j]
                    };
                    MapPoints.Add((startX + i, startY + j), tile);
                }
            }
        }
        #endregion
    }
}
