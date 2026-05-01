using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TransportTycoon.WPF.ViewModel;

namespace TransportTycoon.WPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        #region Private fields
        private Point? _dragStartPoint = null;
        private Point _dragStartCamera;
        private bool _isLeftDragging;
        private int _lastDragRoadX = -1;
        private int _lastDragRoadY = -1;
        #endregion

        #region Bindings
        /// <summary>
        /// A binding for the viewmodel.
        /// </summary>
        /// <remarks>
        /// This is need properly linking the <see cref="GameViewModel"/> to this UserControl.
        /// We need to subscribe to the <see cref="GameViewModel.MapUpdated"/> event to know when to redraw the map.
        /// </remarks>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(GameViewModel),
                typeof(Map),
                new PropertyMetadata(null, OnViewModelChanged));
        #endregion

        #region Properties
        /// <summary>
        /// The underlying property for the <see cref="ViewModelProperty"/>.
        /// </summary>
        public GameViewModel? ViewModel
        {
            get => (GameViewModel?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Exposes the internal <see cref="FastMapRenderer"/> to allow other controls (like the minimap) to link to it.
        /// </summary>
        public FastMapRenderer GameMapRenderer => InternalGameMapRenderer;
        #endregion

        #region Constructors
        public Map()
        {
            InitializeComponent();

            CompositionTarget.Rendering += OnFrameRendered;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Calculates the tile coordinates corresponding to the specified mouse position in screen space.
        /// </summary>
        /// <param name="mousePos">The mouse position in screen coordinates for which to determine the tile coordinates.</param>
        /// <returns>A tuple containing the X and Y indices of the tile at the specified mouse position.</returns>
        private (int tileX, int tileY) GetTileCoordinatesFromMousePosition(Point mousePos)
        {
            (double worldX, double worldY) = GetWorldCoordinatesFromMousePosition(mousePos);
            int tileX = (int)worldX / FastMapRenderer.TileSize;
            int tileY = (int)worldY / FastMapRenderer.TileSize;
            return (tileX, tileY);
        }

        /// <summary>
        /// Calculates the world coordinates corresponding to the specified mouse position on the screen.
        /// </summary>
        /// <param name="mousePos">The position of the mouse pointer in screen coordinates, relative to the top-left corner of the viewport.</param>
        /// <returns>A tuple containing the X and Y coordinates in world space that correspond to the given mouse position.</returns>
        private (double worldX, double worldY) GetWorldCoordinatesFromMousePosition(Point mousePos)
        {
            double worldX = InternalGameMapRenderer.CameraX + (mousePos.X / InternalGameMapRenderer.ZoomLevel);
            double worldY = InternalGameMapRenderer.CameraY + (mousePos.Y / InternalGameMapRenderer.ZoomLevel);
            return (worldX, worldY);
        }

        /// <summary>
        /// Determines whether the specified tile coordinates are within the bounds of the current game map.
        /// </summary>
        /// <param name="tileX">The tile's X-coordinate to check.</param>
        /// <param name="tileY">The tile's Y-coordiante to check.</param>
        /// <returns><see langword="true"/> if both tileX and tileY are within the valid range of the map; otherwise, <see langword="false"/>.</returns>
        private bool IsInMapBounds(int tileX, int tileY)
        {
            int mapWidth = InternalGameMapRenderer.Map.GetLength(0);
            int mapHeight = InternalGameMapRenderer.Map.GetLength(1);
            return (0 <= tileX && tileX < mapWidth) && (0 <= tileY && tileY < mapHeight);
        }
        #endregion

        #region Private event methods
        /// <summary>
        /// WPF's game loop event. We use it to trigger redraws of the map.
        /// </summary>
        /// <param name="_1"></param>
        /// <param name="_2"></param>
        private void OnFrameRendered(object? _1, EventArgs _2)
        {
            InternalGameMapRenderer.Redraw();
        }

        /// <summary>
        /// Unload event to prevent memory leaks.
        /// </summary>
        /// <param name="_1"></param>
        /// <param name="_2"></param>
        private void UserControl_Unloaded(object? _1, EventArgs _2)
        {
            CompositionTarget.Rendering -= OnFrameRendered;
        }

        /// <summary>
        /// The method which is triggered when the <see cref="ViewModel"/> property changes.
        /// It is responsible for subscribing to the new viewmodel's events and unsubscribing from the old one.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var map = (Map)d;
            GameViewModel? oldVm = e.OldValue as GameViewModel;
            GameViewModel? newVm = e.NewValue as GameViewModel;

            oldVm?.VehicleDestroyed -= map.GameViewModel_OnVehichleDestroyed;
            oldVm?.ShowBalanceMessage -= map.GameViewModel_OnShowBalanceMessage;

            newVm?.VehicleDestroyed += map.GameViewModel_OnVehichleDestroyed;
            newVm?.ShowBalanceMessage += map.GameViewModel_OnShowBalanceMessage;
        }

        private void GameViewModel_OnShowBalanceMessage(int x, int y, int value)////
        {
            string text = value.ToString();
            _ = ShowFloatingMessage(text, x, y);
        }

        private async Task ShowFloatingMessage(string text, int tileX, int tileY)////
        {
            //Pixel point coordinates
            int TILE_SIZE = FastMapRenderer.TileSize;
            double worldX = tileX * TILE_SIZE;
            double worldY = tileY * TILE_SIZE;

            //Pixel point coordinates from screen's upper left corner
            double screenX = (worldX - InternalGameMapRenderer.CameraX) * InternalGameMapRenderer.ZoomLevel;
            double screenY = (worldY - InternalGameMapRenderer.CameraY) * InternalGameMapRenderer.ZoomLevel;

            //Message color
            Brush textColor;
            if (text.StartsWith("-")) textColor = Brushes.Red;
            else textColor = Brushes.LightGreen;

            //Message text
            var textBlock = new System.Windows.Controls.TextBlock
            {
                Text = text,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = textColor,
                Opacity = 1.0
            };

            // Coin picture
            var coinImage = new Image
            {
                Source = new BitmapImage(new Uri("/Assets/Images/Icons/coin.png", UriKind.Relative)),
                Width = 24,
                Height = 24,
                Opacity = 1.0,
                Margin = new Thickness(3, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            // StackPanel for message
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0)
            };
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(coinImage);

            //Stackpanel added to canvas
            FloatingCanvas.Children.Add(stackPanel);
            Canvas.SetLeft(stackPanel, screenX);
            Canvas.SetTop(stackPanel, screenY);

            //Animation start and end point, timer for animation and duration of animation
            double startY = screenY;
            double endY = screenY - 50;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            double durationMs = 1000;

            //Animation loop
            while (sw.ElapsedMilliseconds < durationMs)
            {
                //Elapsed time ratio
                double t = sw.ElapsedMilliseconds / durationMs;
                //New point with interpolation due to elapsed time ratio
                double newY = startY + (endY - startY) * t;
                //Reduce opacity
                double newOpacity = 1.0 - t;

                //Set new properties of stackpanel
                await Dispatcher.InvokeAsync(() =>
                {
                    Canvas.SetTop(stackPanel, newY);
                    stackPanel.Opacity = newOpacity;
                });

                //Wait a little in order to smooth animation
                await Task.Delay(16);
            }

            //Remove stackpanel from canvas
            await Dispatcher.InvokeAsync(() => FloatingCanvas.Children.Remove(stackPanel));
        }

        private void GameViewModel_OnVehichleDestroyed(UInt64 vehicleId)
        {
            GameMapRenderer.RemoveVehicleFromCache(vehicleId);
        }

        /// <summary>
        /// Eventhandler for Mouse Right Button press.
        /// </summary>
        private void GameMapRenderer_PreviewMouseRightButtonDown(object? _, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(this);
            _dragStartCamera = new(InternalGameMapRenderer.CameraX, InternalGameMapRenderer.CameraY);
            InternalGameMapRenderer.CaptureMouse();
        }

        /// <summary>
        /// An eventhandler for Mouse Movement.
        /// </summary>
        private void GameMapRenderer_PreviewMouseMove(object? _, MouseEventArgs e)
        {
            if (DataContext is not GameViewModel viewModel) return;

            if (_isLeftDragging && (viewModel.SelectedButton == 21 || viewModel.SelectedButton == 24))
            {
                Point dragMousePos = e.GetPosition(InternalGameMapRenderer);
                (int dragX, int dragY) = GetTileCoordinatesFromMousePosition(dragMousePos);

                if (IsInMapBounds(dragX, dragY))
                {
                    if (dragX != _lastDragRoadX || dragY != _lastDragRoadY)
                    {
                        _lastDragRoadX = dragX;
                        _lastDragRoadY = dragY;

                        viewModel.OnTileLeftClick(dragX, dragY);
                    }
                }
            }

            if (_dragStartPoint.HasValue)
            {
                Point screenMousePos = e.GetPosition(this);

                // Calculate how much the mouse has moved in world coordinates
                double deltaX = (screenMousePos.X - _dragStartPoint.Value.X) / InternalGameMapRenderer.ZoomLevel;
                double deltaY = (screenMousePos.Y - _dragStartPoint.Value.Y) / InternalGameMapRenderer.ZoomLevel;

                //// Calculate camera position based on the mouse movement
                double desiredCameraX = _dragStartCamera.X - deltaX;
                double desiredCameraY = _dragStartCamera.Y - deltaY;

                InternalGameMapRenderer.SetCameraView(desiredCameraX, desiredCameraY, InternalGameMapRenderer.ZoomLevel);
            }
            else
            {
                Point screenMousePos = e.GetPosition(InternalGameMapRenderer);

                (int tileX, int tileY) = GetTileCoordinatesFromMousePosition(screenMousePos);

                if (IsInMapBounds(tileX, tileY))
                {
                    if (InternalGameMapRenderer.HoverX != tileX || InternalGameMapRenderer.HoverY != tileY)
                    {
                        InternalGameMapRenderer.HoverX = tileX;
                        InternalGameMapRenderer.HoverY = tileY;
                    }
                }
                else
                {
                    if (InternalGameMapRenderer.HoverX != -1)
                    {
                        InternalGameMapRenderer.HoverX = -1;
                        InternalGameMapRenderer.HoverY = -1;
                    }
                }
            }
        }

        /// <summary>
        /// An eventhandler for Mouse Right Button release.
        /// </summary>
        /// <param name="_1"></param>
        /// <param name="_2"></param>
        private void GameMapRenderer_PreviewMouseRightButtonUp(object? _1, MouseButtonEventArgs _2)
        {
            if (_dragStartPoint.HasValue)
            {
                InternalGameMapRenderer.ReleaseMouseCapture();
                _dragStartPoint = null;
            }
        }

        /// <summary>
        /// An eventhandler for Mouse Wheel Scrolling.
        /// </summary>
        private void GameMapRenderer_PreviewMouseWheel(object? _, MouseWheelEventArgs e)
        {
            const double ZOOM_IN_STEP = 1.1;
            const double ZOOM_OUT_STEP = 1 / 1.1;

            double zoomFactor = e.Delta > 0 ? ZOOM_IN_STEP : ZOOM_OUT_STEP;
            double newZoomLevel = InternalGameMapRenderer.ZoomLevel * zoomFactor;

            // Get mouse position
            Point screenMousePos = e.GetPosition(InternalGameMapRenderer);

            (double wordX, double wordY) = GetWorldCoordinatesFromMousePosition(screenMousePos);

            double desiredCameraX = wordX - (screenMousePos.X / newZoomLevel);
            double desiredCameraY = wordY - (screenMousePos.Y / newZoomLevel);

            InternalGameMapRenderer.SetCameraView(desiredCameraX, desiredCameraY, newZoomLevel);
        }

        /// <summary>
        /// An eventhandler Left Mouse Button press.
        /// </summary>
        private void GameMapRenderer_PreviewMouseLeftButtonDown(object? _, MouseButtonEventArgs e)
        {
            Point screenMousePos = e.GetPosition(InternalGameMapRenderer);

            (int tileX, int tileY) = GetTileCoordinatesFromMousePosition(screenMousePos);

            if (IsInMapBounds(tileX, tileY))
            {
                //InternalGameMapRenderer.SelectedTile = tileX;
                //InternalGameMapRenderer.SelectedY = tileY;

                _isLeftDragging = true;
                _lastDragRoadX = tileX;
                _lastDragRoadY = tileY;

                InternalGameMapRenderer.CaptureMouse();

                if (DataContext is GameViewModel viewModel)
                {
                    viewModel.OnTileLeftClick(tileX, tileY);
                }
            }
            //else
            //{
            //    InternalGameMapRenderer.SelectedTile = -1;
            //    InternalGameMapRenderer.SelectedY = -1;
            //}
        }

        /// <summary>
        /// An eventhandler Left Mouse Button release.
        /// </summary>
        private void GameMapRenderer_PreviewMouseLeftButtonUp(object? _1, MouseButtonEventArgs _2)
        {
            //InternalGameMapRenderer.SelectedTile = -1;
            //InternalGameMapRenderer.SelectedY = -1;

            _isLeftDragging = false;
            _lastDragRoadX = -1;
            _lastDragRoadY = -1;

            InternalGameMapRenderer.ReleaseMouseCapture();
        }

        /// <summary>
        /// An eventhandler Middle/Wheel Mouse Button press.
        /// </summary>
        private void GameMapRenderer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle) return;

            Point screenMousePos = e.GetPosition(InternalGameMapRenderer);

            (int tileX, int tileY) = GetTileCoordinatesFromMousePosition(screenMousePos);

            if (IsInMapBounds(tileX, tileY))
            {
                if (DataContext is GameViewModel viewModel)
                {
                    viewModel.OnTileWheelClick(tileX, tileY);
                }
            }
        }
        #endregion
    }
}
