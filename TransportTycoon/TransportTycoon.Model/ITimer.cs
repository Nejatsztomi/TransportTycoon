namespace TransportTycoon.Model
{
    /// <summary>
    /// Interface for a timer.
    /// Can be mocked.
    /// </summary>
    public interface ITimer
    {
        #region Properties
        /// <summary>
        /// Is the timer enabled?
        /// </summary>
        /// 
        public bool Enabled { get; set; }
        #endregion

        #region Events

        /// <summary>
        /// The event that is raised when the timer elapses.
        /// </summary>
        public event Action<double>? Tick;
        #endregion

        #region Methods
        /// <summary>
        /// Start the timer.
        /// </summary>
        /// 
        public void Start();

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void Stop();
        #endregion
    }
}
