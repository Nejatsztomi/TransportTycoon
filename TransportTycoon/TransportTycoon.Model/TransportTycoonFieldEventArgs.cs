namespace TransportTycoon.Model
{
    public sealed class TransportTycoonFieldEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }

        public TransportTycoonFieldEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
