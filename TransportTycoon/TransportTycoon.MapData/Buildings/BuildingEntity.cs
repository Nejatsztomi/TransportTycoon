namespace TransportTycoon.MapData.Buildings
{
    /// <summary>
    /// An abstract class representing a building entity in the game.
    /// This class serves as a base for specific types of buildings, such as cities, sites, and industries.
    /// </summary>
    public abstract class BuildingEntity
    {
        #region Properties
        /// <summary>
        /// Gets the maximum number of items that can be stored.
        /// </summary>
        public int MaxCapacity { get; private set; } = 1000;

        /// <summary>
        /// Gets or sets the current capacity value.
        /// </summary>
        public double CurrentCapacity { set; get; } = 0;

        /// <summary>
        /// Gets or sets the productivity value, adjusted by the current multiplier.
        /// </summary>
        public double Productivity
        {
            get => field * GetMultiplier();
            set;
        } = 1.0;

        /// <summary>
        /// Factor that determines the production rate of the building.
        /// The actual production is calculated as Scaler * Productivity.
        /// </summary>
        public int Scaler { protected set; get; }

        /// <summary>
        /// Gets the offset value for the current production calculation.
        /// </summary>
        public int Offset { protected set; get; }

        /// <summary>
        /// Stores the coordinates of the building on map
        /// </summary>
        public SortedDictionary<(int X, int Y), Field> MapPoints { protected set; get; } = [];

        /// <summary>
        /// The width of the entity on the map, in terms of number of tiles it occupies horizontally.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the entity on the map, in terms of number of tiles it occupies vertically.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the coordinates of the top-left point of the building
        /// </summary>
        public (int X, int Y) TopLeftPoints => MapPoints.Keys.First();
        #endregion

        #region Constructors
        protected BuildingEntity(int width = 2, int height = 2)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than 0.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than 0.");

            Width = width;
            Height = height;
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// Returns the facility's consume load type.
        /// </summary>
        /// <returns>The load type.</returns>
        public abstract Load? GetConsumeLoad();

        /// <summary>
        /// Returns the facility's provided load type.
        /// </summary>
        /// <returns>The load type.</returns>
        public abstract Load GetProvideLoad();

        /// <summary>
        /// Generates building placement points starting from the specified coordinates using the provided height map.
        /// </summary>
        /// <param name="startX">The X-coordinate of the starting point for building generation. Must be within the bounds of the height map.</param>
        /// <param name="startY">The Y-coordinate of the starting point for building generation. Must be within the bounds of the height map.</param>
        /// <param name="heightMap">A two-dimensional array representing the terrain heights. The method uses this map to determine valid
        /// building locations. Cannot be <see langword="null"/>.</param>
        public abstract void GenerateBuildingPoints(int startX, int startY, int[,] heightMap);
        #endregion

        #region Virtual methods
        /// <summary>
        /// The production itself
        /// </summary>
        public virtual void Production()
        {
            if (!(CurrentCapacity < MaxCapacity)) return;

            CurrentCapacity = Math.Min(CurrentCapacity + Scaler * Productivity, MaxCapacity);
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
        /// <summary>
        /// Sets the current capacity of the entity, ensuring it does not exceed the maximum capacity or drop below zero.
        /// </summary>
        /// <param name="currentCapacity">The new current capacity value.</param>
        public void SetCurrentCapacity(int currentCapacity)
        {
            CurrentCapacity = Math.Clamp(currentCapacity, 0, MaxCapacity);
        }
        #endregion
    }

    /// <summary>
    /// Represents a city building entity that provides population resources and manages the placement of house tiles
    /// within a map.
    /// </summary>
    /// <remarks>This class is a specialized type of building entity designed to model a city within the
    /// simulation. It overrides resource provision and building point generation to reflect city-specific behavior.
    /// Instances of this class are sealed and cannot be inherited due to performance reasons.</remarks>
    public sealed class CityEntity : BuildingEntity
    {
        #region Constructors
        /// <summary>
        /// The constructor for the <see cref="CityEntity"/> class initializes a new instance of a city building entity with specified width and height.
        /// </summary>
        /// <param name="width">The width of the city entity.</param>
        /// <param name="height">The height of the city entity.</param>
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

    /// <summary>
    /// An abstract class representing a site building entity that provides raw materials and manages the placement of resource tiles
    /// within a map.
    /// </summary>
    public abstract class SiteEntity : BuildingEntity
    {
        #region Constructors
        protected SiteEntity()
        {
            Scaler = 1;
        }
        #endregion
    }

    /// <summary>
    /// Represents a lumber camp building entity that provides <see cref="LoadType.Wood"/> resources and manages the placement of lumber camp tiles
    /// within a map.
    /// </summary>
    /// <remarks>
    /// It is sealed due to performance reasons.
    /// </remarks>
    public sealed class LumberCampEntity : SiteEntity
    {
        #region Constructors
        /// <summary>
        /// The constructor for the <see cref="LumberCampEntity"/> class initializes a new instance of a lumber camp building entity.
        /// </summary>
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

    /// <summary>
    /// Represents a mine building entity that provides <see cref="LoadType.Oil"/> resources and manages the placement of mine tiles
    /// within a map.
    /// </summary>
    /// <remarks>
    /// It is sealed due to performance reasons.
    /// </remarks>
    public sealed class MineEntity : SiteEntity
    {
        #region Constructors
        /// <summary>
        /// The constructor for the <see cref="MineEntity"/> class initializes a new instance of a mine building entity.
        /// </summary>
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

    /// <summary>
    /// Represents a farm building entity that provides <see cref="LoadType.Wheat"/> resources and manages the placement of farm tiles
    /// within a map.
    /// </summary>
    /// <remarks>
    /// It is sealed due to performance reasons.
    /// </remarks>
    public sealed class FarmEntity : SiteEntity
    {
        #region Constructors
        /// <summary>
        /// The constructor for the <see cref="FarmEntity"/> class initializes a new instance of a farm building entity.
        /// </summary>
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

    /// <summary>
    /// Represents an abstract industrial building entity that manages production based on consumption capacity within a
    /// simulation or management system.
    /// </summary>
    /// <remarks>This class provides core functionality for industry-type buildings, including tracking and
    /// managing consumption and production capacities. It is intended to be used as a base class for specific industry
    /// implementations. Inheriting classes should extend or override behavior as needed to model different types of
    /// industrial entities.</remarks>
    public abstract class IndustryEntity : BuildingEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the maximum number of items that can be consumed for production.
        /// </summary>
        public int MaxConsumeCapacity { get; } = 1000;

        /// <summary>
        /// Gets or sets the current number of items that can be consumed for production.
        /// </summary>
        public double ConsumeCapacity { get; private set; } = 0;
        #endregion

        #region Constructors
        protected IndustryEntity()
        {
            Scaler = 2;
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// A method which simulates the production process of the industry entity.
        /// </summary>
        public override void Production()
        {
            if ((int)ConsumeCapacity == 0 || (int)CurrentCapacity == (int)MaxCapacity) return;

            double production = (double)Scaler * Productivity;

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

        /// <summary>
        /// Sets the consume capacity to the specified value.
        /// </summary>
        /// <param name="value">The new consume capacity value. Must be greater than or equal to 0.</param>
        public void SetConsumeCapacity(int value)
        {
            if (value >= 0)
            {
                ConsumeCapacity = value;
            }

        }
        #endregion
    }

    /// <summary>
    /// Represents a mill building entity that consumes <see cref="LoadType.Wheat"/> and provides <see cref="LoadType.Flour"/> resources and manages the placement of mill tiles
    /// within a map.
    /// </summary>
    /// <remarks>
    /// It is sealed due to performance reasons.
    /// </remarks>
    public sealed class MillEntity : IndustryEntity
    {
        #region Constructors
        /// <summary>
        /// The constructor for the <see cref="MillEntity"/> class initializes a new instance of a mill building entity.
        /// </summary>
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

    /// <summary>
    /// Represents a plant building entity that consumes <see cref="LoadType.Wood"/> and provides <see cref="LoadType.Paper"/> resources and manages the placement of plant tiles
    /// within a map.
    /// </summary>
    /// <remarks>
    /// It is sealed due to performance reasons.
    /// </remarks>
    public sealed class PlantEntity : IndustryEntity
    {
        #region Constructors
        /// <summary>
        /// The constructor for the <see cref="PlantEntity"/> class initializes a new instance of a plant building entity.
        /// </summary>
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

    /// <summary>
    /// Represents a factory building entity that consumes <see cref="LoadType.Oil"/> and provides <see cref="LoadType.Rubber"/> resources and manages the placement of mill tiles
    /// within a map.
    /// </summary>
    /// <remarks>
    /// It is sealed due to performance reasons.
    /// </remarks>
    public sealed class FactoryEntity : IndustryEntity
    {
        #region Constructors
        /// <summary>
        /// The constructor for the <see cref="FactoryEntity"/> class initializes a new instance of a factory building entity.
        /// </summary>
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
