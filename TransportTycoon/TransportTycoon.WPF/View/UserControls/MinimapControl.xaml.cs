using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TransportTycoon.WPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for MinimapControl.xaml
    /// </summary>
    public partial class MinimapControl : UserControl
    {
        #region Properties
        public static readonly DependencyProperty TargetRendererProperty =
        DependencyProperty.Register(
            nameof(TargetRenderer),
            typeof(FastMapRenderer),
            typeof(MinimapControl),
            new PropertyMetadata(null, OnTargetRendererChanged));

        public FastMapRenderer TargetRenderer
        {
            get => (FastMapRenderer)GetValue(TargetRendererProperty);
            set => SetValue(TargetRendererProperty, value);
        }
        #endregion

        #region Constructors
        public MinimapControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Private static methods
        private static void OnTargetRendererChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MinimapControl minimap)
            {
                if (e.OldValue is FastMapRenderer oldRenderer)
                {
                    oldRenderer.LayoutUpdated -= minimap.Renderer_LayoutUpdated;
                }

                if (e.NewValue is FastMapRenderer newRenderer)
                {
                    newRenderer.LayoutUpdated += minimap.Renderer_LayoutUpdated;
                }
            }
        }
        #endregion

        #region Private methods
        private void Renderer_LayoutUpdated(object? _1, EventArgs _2)
        {
            UpdateMinimapViewport();
        }

        private void UpdateMinimapViewport()
        {
            if (TargetRenderer.Map is null || TargetRenderer.ActualWidth <= 0.0) return;

            double totalWorldWidth = TargetRenderer.Map.GetLength(0) * FastMapRenderer.TileSize;
            double totalWorldHeight = TargetRenderer.Map.GetLength(1) * FastMapRenderer.TileSize;

            double scaleX = MinimapCanvas.ActualWidth / totalWorldWidth;
            double scaleY = MinimapCanvas.ActualHeight / totalWorldHeight;

            double boxX = TargetRenderer.CameraX * scaleX;
            double boxY = TargetRenderer.CameraY * scaleY;

            double visibleWorldWidth = TargetRenderer.ActualWidth / TargetRenderer.ZoomLevel;
            double visibleWorldHeight = TargetRenderer.ActualHeight / TargetRenderer.ZoomLevel;

            double boxWidth = visibleWorldWidth * scaleX;
            double boxHeight = visibleWorldHeight * scaleY;

            Canvas.SetLeft(MinimapViewportBox, boxX);
            Canvas.SetTop(MinimapViewportBox, boxY);

            MinimapViewportBox.Width = Math.Max(1, boxWidth);
            MinimapViewportBox.Height = Math.Max(1, boxHeight);
        }

        private void NavigateFromMinimap(Point minimapPos)
        {
            if (TargetRenderer.Map is null || TargetRenderer.ActualWidth <= 0.0) return;

            double percentX = minimapPos.X / MinimapCanvas.ActualWidth;
            double percentY = minimapPos.Y / MinimapCanvas.ActualHeight;

            double totalWorldWidth = TargetRenderer.Map.GetLength(0) * FastMapRenderer.TileSize;
            double totalWorldHeight = TargetRenderer.Map.GetLength(1) * FastMapRenderer.TileSize;

            double targetWorldX = percentX * totalWorldWidth;
            double targetWorldY = percentY * totalWorldHeight;

            double visibleWorldWidth = TargetRenderer.ActualWidth / TargetRenderer.ZoomLevel;
            double visibleWorldHeight = TargetRenderer.ActualHeight / TargetRenderer.ZoomLevel;

            double desiredCameraX = targetWorldX - (visibleWorldWidth / 2);
            double desiredCameraY = targetWorldY - (visibleWorldHeight / 2);

            TargetRenderer.CameraX = desiredCameraX;
            TargetRenderer.CameraY = desiredCameraY;

            UpdateMinimapViewport();
        }
        #endregion

        #region Private event methods
        private void MinimapCanvas_PreviewMouseLeftButtonDown(object? _, MouseButtonEventArgs e)
        {
            MinimapCanvas.CaptureMouse();

            NavigateFromMinimap(e.GetPosition(MinimapCanvas));
        }

        private void MinimapCanvas_PreviewMouseMove(object? _, MouseEventArgs e)
        {
            // Only navigate if they are actually holding the left mouse button down
            if (e.LeftButton == MouseButtonState.Pressed && MinimapCanvas.IsMouseCaptured)
            {
                NavigateFromMinimap(e.GetPosition(MinimapCanvas));
            }
        }

        private void MinimapCanvas_PreviewMouseLeftButtonUp(object? _1, MouseButtonEventArgs _2)
        {
            MinimapCanvas.ReleaseMouseCapture();
        }
        #endregion
    }
}
