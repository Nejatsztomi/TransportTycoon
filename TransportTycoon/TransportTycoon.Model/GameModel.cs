using System;
using System.Collections.Generic;
using System.Text;
using TransportTycoon.MapData;

namespace TransportTycoon.Model
{
    public enum GameMode { Run, Paused,Editor}
    public enum TimeSpeed { Normal, Fast, SuperFast }
    public enum Difficulty {Easy, Medium, Hard}

    //Mintázat az összes osztályban
    #region Fields
    #endregion
    #region Properties
    #endregion
    #region Constructor
    #endregion
    #region Public Methods
    #endregion
    #region Private Methods
    #endregion
    #region Private event Methods
    #endregion

    



    public class GameModel
    {
        #region Fields

        #endregion

        #region Properties
        public Difficulty Difficulty { get; private set; }
        #endregion

        #region Events
        #endregion

        #region Constructor
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        private void SetTax() 
        {
            int tax = 30;
            switch (this.Difficulty) 
            {                
                case Difficulty.Easy:
                    tax = 10;
                    break;
                case Difficulty.Medium:
                    tax = 30;
                    break;
                case Difficulty.Hard:
                    tax = 50;
                    break;
                        
            }
            Goods.SetGlobalTax(tax);
        }
        #endregion

        #region Private event Methods
        #endregion

        #region Timer event handlers
        #endregion

    }




}
