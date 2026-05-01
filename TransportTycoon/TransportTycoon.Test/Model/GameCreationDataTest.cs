using TransportTycoon.MapData.MapGenerator;
using TransportTycoon.Model;

namespace TransportTycoon.Test.Model
{
    public class GameCreationDataTest
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            var context = new MapGenerationContext(10, 20, 123, new MapGenerationSettings());
            var saveName = "TestSave";
            var difficulty = Difficulty.Hard;
            var balance = 5000;

            var data = new GameCreationData(context, saveName, difficulty, balance);

            Assert.Equal(saveName, data.SaveName);
            Assert.Equal(difficulty, data.Difficulty);
            Assert.Equal(balance, data.Balance);
            Assert.Equal(context, data.MapGenerationContext);
        }

        [Fact]
        public void Constructor_ThrowsOnInvalidSaveName()
        {
            var context = new MapGenerationContext();
            Assert.Throws<ArgumentException>(() => new GameCreationData(context, " "));
            Assert.Throws<ArgumentException>(() => new GameCreationData(context, ""));
        }

        [Fact]
        public void Constructor_ThrowsOnNegativeBalance()
        {
            var context = new MapGenerationContext();
            Assert.Throws<ArgumentException>(() => new GameCreationData(context, "ValidName", Difficulty.Easy, -1));
        }

        [Fact]
        public void DefaultConstructor_SetsDefaults()
        {
            var data = new GameCreationData();
            Assert.False(string.IsNullOrWhiteSpace(data.SaveName));
            Assert.Equal(GameModel.DefaultDifficulty, data.Difficulty);
            Assert.Equal(GameModel.DefaultBalance, data.Balance);
        }
    }
}
