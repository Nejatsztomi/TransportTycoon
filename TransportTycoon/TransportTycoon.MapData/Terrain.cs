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

        public void Grow()
        {
            if (!IsFull)
            {
                Trees++;
            }
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
        public Plain(int x, int y)
        {
            X = x;
            Y = y;
            Height = 1;
            Trees = 0;
            FieldType = FieldType.Plain;
        }

    }
    public class Hill : Terrain
    {
        public Hill(int x, int y)
        {
            X = x;
            Y = y;
            Height = 2;
            Trees = 0;
            FieldType = FieldType.Hill;
        }
    }
    public class Mountin : Terrain
    {
        public Mountin(int x, int y)
        {
            X = x;
            Y = y;
            Height = 3;
            Trees = 0;
            FieldType = FieldType.Mountain;
        }
    }
    public class HightMountin : Terrain
    {
        public HightMountin(int x, int y)
        {
            X = x;
            Y = y;
            Height = 4;
            Trees = 0;
            FieldType = FieldType.HighMountain;
        }
    }
}
