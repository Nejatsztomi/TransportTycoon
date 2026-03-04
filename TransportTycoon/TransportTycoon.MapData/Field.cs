using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.MapData
{
    public abstract class Field
    {
        public int x { protected set; get; }
        public int y { protected set; get; }
        public int height {protected set; get; }
        public virtual bool modifiable {protected set; get; }

        //i have left 2 datatypes because it shouldn't be here!!!!


        //note szerintem erre itt nincs szükség
        public void ChangeType(FieldType type) 
        {

        }

    }
}
