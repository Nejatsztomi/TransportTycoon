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

    public abstract class Load
    {
        public int Price { protected get; set; } 
    }

    public class People : Load
    {
        public People() 
        {
            Price = 120;
        }
    }



}
