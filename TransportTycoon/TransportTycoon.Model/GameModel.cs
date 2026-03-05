using System;
using System.Collections.Generic;
using System.Text;

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
            Goods.SetGlobalTax(this.Difficulty);
        }
        #endregion

        #region Private event Methods
        #endregion

        #region Timer event handlers
        #endregion

    }




}
