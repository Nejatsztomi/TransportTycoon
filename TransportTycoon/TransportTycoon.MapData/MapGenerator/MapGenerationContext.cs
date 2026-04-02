namespace TransportTycoon.MapData.MapGenerator
{
    // TODO: Add invarints to ensure width and height are positive, and seed can be any integer
    public readonly record struct MapGenerationContext(int Width, int Height, int Seed);
}
