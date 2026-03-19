namespace TransportTycoon.MapData
{
    public abstract class Industry : BuildingBlocks
    {
        public LoadType ConsumeGood { protected set; get; }
        public int ConsumeOccupancy { protected set; get; }
        protected Industry(int x, int y) : base(x, y)
        {
            Scaler = 2;
        }

        protected override void Production()
        {
            double multiplier = GetMultiplier();
            int production = Scaler * ConsumeOccupancy * Convert.ToInt32((double)Productivity * multiplier);

            if (Occupancy + production > Capacity)
            {
                Occupancy = Capacity;
                ConsumeOccupancy = (production / Scaler) / Convert.ToInt32((double)Productivity * multiplier);
            }
            else
            {
                Occupancy += production;
                ConsumeOccupancy = 0;
            }
        }
    }

    public class Mill : Industry
    {
        public Mill(int x, int y) : base(x, y)
        {
            ConsumeGood = LoadType.Wheat;
            Offset = 70;
        }

        public override LoadType GetLoad()
        {
            return LoadType.Flour;
        }
    }

    public class Plant : Industry
    {
        public Plant(int x, int y) : base(x, y)
        {
            ConsumeGood = LoadType.Wood;
            Offset = 60;
        }

        public override LoadType GetLoad()
        {
            return LoadType.Paper;
        }
    }

    public class Factory : Industry
    {
        public Factory(int x, int y) : base(x, y)
        {
            ConsumeGood = LoadType.Oil;
            Offset = 50;
        }

        public override LoadType GetLoad()
        {
            return LoadType.Rubber;
        }
    }
}
