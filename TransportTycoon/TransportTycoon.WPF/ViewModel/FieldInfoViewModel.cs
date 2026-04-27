using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.MapData;

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

    public static class FieldInfoFactory
    {
        public static FieldInfoViewModel Create(IField field)
        {
            return field switch
            {
                Terrain t => new TerrainFieldInfoViewModel
                {
                    
                    Type = t.GetType().ToString(),
                    Height = t.Height,
                    X = t.X,
                    Y = t.Y,
                    TreeCount = t.Trees
                },

                _ => new FieldInfoViewModel
                {
                    Type = field.GetType().ToString(),
                    Height = field.Height,
                    X = field.X,
                    Y = field.Y
                }
            };
        }
    }
}
