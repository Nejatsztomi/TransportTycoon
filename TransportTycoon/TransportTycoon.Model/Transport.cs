using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.Model
{
    public abstract class Transport : Vehicle
    {
        #region Field
        public List<Goods> acceptedGoods {  get; protected set; }
        #endregion      
    }

    public class Van : Transport
    {
        #region Constructor
        public Van()
        {

        }
        #endregion
    }
}
