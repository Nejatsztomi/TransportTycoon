namespace TransportTycoon.MapData
{
    public class Terrain : Field
    {
        #region Fields
        public int Trees { set; get; }
        public override bool Modifiable { get; protected set; }
        #endregion

        public Terrain(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
            Trees = 0;
            SetFieldType();
            Modifiable = true;
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

        public bool IsFull => Trees == 4;

        public void SpreadForest()
        {
            Trees = 1;
        }

        public override int GetTrees() => Trees;
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
