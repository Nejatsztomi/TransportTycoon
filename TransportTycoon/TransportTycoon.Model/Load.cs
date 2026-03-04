using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.Model
{
    public enum LoadType 
    {
        Wheat,
        Oil,
        Wood,
        Flour,
        Rubber,
        Paper,
        People
    }

    public class Load
    {
        public int Price { protected get; set; }

    }
}
