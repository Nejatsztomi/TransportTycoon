using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.Model
{
    public class TransportTycoonFieldEventArgs : EventArgs
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
