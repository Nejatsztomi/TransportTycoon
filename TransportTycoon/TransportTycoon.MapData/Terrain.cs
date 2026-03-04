namespace TransportTycoon.MapData
{
    public abstract class Terrain : Field
    {
        //datas
        public int trees { protected set; get; }
        public override bool modifiable { get; protected set; }
        //methods
        public void IncreaseHeight() 
        {
            this.height++;
        }
        public void DecreaseHeight() 
        {
            this.height--; 
        }
        public void Grow() 
        {
            if (!IsFull()) 
            {
                trees++;
            }
        }

        public bool IsFull() 
        {
            if (trees == 4) 
            {
                return true;
            }
            return false;
        }

        public void SpreadForest() 
        {
            trees = 1;
        }
    }


    public class Plain : Terrain 
    {
        public Plain(int x, int y,int height, int trees)
        {
            this.x = x;
            this.y = y;
            this.height = height;
            this.trees = 0;
        }
    }
    public class Hill : Terrain
    {
        public Hill(int x, int y, int height, int trees)
        {
            this.x = x;
            this.y = y;
            this.height = height;
            this.trees = 0;
        }
    }
    public class HightMountin : Terrain
    {
        public HightMountin(int x, int y, int height, int trees)
        {
            this.x = x;
            this.y = y;
            this.height = height;
            this.trees = 0;
        }
    }

    public class Mountin : Terrain
    {
        public Mountin(int x, int y, int height, int trees)
        {
            this.x = x;
            this.y = y;
            this.height = height;
            this.trees = 0;
        }
    }


}
