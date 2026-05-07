using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    /// <summary>
    /// A factory class for creating instances of <see cref="IStructureGenerator"/>.
    /// This factory abstracts the instantiation logic and allows for easy swapping of different structure generator implementations without modifying the client code that uses it.
    /// </summary>
    public static class StructureGeneratorFactory
    {
        /// <summary>
        /// Creates a new instance of an object that generates map structures using the specified random provider and
        /// generation context.
        /// </summary>
        /// <param name="randomProvider">The random number provider used to control randomness during structure generation. Cannot be <see langword="null"/>.</param>
        /// <param name="context">The context containing configuration and state information for map generation. Cannot be <see langword="null"/>.</param>
        /// <returns>An object that implements <see cref="IStructureGenerator"/> and can be used to generate map structures
        /// according to the provided context.</returns>
        public static IStructureGenerator Create(IRandomProvider randomProvider, MapGenerationContext context) => new StructureGenerator(randomProvider, context);
    }

    internal class StructureGenerator : BaseStructurePlacementGenerator
    {
        #region Private fields
        private const string PluginId = "BaseGame.Structures";
        #endregion

        #region Public properties
        public override GenerationPhase Phase => GenerationPhase.Structures;
        #endregion

        #region Constructors
        internal StructureGenerator(IRandomProvider randomProvider, MapGenerationContext context) : base(randomProvider, context) { }
        #endregion

        #region Public methods
        public override List<BuildingEntity> GenerateStructures(MapGenerationContext context)
        {
            var random = _random.GetRandom(context.Seed, PluginId);
            var structures = new List<BuildingEntity>(context.Settings.MaxStructure);

            var validPoints = GetValidPointsForPlacement(context, 2, 2);

            validPoints.RemoveAll(point =>
                context.WaterMap[point.X, point.Y] ||
                context.StructureMap[point.X, point.Y] ||
                context.HeightMap[point.X, point.Y] >= 4);

            for (int i = 1; i < context.Settings.MinStructure; i += 2)
            {
                (var siteEntity, var industryEntity) = GenerateRandomEntityPair(random);
                ForcePlace(siteEntity, context, -1, -1, random, validPoints);
                (int x, int y) = siteEntity.TopLeftPoints;
                ForcePlace(industryEntity, context, x, y, random, validPoints);

                structures.Add(siteEntity);
                structures.Add(industryEntity);
            }

            // Handle odd number of structures if necessary
            if (context.Settings.MinStructure % 2 != 0)
            {
                var buildingEntity = GenerateRandomEntity(random);
                ForcePlace(buildingEntity, context, -1, -1, random, validPoints);
                structures.Add(buildingEntity);
            }

            // Generate remaining structures without pairing
            for (int i = context.Settings.MinStructure; i < context.Settings.MaxStructure; i++)
            {
                var buildingEntity = GenerateRandomEntity(random);
                TryPlace(buildingEntity, context, -1, -1, random, validPoints);
                structures.Add(buildingEntity);
            }

            return structures;
        }
        #endregion

        #region Private methods


        private (SiteEntity siteEntity, IndustryEntity industryEntity) GenerateRandomEntityPair(IRandom random)
        {
            //var pairs = new (SiteEntity siteEntity, IndustryEntity industryEntity)[]
            //{
            //    (new FarmEntity(), new MillEntity()),
            //    (new MineEntity(), new FactoryEntity()),
            //    (new LumberCampEntity(), new PlantEntity())
            //};

            //return pairs[random.Next(0, pairs.Length)];
            return random.Next(0, 3) switch
            {
                0 => (new FarmEntity(), new MillEntity()),
                1 => (new MineEntity(), new PlantEntity()),
                _ => (new LumberCampEntity(), new FactoryEntity())
            };
        }

        private BuildingEntity GenerateRandomEntity(IRandom random)
        {
            return GenerateRandomEntityPair(random) switch
            {
                (SiteEntity siteEntity, IndustryEntity industryEntity) => random.Next(0, 2) == 0 ? siteEntity : industryEntity
            };
        }
        #endregion
    }
}
