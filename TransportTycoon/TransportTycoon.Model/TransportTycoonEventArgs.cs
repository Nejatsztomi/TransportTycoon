using System;
using System.Collections.Generic;
using System.Text;

namespace TransportTycoon.Model
{
    public class TransportTycoonEventArgs : EventArgs
    {
        private int gameTime;
        private int numberOfVehicles;

        public int GameTime 
        {
            get 
            {
                return gameTime;
            }
        }

        public int NumberOfVehicles
        {
            get
            {
                return numberOfVehicles;
            }
        }

        public TransportTycoonEventArgs(int gameTime, int numberOfVehicles) 
        {
            this.gameTime = gameTime;
            this.numberOfVehicles = numberOfVehicles;
        }
    }
}
