namespace TransportTycoon.MapData
{
    public enum FieldType
    {
        Water,
        Plain,
        Hill,
        Mountain,
        HighMountain,
        House,
        Farm,
        Mine,
        LumberCamp,
        Mill,
        Factory,
        Plant,
        Road,
        Bridge,
        Stop,
    }

    public interface IField
    {
        #region Properties
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int Height { get; protected set; }
        public FieldType FieldType { get; }

        public virtual bool Modifiable
        {
            get => true;
            //protected set
            //{
            //    field = value;
            //}
        }
        #endregion

        #region Public Methods
        public virtual int GetTrees() => 0;
        //public void ChangeType(FieldType type) => FieldType = type;
        #endregion
    }

    public struct Water : IField
    {
        #region Properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public readonly FieldType FieldType => FieldType.Water;

        public readonly bool Modifiable => false;
        #endregion

        public Water(int x, int y)
        {
            X = x;
            Y = y;
            Height = 0;
        }
    }
}
