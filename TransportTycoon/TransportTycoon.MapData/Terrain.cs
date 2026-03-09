namespace TransportTycoon.MapData
{
    public abstract class Terrain : Field
    {
        //datas
        public int Trees { protected set; get; }
        public override bool Modifiable { get; protected set; }
        //methods
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
            if (!IsFull()) 
            {
                Trees++;
            }
        }

        public bool IsFull() 
        {
            if (Trees == 4) 
            {
                return true;
            }
            return false;
        }

        public void SpreadForest() 
        {
            Trees = 1;
        }

        public override int GetTrees() => Trees;
    }


    public class Plain : Terrain 
    {
        public Plain(int x, int y)
        {
            X = x;
            Y = y;
            Height = 1;
            Trees = 0;
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
        }
    }

    


}
