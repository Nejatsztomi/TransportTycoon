using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    public class Vertex
    {
        #region Properties
        public int X { get; private set; }
        public int Y { get; private set; }
        public FieldType Type { get; private set; }
        public bool IsValidDestination => Type == FieldType.Stop;
        #endregion

        #region Constructors
        public Vertex(int x, int y, FieldType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
        #endregion
    }
}
