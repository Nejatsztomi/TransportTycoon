using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.WPF.ViewModel
{
    public partial class FieldInfoViewModel : ViewModelBase
    {
        public string Type { get; init; } = "";
        public int Height { get; init; }
        public int X { get; init; }
        public int Y { get; init; }
    }

    public class TerrainFieldInfoViewModel : FieldInfoViewModel
    {
        public int TreeCount { get; init; }
    }
}
