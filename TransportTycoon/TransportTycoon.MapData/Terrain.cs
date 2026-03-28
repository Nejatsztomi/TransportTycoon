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
            Modifiable = true;
        }


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
