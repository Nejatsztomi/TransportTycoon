using TransportTycoon.MapData;

namespace TransportTycoon.Test.MapData
{
    public class LoadAndGoodsTests
    {
        // Segédosztály a védett Value() metódus teszteléséhez
        private class TestGoods : Goods
        {
            public TestGoods(int price)
            {
                Price = price; // Beállítjuk a teszt árat
            }

            public int CalculateValue()
            {
                return Value(); // Meghívjuk a védett metódust
            }
        }

        [Fact]
        public void People_Constructor_SetsCorrectProperties()
        {
            // Act
            var people = new People();

            // Assert
            Assert.Equal(120, people.Price);
            Assert.Equal(LoadType.People, people.LoadType);
        }

        [Theory]
        [InlineData(typeof(Wheat), 150, LoadType.Wheat)]
        [InlineData(typeof(Oil), 200, LoadType.Oil)]
        [InlineData(typeof(Wood), 130, LoadType.Wood)]
        [InlineData(typeof(Flour), 400, LoadType.Flour)]
        [InlineData(typeof(Rubber), 260, LoadType.Rubber)]
        [InlineData(typeof(Paper), 300, LoadType.Paper)]
        public void Goods_Constructors_SetCorrectProperties(Type goodsType, int expectedPrice, LoadType expectedType)
        {
            // Act - Dinamikusan példányosítjuk a megadott típust
            var goods = (Goods)Activator.CreateInstance(goodsType)!;

            // Assert
            Assert.Equal(expectedPrice, goods.Price);
            Assert.Equal(expectedType, goods.LoadType);
        }

        [Fact]
        public void Goods_SetGlobalTax_UpdatesTax()
        {
            // Arrange
            int initialTax = Goods.Tax; // Mentsük el az eredeti értéket, mert a statikus változók közösek a tesztek között

            try
            {
                // Act
                Goods.SetGlobalTax(25);

                // Assert
                Assert.Equal(25, Goods.Tax);
            }
            finally
            {
                // Takarítás: visszaállítjuk az eredeti állapotot
                Goods.SetGlobalTax(initialTax);
            }
        }

        [Fact]
        public void Goods_Value_CalculatesCorrectly()
        {
            // Arrange
            int initialTax = Goods.Tax;

            try
            {
                Goods.SetGlobalTax(10); // Beállítjuk az adót 10-re
                var testGoods = new TestGoods(15); // A teszt áru ára 15

                // Act
                int resultValue = testGoods.CalculateValue();

                // Assert
                Assert.Equal(150, resultValue); // 10 (Tax) * 15 (Price) = 150
            }
            finally
            {
                // Takarítás
                Goods.SetGlobalTax(initialTax);
            }
        }
    }
}
