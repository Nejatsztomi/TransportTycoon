using NSubstitute;
using TransportTycoon.MapData;
using TransportTycoon.MapData.MapGenerator;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class PathTracerTest
    {
        [Fact]
        public void TraceSegment_ReturnsFoundIntersection_WhenStopIsReached()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 1, 0, new MapGenerationSettings());
            var table = new GameTable(mapGen, context);

            var start = new Road(0, 0, RoadType.Horizontal, 0);
            var destination = new Stop(1, 0, 0);
            table.Table[0, 0] = start;
            table.Table[1, 0] = destination;

            var tracer = new GraphNS.PathTracer(table);

            // Act
            var result = tracer.TraceSegment(start, (1, 0));

            // Assert
            Assert.Equal(GraphNS.TraceStatus.FoundIntersection, result.Status);
            Assert.Equal(destination, result.EndTile);
            Assert.Equal(2, result.PathTaken.Count);
            Assert.Equal(start, result.PathTaken[0]);
            Assert.Equal(destination, result.PathTaken[1]);
        }

        [Fact]
        public void TraceSegment_ReturnsClosedCircle_WhenPathLoopsBackToStart()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 2, 0, new MapGenerationSettings());
            var table = new GameTable(mapGen, context);

            var start = new Road(0, 0, RoadType.RightTurn, 0);
            var right = new Road(1, 0, RoadType.LeftTurn, 0);
            var bottomRight = new Road(1, 1, RoadType.UpperRightTurn, 0);
            var bottomLeft = new Road(0, 1, RoadType.UpperLeftTurn, 0);

            table.Table[0, 0] = start;
            table.Table[1, 0] = right;
            table.Table[1, 1] = bottomRight;
            table.Table[0, 1] = bottomLeft;

            var tracer = new GraphNS.PathTracer(table);

            // Act
            var result = tracer.TraceSegment(start, (1, 0));

            // Assert
            Assert.Equal(GraphNS.TraceStatus.ClosedCircle, result.Status);
            Assert.Null(result.EndTile);
            Assert.Equal(4, result.PathTaken.Count);
            Assert.Equal(start, result.PathTaken[0]);
            Assert.Equal(right, result.PathTaken[1]);
            Assert.Equal(bottomRight, result.PathTaken[2]);
            Assert.Equal(bottomLeft, result.PathTaken[3]);
        }

        [Fact]
        public void TraceSegment_ReturnsDeadEnd_WhenNextTileIsNotInfrastructure()
        {
            // Arrange
            var mapGen = Substitute.For<IMapGenerator>();
            var context = new MapGenerationContext(2, 1, 0, new MapGenerationSettings());
            var table = new GameTable(mapGen, context);

            var start = new Road(0, 0, RoadType.Horizontal, 0);
            table.Table[0, 0] = start;
            table.Table[1, 0] = new Terrain(1, 0, 0);

            var tracer = new GraphNS.PathTracer(table);

            // Act
            var result = tracer.TraceSegment(start, (1, 0));

            // Assert
            Assert.Equal(GraphNS.TraceStatus.DeadEnd, result.Status);
            Assert.Null(result.EndTile);
            Assert.Single(result.PathTaken);
            Assert.Equal(start, result.PathTaken[0]);
        }
    }
}
