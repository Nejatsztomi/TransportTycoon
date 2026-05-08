using NSubstitute;
using System.Reflection;
using System.Runtime.CompilerServices;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;

namespace TransportTycoon.Test.Model
{
    public class StopAdvancedTests
    {
        // Segédmetódus, ami konstruktor hívása NÉLKÜL hoz létre egy valós objektumot,
        // majd Reflection segítségével erőszakosan beállítja a védett property-ket.
        private T CreateRealEntity<T>(int currentCapacity, int maxCapacity) where T : BuildingEntity
        {
            var entity = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));

            SetPrivateFieldOrProperty(entity, "CurrentCapacity", currentCapacity);
            SetPrivateFieldOrProperty(entity, "MaxCapacity", maxCapacity);

            return entity;
        }

        // Kikerüli az "inaccessible set accessor" hibát azáltal, hogy a háttérváltozót írja felül
        private void SetPrivateFieldOrProperty(object target, string name, object value)
        {
            var type = target.GetType();
            while (type != null)
            {
                // Próbáljuk property-n keresztül (ha van elérhető setter)
                var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(target, value);
                    return;
                }

                // Ha védett/privát a setter, a C# egy <PropertyNév>k__BackingField nevű rejtett mezőt generál. Ezt írjuk felül!
                var field = type.GetField($"<{name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(target, value);
                    return;
                }

                type = type.BaseType;
            }
        }

        #region ShowWhatTheBuildingsCanGet (Mit fogad a megálló)
        [Fact]
        public void ShowWhatTheBuildingsCanGet_EmptyConnections_ReturnsEmptyList()
        {
            var stop = new Stop(0, 0, 1);
            var result = stop.ShowWhatTheBuildingsCanGet([LoadType.Wood]);
            Assert.Empty(result);
        }

        [Fact]
        public void ShowWhatTheBuildingsCanGet_CityEntity_AlwaysAdded()
        {
            var stop = new Stop(0, 0, 1);
            var mockBlock = Substitute.For<IBuildingBlocks>();
            // A város mindent kérdés nélkül elfogad
            mockBlock.BuildingEntity.Returns(CreateRealEntity<CityEntity>(0, 100));
            stop.SetBuildingBlocks(mockBlock);

            var result = stop.ShowWhatTheBuildingsCanGet([LoadType.Wood]);

            Assert.Single(result);
            Assert.Contains(mockBlock, result);
        }

        [Fact]
        public void ShowWhatTheBuildingsCanGet_NormalEntity_HasCapacityButNotAcceptedLoad_NotAdded()
        {
            var stop = new Stop(0, 0, 1);
            var mockBlock = Substitute.For<IBuildingBlocks>();

            // Van hely (50/100)
            var realEntity = CreateRealEntity<MillEntity>(50, 100);
            mockBlock.BuildingEntity.Returns(realEntity);
            stop.SetBuildingBlocks(mockBlock);

            // A járművünk olyan árut hoz, amit a malom NEM kér (pl. Olajat)
            var result = stop.ShowWhatTheBuildingsCanGet([LoadType.Oil]);

            Assert.Empty(result); // Nem fogadja be
        }

        [Fact]
        public void ShowWhatTheBuildingsCanGet_NormalEntity_HasCapacityAndAcceptedLoad_Added()
        {
            var stop = new Stop(0, 0, 1);
            var mockBlock = Substitute.For<IBuildingBlocks>();

            // Van hely (50/100)
            var realEntity = CreateRealEntity<MillEntity>(50, 100);
            mockBlock.BuildingEntity.Returns(realEntity);
            stop.SetBuildingBlocks(mockBlock);

            var expectedLoad = realEntity.GetConsumeLoad()?.LoadType ?? LoadType.Wheat;

            // A jármű pont azt hozza, amit a gyár kér!
            var result = stop.ShowWhatTheBuildingsCanGet([expectedLoad]);

            Assert.Single(result);
            Assert.Contains(mockBlock, result);
        }
        #endregion

        #region ShowWhatTheBuildingsCanGive (Mit ad a megálló)
        [Fact]
        public void ShowWhatTheBuildingsCanGive_CityEntity_AlwaysAdded()
        {
            var stop = new Stop(0, 0, 1);
            var mockBlock = Substitute.For<IBuildingBlocks>();
            mockBlock.BuildingEntity.Returns(CreateRealEntity<CityEntity>(50, 100));
            stop.SetBuildingBlocks(mockBlock);

            var result = stop.ShowWhatTheBuildingsCanGive([LoadType.People]);

            Assert.Single(result);
            Assert.Contains(mockBlock, result);
        }

        [Fact]
        public void ShowWhatTheBuildingsCanGive_NormalEntity_HasCapacityButVehicleDoesNotAccept_NotAdded()
        {
            var stop = new Stop(0, 0, 1);
            var mockBlock = Substitute.For<IBuildingBlocks>();

            // Van készlete a Fatelepen (50/100)
            var realEntity = CreateRealEntity<LumberCampEntity>(50, 100);
            mockBlock.BuildingEntity.Returns(realEntity);
            stop.SetBuildingBlocks(mockBlock);

            // A járművünk csak Búzát bír elvinni
            var result = stop.ShowWhatTheBuildingsCanGive([LoadType.Wheat]);

            Assert.Empty(result); // Nem tudjuk elvinni
        }

        [Fact]
        public void ShowWhatTheBuildingsCanGive_NormalEntity_HasCapacityAndVehicleAccepts_Added()
        {
            var stop = new Stop(0, 0, 1);
            var mockBlock = Substitute.For<IBuildingBlocks>();

            // Van készlet
            var realEntity = CreateRealEntity<LumberCampEntity>(50, 100);
            mockBlock.BuildingEntity.Returns(realEntity);
            stop.SetBuildingBlocks(mockBlock);

            // Lekérjük, mit ad a Fatelep, és a jármű pont azt képes elvinni
            var providedLoad = realEntity.GetProvideLoad()?.LoadType ?? LoadType.Wood;

            var result = stop.ShowWhatTheBuildingsCanGive([providedLoad]);

            Assert.Single(result);
            Assert.Contains(mockBlock, result);
        }
        #endregion

        #region IInfrastructure alap metódusok (Place, Remove)
        [Fact]
        public void IInfrastructure_PlaceAndRemove_ExecuteWithoutExceptions()
        {
            IInfrastructure infrastructure = new Stop(0, 0, 1);

            var exceptionPlace = Record.Exception(() => infrastructure.Place());
            var exceptionRemove = Record.Exception(() => infrastructure.Remove());

            Assert.Null(exceptionPlace);
            Assert.Null(exceptionRemove);
        }
        #endregion
    }
}
