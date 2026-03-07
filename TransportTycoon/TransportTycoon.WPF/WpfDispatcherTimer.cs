using System.Windows.Threading;
using ITimer = TransportTycoon.Model.ITimer;

namespace TransportTycoon.WPF
{
    /// <summary>
    /// Represents a timer that is integrated with the WPF dispatcher.
    /// </summary>
    internal class WpfDispatcherTimer : ITimer
    {
        #region Private fields
        private readonly DispatcherTimer _timer;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the WpfDispatcherTimer class with the specified time interval between ticks.
        /// </summary>
        /// <param name="interval">The time interval between timer ticks. Must be a positive TimeSpan value.</param>
        public WpfDispatcherTimer(TimeSpan interval)
        {
            _timer = new DispatcherTimer()
            {
                Interval = interval,
            };
            _timer.Tick += (s, e) => Elapsed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the WPFDispatcherTimer class with a default interval of one second.
        /// </summary>
        public WpfDispatcherTimer() : this(TimeSpan.FromSeconds(1)) { }

        /// <summary>
        /// Initializes a new instance of the WPFDispatcherTimer class that raises the Tick event at the specified
        /// interval, measured in milliseconds.
        /// </summary>
        /// <remarks>This constructor is a convenience overload that allows specifying the timer interval
        /// in milliseconds. The value is internally converted to a TimeSpan.</remarks>
        /// <param name="interval">The interval, in milliseconds, at which the timer raises the Tick event. Must be a positive value.</param>
        public WpfDispatcherTimer(long interval) : this(TimeSpan.FromMilliseconds(interval)) { }

        /// <summary>
        /// Initializes a new instance of the WPFDispatcherTimer class that raises the Tick event at the specified
        /// interval, measured in milliseconds.
        /// </summary>
        /// <remarks>This constructor is a convenience overload that allows specifying the timer interval
        /// in milliseconds. The value is internally converted to a TimeSpan.</remarks>
        /// <param name="interval">The interval, in milliseconds, at which the timer raises the Tick event. Must be a positive value.</param>
        public WpfDispatcherTimer(int interval) : this(TimeSpan.FromMilliseconds(interval)) { }
        #endregion

        #region Public properties
        public bool Enabled
        {
            get => _timer.IsEnabled;
            set
            {
                if (value)
                    _timer.Start();
                else
                    _timer.Stop();
            }
        }

        public double Interval
        {
            get => _timer.Interval.TotalMilliseconds;
            set => _timer.Interval = TimeSpan.FromMilliseconds(value);
        }
        #endregion

        #region Events
        public event EventHandler? Elapsed;
        #endregion

        #region Public methods
        public void Start() => _timer.Start();

        public void Stop() => _timer.Stop();
        #endregion
    }
}
