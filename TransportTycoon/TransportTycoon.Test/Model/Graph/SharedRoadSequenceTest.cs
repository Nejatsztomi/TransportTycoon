using NSubstitute;
using TransportTycoon.MapData;
using GraphNS = TransportTycoon.Model.Graph;

namespace TransportTycoon.Test.Model.Graph
{
    public class SharedRoadSequenceTest
    {
        [Fact]
        public void Constructor_SetsFieldsProperly()
        {
            // Arrange
            var field1 = Substitute.For<Field>();
            var field2 = Substitute.For<Field>();
            var fields = new List<Field> { field1, field2 };

            // Act
            var sequence = new GraphNS.SharedRoadSequence(fields);

            // Assert (using ForwardEnumerator)
            Assert.Equal(fields, [.. sequence.ForwardEnumerator()]);
            // Assert (using BackwardEnumerator)
            Assert.Equal(fields.AsEnumerable().Reverse(), sequence.BackwardEnumerator());
        }
    }
}
