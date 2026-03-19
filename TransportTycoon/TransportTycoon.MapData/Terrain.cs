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

    public class Plain : Terrain
    {
        #region Constructors
        public Plain(int x, int y)
        {
            X = x;
            Y = y;
            Height = 1;
            Trees = 0;
            FieldType = FieldType.Plain;
        }
        #endregion
    }

    public class Hill : Terrain
    {
        #region Constructors
        public Hill(int x, int y)
        {
            X = x;
            Y = y;
            Height = 2;
            Trees = 0;
            FieldType = FieldType.Hill;
        }
        #endregion
    }

    public class Mountain : Terrain
    {
        #region Constructors
        public Mountain(int x, int y)
        {
            X = x;
            Y = y;
            Height = 3;
            Trees = 0;
            FieldType = FieldType.Mountain;
        }
        #endregion
    }

    public class HighMountain : Terrain
    {
        #region Constructors
        public HighMountain(int x, int y)
        {
            X = x;
            Y = y;
            Height = 4;
            Trees = 0;
            FieldType = FieldType.HighMountain;
        }
        #endregion
    }
}
