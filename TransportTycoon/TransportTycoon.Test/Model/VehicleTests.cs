using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.MapData;
using TransportTycoon.Model;
using TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model
{
    public class VehicleTests
    {
        #region Konstruktor és Alaptulajdonságok Tesztjei
        [Fact]
        public void SmallBus_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var bus = new SmallBus(10, 20, Direction.Up);

            // Assert
            Assert.Equal(10, bus.X);
            Assert.Equal(20, bus.Y);
            Assert.Equal(Direction.Up, bus.Direction);
            Assert.Equal(1, bus.TopSpeed);
            Assert.Equal(100, bus.MaxCapacity);
            Assert.Equal(100, bus.Price);
            Assert.Equal(100, bus.Maintance);
            Assert.Equal(VehicleType.SmallBus, bus.Type);
            Assert.Single(bus.AcceptedGoods!);
            Assert.Contains(LoadType.People, bus.AcceptedGoods!);
            Assert.Equal(bus.TopSpeed, bus.CurrentSpeed);
        }

        [Fact]
        public void Truck_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var truck = new Truck(5, 5, Direction.Right);

            // Assert
            Assert.Equal(1.5, truck.TopSpeed);
            Assert.Equal(100, truck.MaxCapacity);
            Assert.Equal(VehicleType.Truck, truck.Type);
            Assert.Equal(5, truck.AcceptedGoods!.Count);
            Assert.Contains(LoadType.Wood, truck.AcceptedGoods);
            Assert.Contains(LoadType.Flour, truck.AcceptedGoods);
        }

        [Fact]
        public void LiquidTruck_Constructor_SetsCorrectDefaultValues()
        {
            // Arrange & Act
            var liquidTruck = new LiquidTruck(0, 0, Direction.Down);

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
            var van = new Van(0, 0, Direction.Up);

            // Act
            van.SetCurrentCapacity(50);

            // Assert
            Assert.Equal(50, van.CurrentCapacity);
        }

        [Fact]
        public void SetCurrentCapacity_ExceedsMaxCapacity_DoesNotChangeCapacity()
        {
            // Arrange
            var van = new Van(0, 0, Direction.Up);
            van.SetCurrentCapacity(50); // Beállítunk egy érvényes 50-es értéket

            // Act
            van.SetCurrentCapacity(150); // Érvénytelen érték, mert MaxCapacity = 100

            // Assert
            Assert.Equal(50, van.CurrentCapacity); // Az érték nem változhatott
        }

        [Fact]
        public void SetCurrentCapacity_BelowZero_DoesNotChangeCapacity()
        {
            // Arrange
            var van = new Van(0, 0, Direction.Up);
            van.SetCurrentCapacity(50); // Beállítunk egy érvényes 50-es értéket

            // Act
            van.SetCurrentCapacity(-20); // Érvénytelen (negatív) érték

            // Assert
            Assert.Equal(50, van.CurrentCapacity); // Az érték nem változhatott
        }

        [Fact]
        public void SetCurrentLoad_UpdatesLoadCorrectly()
        {
            // Arrange
            var pickup = new Pickup(0, 0, Direction.Up);
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
            var bus = new BigBus(0, 0, Direction.Up);
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
            var bus = new SmallBus(0, 0, Direction.Up);

            // Act
            bus.ChangeCurrentSpeed(5.0);

            // Assert
            Assert.Equal(1.0, bus.CurrentSpeed);
        }
        [Fact]
        public void ChangeCurrentSpeed_BelowZero_DoesNotChangeSpeed()
        {
            // Arrange
            var bus = new SmallBus(0, 0, Direction.Up);
            double initialSpeed = bus.CurrentSpeed; // A konstruktor 1.0-ra állítja

            // Act
            bus.ChangeCurrentSpeed(-1.0); // Érvénytelen érték, a metódus nem csinál semmit

            // Assert
            Assert.Equal(initialSpeed, bus.CurrentSpeed); // Marad 1.0
        }
        #endregion

        #region Útvonal (Prouth) Tesztek
        [Fact]
        public void StartDriving_WithValidEdges_SetsCurrentRoute()
        {
            // Arrange
            var van = new Van(0, 0, Direction.Up);

            // Javított Edge és Node konstruktor hívás az Edge.cs és Node.cs alapján
            var startNode = new Node(0, 0, typeof(Stop));
            var endNode = new Node(0, 1, typeof(Stop));
            var dummyRoads = new List<IField>(); // Üres lista az IField-ekhez

            var edges = new List<Edge>
            {
                new Edge(startNode, endNode, dummyRoads, 10.0) // Node start, Node end, IEnumerable<IField> roads, double cost
            };

            // Act
            van.StartDriving(edges);

            // Assert
            Assert.NotNull(van.CurrentRoute);
            Assert.Single(van.CurrentRoute);
        }
        #endregion
    }
}
