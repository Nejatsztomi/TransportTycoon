namespace TransportTycoon.MapData
{
    public struct Terrain : IField
    {
        #region Fields
        public int Trees { set; get; }
        public readonly bool Modifiable => true;
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public FieldType FieldType { get; set; }
        public readonly bool IsFull => Trees == 4;
        #endregion

        public Terrain(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
            Trees = 0;
            SetFieldType();
        }


        #region Public methods
        public void IncreaseHeight()
        {
            Height++;
            SetFieldType();
        }

        public void DecreaseHeight()
        {
            Height--;
            SetFieldType();
        }

        public bool Grow()
        {
            if (IsFull) return false;
            Trees++;
            return true;
        }

        public void SpreadForest()
        {
            Trees = 1;
        }

        public readonly int GetTrees() => Trees;
        #endregion

        #region Private methods
        private void SetFieldType()
        {
            if (Height == 1) FieldType = FieldType.Plain;
            else if (Height == 2) FieldType = FieldType.Hill;
            else if (Height == 3) FieldType = FieldType.Mountain;
            else FieldType = FieldType.HighMountain;
        }
        #endregion
    }


}
