namespace TransportTycoon.MapData
{
    public abstract class Terrain : Field
    {
        #region Fields
        public int Trees { set; get; }
        public override bool Modifiable { get; protected set; }
        #endregion

        #region Public methods
        public void IncreaseHeight()
        {
            Height++;
        }

        public void DecreaseHeight()
        {
            Height--;
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
    }

    
}
