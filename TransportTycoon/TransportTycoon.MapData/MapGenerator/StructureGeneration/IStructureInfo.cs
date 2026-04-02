namespace TransportTycoon.MapData.MapGenerator.StructureGeneration
{
    public interface IStructureInfo
    {
        #region Properties
        public int Width { get; }
        public int Height { get; }
        #endregion
    }

    public class StructureInfo : IStructureInfo
    {
        #region Structures
        public static readonly StructureInfo Small = new(2, 2);
        public static readonly StructureInfo City = new(5, 5);
        #endregion

        #region Properties
        public int Width { get; }
        public int Height { get; }
        #endregion
        #region Constructors
        private StructureInfo(int width, int height)
        {
            Width = width;
            Height = height;
        }
        #endregion
    }
}
