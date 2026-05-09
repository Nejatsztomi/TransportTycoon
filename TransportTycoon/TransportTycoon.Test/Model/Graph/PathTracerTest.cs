using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class PathTracerTest
    {
        public static TheoryData<RoadType, (int dx, int dy)> AllowedExitDirections =>
            new()
            {
                {RoadType.Vertical, (0, -1)},
                {RoadType.Vertical, (0, 1)},
                {RoadType.Horizontal, (1, 0)},
                {RoadType.Horizontal, (-1, 0)},
                {RoadType.UpperLeftTurn, (0, -1)},
                {RoadType.UpperLeftTurn, (-1, 0)},
                {RoadType.LeftTurn, (0, 1)},
                {RoadType.LeftTurn, (-1, 0)},
                {RoadType.RightTurn, (0, 1)},
                {RoadType.RightTurn, (1, 0)},
                {RoadType.UpperRightTurn, (0, -1)},
                {RoadType.UpperRightTurn, (1, 0)},
                {RoadType.DownTRoad, (-1, 0)},
                {RoadType.DownTRoad, (0, 1)},
                {RoadType.DownTRoad, (1, 0)},
                {RoadType.UpperTRoad, (-1, 0)},
                {RoadType.UpperTRoad, (0, -1)},
                {RoadType.UpperTRoad, (1, 0)},
                {RoadType.LeftTRoad, (0, -1)},
                {RoadType.LeftTRoad, (0, 1)},
                {RoadType.LeftTRoad, (-1, 0)},
                {RoadType.RightTRoad, (0, -1)},
                {RoadType.RightTRoad, (0, 1)},
                {RoadType.RightTRoad, (1, 0)},
                {RoadType.XRoad, (0, -1)},
                {RoadType.XRoad, (0, 1)},
                {RoadType.XRoad, (-1, 0)},
                {RoadType.XRoad, (1, 0)},
            };

        public static TheoryData<RoadType, (int dx, int dy)> BlockedExitDirections =>
            new() {
                {RoadType.Vertical, (1, 0)},
                {RoadType.Horizontal, (0, -1)},
                {RoadType.UpperLeftTurn, (0, 1)},
                {RoadType.LeftTurn, (0, -1)},
                {RoadType.RightTurn, (-1, 0)},
                {RoadType.UpperRightTurn, (0, 1)},
                {RoadType.DownTRoad, (0, -1)},
                {RoadType.UpperTRoad, (0, 1)},
                {RoadType.LeftTRoad, (1, 0)},
                { RoadType.RightTRoad, (-1, 0)},
            };

        public static TheoryData<BridgeType, (int dx, int dy)> VerticalBridgeDirections =>
            new()
            {
                {BridgeType.VerticalGreenBridge, (0, -1)},
                {BridgeType.VerticalGreenBridge, (0, 1)},
                {BridgeType.VerticalYellowBridge, (0, -1)},
                {BridgeType.VerticalYellowBridge, (0, 1)},
                {BridgeType.VerticalRedBridge, (0, -1)},
                {BridgeType.VerticalRedBridge, (0, 1)},
            };

        public static TheoryData<BridgeType, (int dx, int dy)> HorizontalBridgeDirections =>
            new()
            {
                {BridgeType.HorizontalGreenBridge, (1, 0)},
                {BridgeType.HorizontalGreenBridge, (-1, 0)},
                {BridgeType.HorizontalYellowBridge, (1, 0)},
                {BridgeType.HorizontalYellowBridge, (-1, 0)},
                {BridgeType.HorizontalRedBridge, (1, 0)},
                {BridgeType.HorizontalRedBridge, (-1, 0)},
            };

        [Theory]
        [MemberData(nameof(AllowedExitDirections))]
        public void TraceSegment_AllowsMovement_WhenRoadHasMatchingExit(RoadType roadType, (int dx, int dy) momentum)
        {
            var table = CreateTable(3, 3);
            var start = new Road(1, 1, roadType, 0);
            var destination = new Stop(1 + momentum.dx, 1 + momentum.dy, 0);
            table.Table[1, 1] = start;
            table.Table[destination.X, destination.Y] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, momentum);

            Assert.Equal(GraphNS.TraceStatus.FoundIntersection, result.Status);
            Assert.Equal(destination, result.EndTile);
            Assert.Equal(2, result.PathTaken.Count);
            Assert.Equal(start, result.PathTaken[0]);
            Assert.Equal(destination, result.PathTaken[1]);
        }

        [Theory]
        [MemberData(nameof(BlockedExitDirections))]
        public void TraceSegment_ReturnsDeadEnd_WhenRoadDoesNotHaveMatchingExit(RoadType roadType, (int dx, int dy) momentum)
        {
            var table = CreateTable(3, 3);
            var start = new Road(1, 1, roadType, 0);
            var destination = new Stop(1 + momentum.dx, 1 + momentum.dy, 0);
            table.Table[1, 1] = start;
            table.Table[destination.X, destination.Y] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, momentum);

            Assert.Equal(GraphNS.TraceStatus.DeadEnd, result.Status);
            Assert.Null(result.EndTile);
            Assert.Single(result.PathTaken);
            Assert.Equal(start, result.PathTaken[0]);
        }

        [Fact]
        public void TraceSegment_ReturnsDeadEnd_WhenHeightDifferenceIsGreaterThanOne()
        {
            var table = CreateTable(3, 3);
            var start = new Road(1, 1, RoadType.Horizontal, 0);
            var destination = new Road(2, 1, RoadType.Horizontal, 2);
            table.Table[1, 1] = start;
            table.Table[2, 1] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.DeadEnd, result.Status);
            Assert.Null(result.EndTile);
            Assert.Single(result.PathTaken);
        }

        [Fact]
        public void TraceSegment_AccumulatesForwardAndBackwardCost_WhenRoadHeightsIncreaseByOne()
        {
            var table = CreateTable(3, 1);
            var start = new Road(0, 0, RoadType.Horizontal, 0);
            var middle = new Road(1, 0, RoadType.Horizontal, 1);
            var destination = new Stop(2, 0, 2);

            table.Table[0, 0] = start;
            table.Table[1, 0] = middle;
            table.Table[2, 0] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.FoundIntersection, result.Status);
            Assert.Equal(destination, result.EndTile);
            Assert.Equal(3, result.PathTaken.Count);
            Assert.Equal(4.0, result.ForwardCost);
            Assert.Equal(1.6, result.BackwardCost);
        }

        [Fact]
        public void TraceSegment_AccumulatesForwardAndBackwardCost_WhenRoadHeightsDecreaseByOne()
        {
            var table = CreateTable(3, 1);
            var start = new Road(0, 0, RoadType.Horizontal, 2);
            var middle = new Road(1, 0, RoadType.Horizontal, 1);
            var destination = new Stop(2, 0, 0);

            table.Table[0, 0] = start;
            table.Table[1, 0] = middle;
            table.Table[2, 0] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.FoundIntersection, result.Status);
            Assert.Equal(destination, result.EndTile);
            Assert.Equal(3, result.PathTaken.Count);
            Assert.Equal(1.6, result.ForwardCost);
            Assert.Equal(4.0, result.BackwardCost);
        }

        [Fact]
        public void TraceSegment_AllowsMovement_WhenNextTileIsStop()
        {
            var table = CreateTable(3, 3);
            var start = new Road(1, 1, RoadType.Horizontal, 0);
            var destination = new Stop(2, 1, 0);
            table.Table[1, 1] = start;
            table.Table[2, 1] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.FoundIntersection, result.Status);
            Assert.Equal(destination, result.EndTile);
        }

        [Fact]
        public void TraceSegment_AllowsMovement_WhenNextTileIsBridge()
        {
            var table = CreateTable(4, 3);
            var start = new Road(1, 1, RoadType.Horizontal, 0);
            var destination = new GreenBridge(2, 1, BridgeType.HorizontalGreenBridge, 0);
            table.Table[1, 1] = start;
            table.Table[2, 1] = destination;
            table.Table[3, 1] = new Stop(3, 1, 0);
            table.Table[2, 0] = new Terrain(2, 0, 0);
            table.Table[2, 2] = new Terrain(2, 2, 0);

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.FoundIntersection, result.Status);
            Assert.Equal(3, result.EndTile!.X);
            Assert.Equal(1, result.EndTile.Y);
            Assert.Contains(destination, result.PathTaken);
        }

        [Theory]
        [MemberData(nameof(VerticalBridgeDirections))]
        public void TraceSegment_AllowsMovement_WhenVerticalBridgeHasMatchingDirection(BridgeType bridgeType, (int dx, int dy) momentum)
        {
            var table = CreateTable(1, 5);
            var start = new Road(0, 2, RoadType.Vertical, 0);
            var bridge = CreateBridge(bridgeType, 0, 2 + momentum.dy, 0);
            var destination = new Stop(0, 2 + (momentum.dy * 2), 0);

            table.Table[0, 2] = start;
            table.Table[bridge.X, bridge.Y] = bridge;
            table.Table[destination.X, destination.Y] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, momentum);

            Assert.Equal(GraphNS.TraceStatus.FoundIntersection, result.Status);
            Assert.Equal(destination, result.EndTile);
            Assert.Equal(3, result.PathTaken.Count);
            Assert.Equal(start, result.PathTaken[0]);
            Assert.Equal(bridge, result.PathTaken[1]);
            Assert.Equal(destination, result.PathTaken[2]);
        }

        [Theory]
        [MemberData(nameof(HorizontalBridgeDirections))]
        public void TraceSegment_AllowsMovement_WhenHorizontalBridgeHasMatchingDirection(BridgeType bridgeType, (int dx, int dy) momentum)
        {
            var table = CreateTable(5, 1);
            var start = new Road(2, 0, RoadType.Horizontal, 0);
            var bridge = CreateBridge(bridgeType, 2 + momentum.dx, 0, 0);
            var destination = new Stop(2 + (momentum.dx * 2), 0, 0);

            table.Table[2, 0] = start;
            table.Table[bridge.X, bridge.Y] = bridge;
            table.Table[destination.X, destination.Y] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, momentum);

            Assert.Equal(GraphNS.TraceStatus.FoundIntersection, result.Status);
            Assert.Equal(destination, result.EndTile);
            Assert.Equal(3, result.PathTaken.Count);
            Assert.Equal(start, result.PathTaken[0]);
            Assert.Equal(bridge, result.PathTaken[1]);
            Assert.Equal(destination, result.PathTaken[2]);
        }

        [Fact]
        public void TraceSegment_ReturnsDeadEnd_WhenNextTileIsNotInfrastructure()
        {
            var table = CreateTable(3, 3);
            var start = new Road(1, 1, RoadType.Horizontal, 0);
            var destination = new Terrain(2, 1, 0);
            table.Table[1, 1] = start;
            table.Table[2, 1] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.DeadEnd, result.Status);
            Assert.Null(result.EndTile);
            Assert.Single(result.PathTaken);
        }

        [Fact]
        public void TraceSegment_ReturnsClosedCircle_WhenPathLoopsBackToStart()
        {
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 2, 0, new MapGenerationSettings());
            var table = new GameTable(mapGen, context);

            var start = new Road(0, 0, RoadType.RightTurn, 0);
            var right = new Road(1, 0, RoadType.LeftTurn, 0);
            var bottomRight = new Road(1, 1, RoadType.UpperLeftTurn, 0);
            var bottomLeft = new Road(0, 1, RoadType.UpperRightTurn, 0);

            table.Table[0, 0] = start;
            table.Table[1, 0] = right;
            table.Table[1, 1] = bottomRight;
            table.Table[0, 1] = bottomLeft;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.ClosedCircle, result.Status);
            Assert.Null(result.EndTile);
            Assert.Equal(4, result.PathTaken.Count);
            Assert.Equal(start, result.PathTaken[0]);
            Assert.Equal(right, result.PathTaken[1]);
            Assert.Equal(bottomRight, result.PathTaken[2]);
            Assert.Equal(bottomLeft, result.PathTaken[3]);
        }

        [Fact]
        public void TraceSegment_ReturnsDeadEnd_WhenStartTileIsNotInfrastructure()
        {
            var table = CreateTable(3, 3);
            var start = new Terrain(1, 1, 0); // Terrain hits the _ => false case
            var destination = new Road(2, 1, RoadType.Horizontal, 0);
            table.Table[1, 1] = start;
            table.Table[2, 1] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.DeadEnd, result.Status);
            Assert.Null(result.EndTile);
        }

        [Fact]
        public void TraceSegment_ReturnsDeadEnd_WhenRoadTypeIsInvalid()
        {
            var table = CreateTable(3, 3);
            // 254 is an invalid enum value, forcing the switch to the _ => false discard
            var start = new Road(1, 1, (RoadType)254, 0);
            var destination = new Stop(2, 1, 0);
            table.Table[1, 1] = start;
            table.Table[2, 1] = destination;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.DeadEnd, result.Status);
        }

        [Fact]
        public void TraceSegment_ReturnsDeadEnd_WhenApproachingVerticalBridgeHorizontally()
        {
            var table = CreateTable(3, 1);
            var start = new Road(0, 0, RoadType.Horizontal, 0);
            // Bridge is vertical, but momentum is horizontal (1, 0)
            var bridge = CreateBridge(BridgeType.VerticalGreenBridge, 1, 0, 0);

            table.Table[0, 0] = start;
            table.Table[1, 0] = bridge;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (1, 0));

            Assert.Equal(GraphNS.TraceStatus.DeadEnd, result.Status);
        }

        [Fact]
        public void TraceSegment_ReturnsDeadEnd_WhenApproachingHorizontalBridgeVertically()
        {
            var table = CreateTable(1, 3);
            var start = new Road(0, 0, RoadType.Vertical, 0);
            // Bridge is horizontal, but momentum is vertical (0, 1)
            var bridge = CreateBridge(BridgeType.HorizontalGreenBridge, 0, 1, 0);

            table.Table[0, 0] = start;
            table.Table[0, 1] = bridge;

            var tracer = new GraphNS.PathTracer(table);
            var result = tracer.TraceSegment(start, (0, 1));

            Assert.Equal(GraphNS.TraceStatus.DeadEnd, result.Status);
        }

        private static GameTable CreateTable(int width, int height)
        {
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(width, height, 0, new MapGenerationSettings());
            return new GameTable(mapGen, context);
        }

        private static Field CreateBridge(BridgeType bridgeType, int x, int y, int height)
        {
            return bridgeType switch
            {
                BridgeType.VerticalGreenBridge => new GreenBridge(x, y, bridgeType, height),
                BridgeType.VerticalYellowBridge => new YellowBridge(x, y, bridgeType, height),
                BridgeType.VerticalRedBridge => new RedBridge(x, y, bridgeType, height),
                BridgeType.HorizontalGreenBridge => new GreenBridge(x, y, bridgeType, height),
                BridgeType.HorizontalYellowBridge => new YellowBridge(x, y, bridgeType, height),
                BridgeType.HorizontalRedBridge => new RedBridge(x, y, bridgeType, height),
                _ => throw new ArgumentOutOfRangeException(nameof(bridgeType))
            };
        }
    }
}
