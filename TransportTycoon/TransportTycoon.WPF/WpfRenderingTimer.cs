using System.Diagnostics;
using System.Windows.Media;
using ITimer = TransportTycoon.Model.ITimer;

namespace TransportTycoon.WPF
{
    internal sealed class WpfRenderingTimer : ITimer
    {
        #region Private fields
        private readonly Stopwatch _stopwatch;
        private TimeSpan _lastRenderTime;
        #endregion

        #region Properties
        public bool Enabled { get; set; }
        #endregion

        #region Events
        public event Action<double>? Tick;
        #endregion

        #region Constructors
        public WpfRenderingTimer()
        {
            _stopwatch = new();
        }
        #endregion

        #region Public methods
        public void Start()
        {
            if (Enabled) return;
            Enabled = true;

            _stopwatch.Start();
            _lastRenderTime = _stopwatch.Elapsed;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public void Stop()
        {
            if (!Enabled) return;
            Enabled = false;

            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            _stopwatch.Stop();
        }
        #endregion

        #region Private methods
        private void CompositionTarget_Rendering(object? _1, EventArgs _2)
        {
            TimeSpan currentRenderTime = _stopwatch.Elapsed;
            double deltaTime = (currentRenderTime - _lastRenderTime).TotalSeconds;
            _lastRenderTime = currentRenderTime;

            Tick?.Invoke(deltaTime);
        }
        #endregion
    }
}
