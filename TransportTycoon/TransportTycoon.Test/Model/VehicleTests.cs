using TransportTycoon.MapData;
using TransportTycoon.Model;

namespace TransportTycoon.Test.Model
{
    public class VehicleTests
    {
        #region Konstruktor és Alaptulajdonságok Tesztjei
        [Fact]
        public void SmallBus_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var bus = new SmallBus(10, 20, 270, null); // Up = 270

            // Assert
            Assert.Equal(10, bus.X);
            Assert.Equal(20, bus.Y);
            Assert.Equal(270, bus.Angle);
            Assert.Equal(1, bus.TopSpeed);
            Assert.Equal(10, bus.MaxCapacity);
            Assert.Equal(500, bus.Price);
            Assert.Equal(2, bus.Maintenance);
            Assert.Equal(VehicleType.SmallBus, bus.Type);
            Assert.Single(bus.AcceptedGoods!);
            Assert.Contains(LoadType.People, bus.AcceptedGoods!);
            Assert.Equal(bus.TopSpeed, bus.CurrentSpeed);
        }

        [Fact]
        public void Truck_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var truck = new Truck(5, 5, 0, null); // Right = 0

            // Assert
            Assert.Equal(0.9, truck.TopSpeed);
            Assert.Equal(20, truck.MaxCapacity);
            Assert.Equal(VehicleType.Truck, truck.Type);
            Assert.Equal(5, truck.AcceptedGoods!.Count);
            Assert.Contains(LoadType.Wood, truck.AcceptedGoods);
            Assert.Contains(LoadType.Flour, truck.AcceptedGoods);
        }

        [Fact]
        public void LiquidTruck_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var liquidTruck = new LiquidTruck(0, 0, 90, null); // Down = 90

            // Assert
            Assert.Equal(0.9, liquidTruck.TopSpeed);
            Assert.Equal(VehicleType.LiquidTruck, liquidTruck.Type);
            Assert.Single(liquidTruck.AcceptedGoods!);
            Assert.Contains(LoadType.Oil, liquidTruck.AcceptedGoods!);
        }
        #endregion

        #region Kapacitás és Rakomány Tesztek
        [Fact]
        public void SetCurrentCapacity_ValidAmount_UpdatesCapacity()
        {
            // Arrange
            var van = new Van(0, 0, 270, null); // Up = 270

            // Act
            van.SetCurrentCapacity(10);

            // Assert
            Assert.Equal(10, van.CurrentCapacity);
        }

        [Fact]
        public void SetCurrentCapacity_ExceedsMaxCapacity_DoesNotChangeCapacity()
        {
            // Arrange
            var van = new Van(0, 0, Direction.Up);
            van.SetCurrentCapacity(10); // Beállítunk egy érvényes 50-es értéket

            // Act
            van.SetCurrentCapacity(150); // Érvénytelen érték, mert MaxCapacity = 100

            // Assert
            Assert.Equal(10, van.CurrentCapacity); // Az érték nem változhatott
        }

        [Fact]
        public void SetCurrentCapacity_BelowZero_DoesNotChangeCapacity()
        {
            // Arrange
            var van = new Van(0, 0, Direction.Up);
            van.SetCurrentCapacity(10); // Beállítunk egy érvényes 50-es értéket

            // Act
            van.SetCurrentCapacity(-20); // Érvénytelen (negatív) érték

            // Assert
            Assert.Equal(10, van.CurrentCapacity); // Az érték nem változhatott
        }

        [Fact]
        public void SetCurrentLoad_UpdatesLoadCorrectly()
        {
            // Arrange
            var pickup = new Pickup(0, 0, 270, null); // Up = 270
            var wood = new Wood();

            // Act
            pickup.SetCurrentLoad(wood);

            // Assert
            Assert.Equal(wood, pickup.CurrentLoad);
        }
        #endregion

        #region Sebesség Tesztek
        [Fact]
        public void ChangeCurrentSpeed_ValidSpeed_UpdatesSpeed()
        {
            // Arrange
            var bus = new BigBus(0, 0, 270, null); // Up = 270
            double validSpeed = bus.TopSpeed / 2;

            // Act
            bus.ChangeCurrentSpeed(validSpeed);

            // Assert
            Assert.Equal(validSpeed, bus.CurrentSpeed);
        }

        [Fact]
        public void ChangeCurrentSpeed_ExceedsTopSpeed_CapsAtTopSpeed()
        {
            // Arrange
            var bus = new SmallBus(0, 0, 270, null); // Up = 270

            // Act
            bus.ChangeCurrentSpeed(5.0);

            // Assert
            Assert.Equal(1.0, bus.CurrentSpeed);
        }
        [Fact]
        public void ChangeCurrentSpeed_BelowZero_DoesNotChangeSpeed()
        {
            // Arrange
            var bus = new SmallBus(0, 0, 270, null); // Up = 270

            // Act
            bus.ChangeCurrentSpeed(-1.0);

            // Assert
            Assert.Equal(0.0, bus.CurrentSpeed); // Math.Clamp sets to 0.0
        }
        #endregion

        #region További Tesztjei
        [Fact]
        public void Vehicle_Ids_AreUnique()
        {
            // Arrange & Act
            var vehicle1 = new SmallBus(0, 0, 0, null);
            var vehicle2 = new Truck(0, 0, 0, null);

            // Assert
            Assert.NotEqual(vehicle1.Id, vehicle2.Id);
        }

        [Fact]
        public void SetCurrentCapacity_ToZero_ClearsCurrentLoad()
        {
            // Arrange
            var van = new Van(0, 0, 0, null);
            var wood = new Wood();
            van.SetCurrentLoad(wood);
            van.SetCurrentCapacity(50);

            // Act
            van.SetCurrentCapacity(0);

            // Assert
            Assert.Equal(0, van.CurrentCapacity);
            Assert.Null(van.CurrentLoad);
        }

        [Fact]
        public void SetCurrentLoad_ToNull_ClearsCurrentCapacity()
        {
            // Arrange
            var pickup = new Pickup(0, 0, 0, null);
            pickup.SetCurrentCapacity(50);

            // Act
            pickup.SetCurrentLoad(null);

            // Assert
            Assert.Null(pickup.CurrentLoad);
            Assert.Equal(0, pickup.CurrentCapacity);
        }

        [Fact]
        public void SetCurrentLoad_InvalidLoadType_DoesNotSetLoad()
        {
            // Arrange
            var bus = new SmallBus(0, 0, 0, null);
            var wood = new Wood(); // SmallBus only accepts People

            // Act
            bus.SetCurrentLoad(wood);

            // Assert
            Assert.Null(bus.CurrentLoad);
        }

        [Fact]
        public void SetCurrentLoad_ValidLoadType_SetsLoadWithoutChangingCapacity()
        {
            // Arrange
            var truck = new Truck(0, 0, 0, null);
            var wood = new Wood();
            truck.SetCurrentCapacity(50);

            // Act
            truck.SetCurrentLoad(wood);

            // Assert
            Assert.Equal(wood, truck.CurrentLoad);
            Assert.Equal(50, truck.CurrentCapacity);
        }

        [Fact]
        public void MapX_MapY_RoundCoordinatesCorrectly()
        {
            // Arrange
            var van = new Van(1, 3, 0, null);

            // Assert
            Assert.Equal(1, van.MapX);
            Assert.Equal(3, van.MapY);
        }

        [Fact]
        public void Vehicle_IsLost_InitiallyFalse()
        {
            // Arrange
            var vehicle = new Truck(0, 0, 0, null);

            // Assert
            Assert.False(vehicle.IsLost);
        }

        [Fact]
        public void Van_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var van = new Van(5, 10, 180, null); // Left = 180

            // Assert
            Assert.Equal(5, van.X);
            Assert.Equal(10, van.Y);
            Assert.Equal(180, van.Angle);
            Assert.Equal(0.9, van.TopSpeed);
            Assert.Equal(100, van.MaxCapacity);
            Assert.Equal(100, van.Price);
            Assert.Equal(100, van.Maintenance);
            Assert.Equal(VehicleType.Van, van.Type);
            Assert.Equal(5, van.AcceptedGoods!.Count);
            Assert.Contains(LoadType.Wood, van.AcceptedGoods);
            Assert.Equal(van.TopSpeed, van.CurrentSpeed);
        }

        [Fact]
        public void Pickup_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var pickup = new Pickup(0, 0, 90, null); // Down = 90

            // Assert
            Assert.Equal(0.9, pickup.TopSpeed);
            Assert.Equal(100, pickup.MaxCapacity);
            Assert.Equal(VehicleType.Pickup, pickup.Type);
            Assert.Equal(5, pickup.AcceptedGoods!.Count);
            Assert.Contains(LoadType.Flour, pickup.AcceptedGoods);
        }

        [Fact]
        public void BigBus_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var bus = new BigBus(15, 25, 0, null); // Right = 0

            // Assert
            Assert.Equal(15, bus.X);
            Assert.Equal(25, bus.Y);
            Assert.Equal(0, bus.Angle);
            Assert.Equal(1, bus.TopSpeed);
            Assert.Equal(100, bus.MaxCapacity);
            Assert.Equal(100, bus.Price);
            Assert.Equal(100, bus.Maintenance);
            Assert.Equal(VehicleType.BigBus, bus.Type);
            Assert.Single(bus.AcceptedGoods!);
            Assert.Contains(LoadType.People, bus.AcceptedGoods!);
            Assert.Equal(bus.TopSpeed, bus.CurrentSpeed);
        }
        #endregion
    }
}
