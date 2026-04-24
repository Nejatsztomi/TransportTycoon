using NSubstitute;
using TransportTycoon.Persistence;

namespace TransportTycoon.Test.Persistence
{
    public class IPersistenceContractTest
    {
        [Fact]
        public async Task SaveGame_And_LoadGame_AreCalled()
        {
            // Arrange
            var persistence = Substitute.For<IPersistence>();
            var data = new GameSaveData();
            var uri = "testfile.json";
            persistence.LoadGame(uri).Returns(Task.FromResult<GameSaveData?>(data));

            // Act
            await persistence.SaveGame(uri, data);
            var loaded = await persistence.LoadGame(uri);

            // Assert
            await persistence.Received(1).SaveGame(uri, data);
            await persistence.Received(1).LoadGame(uri);
            Assert.Equal(data, loaded);
        }
    }
}
