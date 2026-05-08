using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TransportTycoon.MapData;
using TransportTycoon.MapData.Buildings;
using TransportTycoon.Model;
using TransportTycoon.WPF.Utils;

namespace TransportTycoon.WPF.View.UserControls
{
    #region Internal enums
    internal enum FieldType : byte
    {
        Water = 0,
        Plain = 1,
        Hill = 2,
        Mountain = 3,
        HighMountain = 4,

        House,
        Farm,
        Mine,
        LumberCamp,
        Mill,
        Factory,
        Plant,
        Road,
        Bridge,
        Stop,
    }

    internal enum VehicleType : byte
    {
        Van = 0,
        Pickup = 1,
        Truck = 2,
        LiquidTruck = 3,
        SmallBus = 4,
        BigBus = 5,
    }

    internal enum RoadType : byte
    {
        Horizontal = 0,
        Vertical = 1,
        RightTurn = 2,
        LeftTurn = 3,
        UpperRightTurn = 4,
        UpperLeftTurn = 5,
        UpperTRoad = 6,
        DownTRoad = 7,
        RightTRoad = 8,
        LeftTRoad = 9,
        XRoad = 10,
    }

    internal enum BridgeType : byte
    {
        HorizontalGreenBridge = 0,
        VerticalGreenBridge = 1,
        HorizontalYellowBridge = 2,
        VerticalYellowBridge = 3,
        HorizontalRedBridge = 4,
        VerticalRedBridge = 5,
        Null = 6,

    }
    #endregion

    public class FastMapRenderer : FrameworkElement
    {
        #region Constants
        public const int TileSize = 64;
        #endregion

        #region Private fields
        /// <summary>
        /// A dictionary that link each <see cref="FieldType"/> to their corresponding terrain image.
        /// </summary>
        /// <remarks>Must be set manually for each <see cref="FieldType"/>.</remarks>
        private readonly Dictionary<FieldType, ImageSource> _terrainTextures;

        /// <summary>
        /// A dictionary that link each <see cref="FieldType"/> to their structure image.
        /// </summary>
        /// <remarks>Must be set manually for each <see cref="FieldType"/>.</remarks>
        private readonly Dictionary<FieldType, ImageSource> _structureTextures;

        /// <summary>
        /// A dictionary that link each <see cref="RoadType"/> to their road image.
        /// </summary>
        /// <remarks>Must be set manually for each <see cref="RoadType"/>.</remarks>
        private readonly Dictionary<RoadType, ImageSource> _roadTextures;

        /// <summary>
        /// A dictionary that link each <see cref="BridgeType"/> to their bridge image.
        /// </summary>
        /// <remarks>Must be set manually for each <see cref="BridgeType"/>.</remarks>
        private readonly Dictionary<BridgeType, ImageSource> _bridgeTextures;

        /// <summary>
        /// A dictionary that link integers 1 to 4 to their tree image.
        /// </summary>
        /// <remarks>Must be set manually for each tree type.</remarks>
        private readonly Dictionary<int, ImageSource> _treesTextures;

        /// <summary>
        /// A dictionary that link each vehicle type to their image.
        /// </summary>
        /// <remarks>Must be set manually for each vehicle type. Not currently implemented.</remarks>
        private readonly Dictionary<VehicleType, ImageSource> _vehicleTextures;

        /// <summary>
        /// A cached brush for highlighting (adding occupancy effect) the hovered tile.
        /// </summary>
        private readonly Brush _highlightBrush = new SolidColorBrush(Color.FromArgb(100, 0, 150, 255));

        /// <summary>
        /// A cached pen for highlighting (adding border) the hovered tile.
        /// </summary>
        private readonly Pen _highlightPen = new(new SolidColorBrush(Color.FromArgb(150, 0, 255, 0)), 2.0);

        /// <summary>
        /// A cached pen for highlighting (adding border) the selected (left-clicked) tile.
        /// </summary>
        private readonly Pen _selectionPen = new(new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), 2.0);

        private readonly Dictionary<int, BitmapSource> _stopTextures;

        /// <summary>
        /// The font type for writing.
        /// </summary>
        private readonly Typeface _stopTextTypeface = new("Arial");

        /// <summary>
        /// The color for writing the stop order on the stop tiles.
        /// </summary>
        private readonly Brush _stopTextBrush = Brushes.White;

        /// <summary>
        /// The border pen for the stop tiles when they are part of the route.
        /// </summary>
        private readonly Pen _stopBorderPen = new(new SolidColorBrush(Color.FromArgb(255, 255, 165, 0)), 3.0);

        private readonly TranslateTransform _cameraTransform = new();
        private readonly ScaleTransform _zoomTransform = new();
        private readonly TransformGroup _cameraTransformGroup = new();

        private readonly Dictionary<UInt64, RotateTransform> _vehicleRotationCache = new(100);
        #endregion

        #region Bindings
        /// <summary>
        /// A binding for the map tiles.
        /// </summary>
        /// <remarks>Since views can't access viewmodels we need to bind it.</remarks>
        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register(
                nameof(Map),
                typeof(IField[,]),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VehiclesProperty =
            DependencyProperty.Register(
            nameof(Vehicles),
            typeof(IEnumerable<Vehicle>), // Assuming you have a base interface for vehicles
            typeof(FastMapRenderer),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// A depedency property for the camera's X position.
        /// It allows the view to move horizontally by changing this value.
        /// </summary>
        /// <remarks>Every time this value changes it causes a rerender to take effect.</remarks>
        public static readonly DependencyProperty CameraXProperty =
            DependencyProperty.Register(
                nameof(CameraX),
                typeof(double),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// A depedency property for the camera's Y position.
        /// It allows the view to move vertically by changing this value.
        /// </summary>
        /// <remarks>Every time this value changes it causes a rerender to take effect.</remarks>
        public static readonly DependencyProperty CameraYProperty =
            DependencyProperty.Register(
                nameof(CameraY),
                typeof(double),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// A depedency property for the camera's zoom level.
        /// </summary>
        /// <remarks>Every time this value changes it causes a rerender to take effect.</remarks>
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register(
                nameof(ZoomLevel),
                typeof(double),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// A depdency property for the X coordinate of the tile currently hovered by the mouse.
        /// By default it is set to <see langword="-1"/>, which means out of bounds and thus no tile is hovered.
        /// </summary>
        /// <remarks>Every time this value changes it causes a rerender to take effect.</remarks>
        public static readonly DependencyProperty HoverXProperty =
            DependencyProperty.Register(
                nameof(HoverX),
                typeof(int),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// A depdency property for the Y coordinate of the tile currently hovered by the mouse.
        /// By default it is set to <see langword="-1"/>, which means out of bounds and thus no tile is hovered. 
        /// </summary>
        /// <remarks>Every time this value changes it causes a rerender to take effect.</remarks>
        public static readonly DependencyProperty HoverYProperty =
            DependencyProperty.Register(
                nameof(HoverY),
                typeof(int),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedTileProperty =
            DependencyProperty.Register(
                nameof(SelectedTile),
                typeof(IField),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RouteStopsProperty =
            DependencyProperty.Register(
            nameof(RouteStops),
            typeof(IEnumerable<StopData>),
            typeof(FastMapRenderer),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Properties
        /// <summary>
        /// The underlying property for <see cref="MapProperty"/>.
        /// </summary>
        public IField[,] Map
        {
            get => (IField[,])GetValue(MapProperty);
            set
            {
                SetValue(MapProperty, value);
            }
        }

        /// <summary>
        /// The underlying property for <see cref="VehiclesProperty"/>.
        /// </summary>
        public IEnumerable<Vehicle> Vehicles
        {
            get => (IEnumerable<Vehicle>)GetValue(VehiclesProperty);
            set => SetValue(VehiclesProperty, value);
        }

        /// <summary>
        /// The underlying property for <see cref="CameraXProperty"/>.
        /// </summary>
        public double CameraX
        {
            get => (double)GetValue(CameraXProperty);
            set => SetValue(CameraXProperty, value);
        }

        /// <summary>
        /// The underlying property for <see cref="CameraYProperty"/>.
        /// </summary>
        public double CameraY
        {
            get => (double)GetValue(CameraYProperty);
            set => SetValue(CameraYProperty, value);
        }

        /// <summary>
        /// The underlying property for <see cref="ZoomLevelProperty"/>.
        /// </summary>
        public double ZoomLevel
        {
            get => (double)GetValue(ZoomLevelProperty);
            set => SetValue(ZoomLevelProperty, value);
        }

        /// <summary>
        /// The underlying property for <see cref="HoverXProperty"/>.
        /// </summary>
        public int HoverX
        {
            get => (int)GetValue(HoverXProperty);
            set => SetValue(HoverXProperty, value);
        }

        /// <summary>
        /// The underlying property for <see cref="HoverYProperty"/>.
        /// </summary>
        public int HoverY
        {
            get => (int)GetValue(HoverYProperty);
            set => SetValue(HoverYProperty, value);
        }

        /// <summary>
        /// The underlying property for <see cref="SelectedTileProperty"/>.
        /// </summary>
        public IField? SelectedTile
        {
            get => (IField?)GetValue(SelectedTileProperty);
            set => SetValue(SelectedTileProperty, value);
        }

        /// <summary>
        /// The underlying property for <see cref="RouteStopsProperty"/>.
        /// </summary>
        public IEnumerable<StopData> RouteStops
        {
            get => (IEnumerable<StopData>)GetValue(RouteStopsProperty);
            set => SetValue(RouteStopsProperty, value);
        }
        #endregion

        #region Constructors
        public FastMapRenderer()
        {
            // RenderOptions
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            // Disable anti-aliasing
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            // Freeze the brushes and pens (just like with images)
            if (_highlightBrush.CanFreeze) _highlightBrush.Freeze();
            if (_highlightPen.CanFreeze) _highlightPen.Freeze();
            if (_selectionPen.CanFreeze) _selectionPen.Freeze();
            if (_stopBorderPen.CanFreeze) _stopBorderPen.Freeze();
            if (_stopTextBrush.CanFreeze) _stopTextBrush.Freeze();

            // Setup the transform group for camera movement and zooming
            _cameraTransformGroup.Children.Add(_cameraTransform);
            _cameraTransformGroup.Children.Add(_zoomTransform);

            // Generate the stop textures
            _stopTextures = new(20);
            GenerateRouteStopTextures(0, 20);

            // TODO: Later maybe JSON or .rex format
            _terrainTextures = new()
            {
                { FieldType.Water, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/water2.png")) },
                { FieldType.Plain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/plain.png")) },
                { FieldType.Hill, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/hill.png")) },
                { FieldType.Mountain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/mountain3.png")) },
                { FieldType.HighMountain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/highmountain3.png")) },
            };

            _structureTextures = new()
            {
                { FieldType.Farm, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Buildings/farm.png")) },
                { FieldType.LumberCamp, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Buildings/lumbercamp.png")) },
                { FieldType.Mine, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Buildings/mine.jpg")) },
                { FieldType.Mill, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Buildings/mill.png")) },
                { FieldType.Plant, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Buildings/plant.png")) },
                { FieldType.Factory, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Buildings/factory.png")) },
                { FieldType.House, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Buildings/house.png")) },
                { FieldType.Stop, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Stop/stop.png")) },
            };

            _treesTextures = new()
            {
                {1, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Trees/tree1.png"))  },
                {2, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Trees/tree2.png")) },
                {3, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Trees/tree3.png")) },
                {4, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Trees/tree4.png")) },
            };

            var straightRoadTexture = LoadTexture(new Uri("pack://application:,,,/Assets/Images/Road/road.png"));
            var turnRoadTexture = LoadTexture(new Uri("pack://application:,,,/Assets/Images/Road/turn.png"));
            var crossTRoadTexture = LoadTexture(new Uri("pack://application:,,,/Assets/Images/Road/crossT.png"));
            var crossXRoadTexture = LoadTexture(new Uri("pack://application:,,,/Assets/Images/Road/crossX.png"));
            _roadTextures = new()
            {
                // Straight
                { RoadType.Vertical, straightRoadTexture },
                { RoadType.Horizontal, RotateTexture(straightRoadTexture, 90.0) },
                
                // Turn
                { RoadType.RightTurn, turnRoadTexture },
                { RoadType.LeftTurn, RotateTexture(turnRoadTexture, 90.0) },
                { RoadType.UpperLeftTurn, RotateTexture(turnRoadTexture, 180.0) },
                { RoadType.UpperRightTurn, RotateTexture(turnRoadTexture, 270.0) },
                
                // Cross T
                { RoadType.DownTRoad, crossTRoadTexture },
                { RoadType.LeftTRoad, RotateTexture(crossTRoadTexture, 90.0) },
                { RoadType.UpperTRoad, RotateTexture(crossTRoadTexture, 180.0) },
                { RoadType.RightTRoad, RotateTexture(crossTRoadTexture, 270.0) },
                
                // Cross X
                { RoadType.XRoad, crossXRoadTexture },
            };

            var greenBridgeTexture = LoadTexture(new Uri("pack://application:,,,/Assets/Images/Bridge/greenBridge.png"));
            var redBridgeTexture = LoadTexture(new Uri("pack://application:,,,/Assets/Images/Bridge/redBridge.png"));
            var yellowBridgeTexture = LoadTexture(new Uri("pack://application:,,,/Assets/Images/Bridge/yellowBridge.png"));
            _bridgeTextures = new()
            {
                { BridgeType.VerticalGreenBridge, greenBridgeTexture },
                { BridgeType.VerticalRedBridge, redBridgeTexture },
                { BridgeType.VerticalYellowBridge, yellowBridgeTexture },

                { BridgeType.HorizontalGreenBridge, RotateTexture(greenBridgeTexture, 90.0) },
                { BridgeType.HorizontalRedBridge, RotateTexture(redBridgeTexture, 90.0) },
                { BridgeType.HorizontalYellowBridge, RotateTexture(yellowBridgeTexture, 90.0) },
            };

            _vehicleTextures = new()
            {
                { VehicleType.SmallBus, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Vehicle/smallBus.png")) },
                { VehicleType.BigBus, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Vehicle/largeBus.png")) },
                { VehicleType.Pickup, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Vehicle/pickup.png"))},
                { VehicleType.Van, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Vehicle/van.png"))},
                { VehicleType.Truck, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Vehicle/truck.png"))},
                { VehicleType.LiquidTruck, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Vehicle/liquidTruck.png"))},
            };
        }
        #endregion

        #region Private methods
        #region Helper methods
        /// <summary>
        /// Loads an image from an <see cref="Uri"/> and prepares it for rendering as a tile.
        /// Forces the size to <see cref="TileSize"/> x <see cref="TileSize"/> and freezes the bitmap for performance."/>
        /// </summary>
        /// <param name="uri">The URI to load the picture from.</param>
        /// <returns>The loaded <see cref="BitmapSource"/> object.</returns>
        private BitmapSource LoadTexture(Uri uri)
        {
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = uri;
            bitmap.DecodePixelWidth = TileSize;
            bitmap.DecodePixelHeight = TileSize;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        /// <summary>
        /// Rotates a given <see cref="BitmapSource"/> by the specified angle and returns the rotated image as a new <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="source">The source <see cref="BitmapSource"/> to rotate.</param>
        /// <param name="angle">The angle in degrees to rotate the image.</param>
        /// <returns>A new <see cref="BitmapSource"/> representing the rotated image.</returns>
        private BitmapSource RotateTexture(BitmapSource source, double angle)
        {
            var rotated = new TransformedBitmap(source, new RotateTransform(angle));
            rotated.Freeze();
            return rotated;
        }

        /// <summary>
        /// Generate a texture for a route stop tile with the given order number.
        /// </summary>
        /// <param name="order">The order number of the route stop.</param>
        /// <returns>A <see cref="BitmapSource"/> representing the route stop tile.</returns>
        private BitmapSource GenerateRouteStopTexture(int order)
        {
            // Standard WPF DPI
            const double DPI = 96.0;
            DrawingVisual visual = new();

            using (DrawingContext ctx = visual.RenderOpen())
            {
                Rect tileRect = new(0, 0, TileSize, TileSize);

                ctx.DrawRectangle(null, _stopBorderPen, tileRect);

                FormattedText text = new(
                    textToFormat: order.ToString(),
                    culture: System.Globalization.CultureInfo.CurrentCulture,
                    flowDirection: FlowDirection.LeftToRight,
                    typeface: _stopTextTypeface,
                    emSize: 24,
                    foreground: _stopTextBrush,
                    pixelsPerDip: DPI / 96.0);

                // Center the text
                double textX = (TileSize / 2.0) - (text.Width / 2.0);
                double textY = (TileSize / 2.0) - (text.Height / 2.0);

                ctx.DrawText(text, new Point(textX, textY));
            }

            RenderTargetBitmap bmp = new(
                TileSize,
                TileSize,
                DPI,
                DPI,
                PixelFormats.Pbgra32
                );

            bmp.Render(visual);
            bmp.Freeze();

            return bmp;
        }

        /// <summary>
        /// Generate textures for route stop tiles with order numbers in the given range and cache them.
        /// </summary>
        private void GenerateRouteStopTextures(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                _stopTextures[i] = GenerateRouteStopTexture(i);
            }
        }

        #region Enum converters
        /// <summary>
        /// Converts a <see cref="MapData.FieldType"/> to the corresponding <see cref="FieldType"/> for texture lookup.
        /// </summary>
        /// <remarks>
        /// This method uses AggriessiveInlining to ensure that the conversion is as fast as possible, since it may be called frequently during rendering.
        /// </remarks>
        /// <param name="type">The <see cref="MapData.FieldType"/> to convert.</param>
        /// <returns>The corresponding <see cref="FieldType"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown if the <paramref name="type"/> is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FieldType? ConvertFieldType(IField type)
        {
            return type switch
            {
                House => FieldType.House,
                Farm => FieldType.Farm,
                Mine => FieldType.Mine,
                LumberCamp => FieldType.LumberCamp,
                Mill => FieldType.Mill,
                Factory => FieldType.Factory,
                Plant => FieldType.Plant,
                Road => FieldType.Road,
                Stop => FieldType.Stop,
                IBridge => FieldType.Bridge,
                IField => null,
                _ => throw new NotImplementedException($"Unsupported field type: {type}"),
            };
        }

        /// <summary>
        /// Converts a <see cref="Model.VehicleType"/>  to the corresponding <see cref="VehicleType"/> for texture lookup.
        /// </summary>
        /// <remarks>
        /// This method uses AggriessiveInlining to ensure that the conversion is as fast as possible, since it may be called frequently during rendering.
        /// </remarks>
        /// <param name="type">The <see cref="Model.VehicleType"/> to convert.</param>
        /// <returns>The corresponding <see cref="VehicleType"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown if the <paramref name="type"/> is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private VehicleType ConvertVehicleType(Model.VehicleType type)
        {
            return type switch
            {
                Model.VehicleType.Van => VehicleType.Van,
                Model.VehicleType.Pickup => VehicleType.Pickup,
                Model.VehicleType.Truck => VehicleType.Truck,
                Model.VehicleType.LiquidTruck => VehicleType.LiquidTruck,
                Model.VehicleType.SmallBus => VehicleType.SmallBus,
                Model.VehicleType.BigBus => VehicleType.BigBus,
                _ => throw new NotImplementedException($"Unsupported vehicle type: {type}"),
            };
        }

        /// <summary>
        /// Converts a <see cref="MapData.RoadType"/>  to the corresponding <see cref="RoadType"/> for texture lookup.
        /// </summary>
        /// <remarks>
        /// This method uses AggriessiveInlining to ensure that the conversion is as fast as possible, since it may be called frequently during rendering.
        /// </remarks>
        /// <param name="type">The <see cref="MapData.RoadType"/> to convert.</param>
        /// <returns>The corresponding <see cref="RoadType"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown if the <paramref name="type"/> is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RoadType ConvertRoadType(MapData.RoadType type)
        {
            return type switch
            {
                MapData.RoadType.Horizontal => RoadType.Horizontal,
                MapData.RoadType.Vertical => RoadType.Vertical,
                MapData.RoadType.RightTurn => RoadType.RightTurn,
                MapData.RoadType.LeftTurn => RoadType.LeftTurn,
                MapData.RoadType.UpperRightTurn => RoadType.UpperRightTurn,
                MapData.RoadType.UpperLeftTurn => RoadType.UpperLeftTurn,
                MapData.RoadType.UpperTRoad => RoadType.UpperTRoad,
                MapData.RoadType.DownTRoad => RoadType.DownTRoad,
                MapData.RoadType.RightTRoad => RoadType.RightTRoad,
                MapData.RoadType.LeftTRoad => RoadType.LeftTRoad,
                MapData.RoadType.XRoad => RoadType.XRoad,
                _ => throw new NotImplementedException($"Unsupported road type: {type}"),
            };
        }

        /// <summary>
        /// Converts a <see cref="MapData.BridgeType"/>  to the corresponding <see cref="BridgeType"/> for texture lookup.
        /// </summary>
        /// <remarks>
        /// This method uses AggriessiveInlining to ensure that the conversion is as fast as possible, since it may be called frequently during rendering.
        /// </remarks>
        /// <param name="type">The <see cref="MapData.BridgeType"/> to convert.</param>
        /// <returns>The corresponding <see cref="BridgeType"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown if the <paramref name="type"/> is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BridgeType ConvertBridgeType(MapData.BridgeType type)
        {
            return type switch
            {
                MapData.BridgeType.HorizontalGreenBridge => BridgeType.HorizontalGreenBridge,
                MapData.BridgeType.VerticalGreenBridge => BridgeType.VerticalGreenBridge,
                MapData.BridgeType.HorizontalYellowBridge => BridgeType.HorizontalYellowBridge,
                MapData.BridgeType.VerticalYellowBridge => BridgeType.VerticalYellowBridge,
                MapData.BridgeType.HorizontalRedBridge => BridgeType.HorizontalRedBridge,
                MapData.BridgeType.VerticalRedBridge => BridgeType.VerticalRedBridge,
                MapData.BridgeType.Null => BridgeType.Null,
                _ => throw new NotImplementedException($"Unsupported bridge type: {type}"),
            };
        }
        #endregion
        #endregion

        #region Layer drawing methods
        /// <summary>
        /// Helper method to draw the terrain layer of a tile.
        /// </summary>
        /// <remarks>
        /// Rendering remains efficient to JIT inlining.
        /// </remarks>
        /// <param name="ctx">The <see cref="DrawingContext"/> object, on which the images appears.</param>
        /// <param name="field">The <see cref="IField"/> object, which we want to draw.</param>
        /// <param name="baseRect">The <see cref="Rect"/> rectangle object, that tells where we draw the image on <see cref="DrawingContext"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawTerrainLayer(DrawingContext ctx, IField field, Rect baseRect)
        {
            if (_terrainTextures.TryGetValue((FieldType)field.Height, out var texture))
            {
                ctx.DrawImage(texture, baseRect);
            }
        }

        /// <summary>
        /// Helper method to draw the structure layer of a tile.
        /// </summary>
        /// <remarks>
        /// Rendering remains efficient to JIT inlining.
        /// </remarks>
        /// <param name="ctx">The <see cref="DrawingContext"/> object, on which the images appears.</param>
        /// <param name="field">The <see cref="IField"/> object, which we want to draw.</param>
        /// <param name="baseRect">The <see cref="Rect"/> rectangle object, that tells where we draw the image on <see cref="DrawingContext"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawStructureLayer(DrawingContext ctx, IField field, Rect baseRect)
        {
            if (ConvertFieldType(field) is FieldType fieldType && _structureTextures.TryGetValue(fieldType, out var texture))
            {
                ctx.DrawImage(texture, baseRect);
            }
        }

        /// <summary>
        /// Helper method to draw the road layer of a tile.
        /// </summary>
        /// <remarks>
        /// Rendering remains efficient to JIT inlining.
        /// </remarks>
        /// <param name="ctx">The <see cref="DrawingContext"/> object, on which the images appears.</param>
        /// <param name="field">The <see cref="IField"/> object, that contains the road and it's rotataion.</param>
        /// <param name="baseRect">The <see cref="Rect"/> rectangle object, that tells where we draw the image on <see cref="DrawingContext"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawRoadLayer(DrawingContext ctx, IField field, Rect baseRect)
        {
            if (field is not null &&
                ConvertFieldType(field) == FieldType.Road &&
                field is Road road &&
                _roadTextures.TryGetValue(ConvertRoadType(road.RoadType), out var texture))
            {
                ctx.DrawImage(texture, baseRect);
            }
        }

        /// <summary>
        /// Helper method to draw the bridge layer of a tile.
        /// </summary>
        /// <remarks>
        /// Rendering remains efficient to JIT inlining.
        /// </remarks>
        /// <param name="ctx">The <see cref="DrawingContext"/> object, on which the images appears.</param>
        /// <param name="field">The <see cref="IField"/> object, that contains the bridge and it's rotation.</param>
        /// <param name="baseRect">The <see cref="Rect"/> rectangle object, that tells where we draw the image on <see cref="DrawingContext"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawBridgeLayer(DrawingContext ctx, IField field, Rect baseRect)
        {
            if (field is not null &&
                ConvertFieldType(field) == FieldType.Bridge &&
                field is IBridge bridge
                && _bridgeTextures.TryGetValue(ConvertBridgeType(bridge.BridgeType), out var texture))
            {
                ctx.DrawImage(texture, baseRect);
            }
        }

        /// <summary>
        /// Helper method to draw the tree layer of a tile.
        /// </summary>
        /// <remarks>
        /// Rendering remains efficient to JIT inlining.
        /// This should be one the last layers to draw, since trees can be on top of roads and structures.
        /// </remarks>
        /// <param name="ctx">The <see cref="DrawingContext"/> object, on which the images appears.</param>
        /// <param name="field">The <see cref="IField"/> object, that contains the tree count.</param>
        /// <param name="baseRect">The <see cref="Rect"/> rectangle object, that tells where we draw the image on <see cref="DrawingContext"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawTreesLayer(DrawingContext ctx, IField field, Rect baseRect)
        {
            if (field.GetTrees() > 0 && _treesTextures.TryGetValue(field.GetTrees(), out var texture))
            {
                ctx.DrawImage(texture, baseRect);
            }
        }

        /// <summary>
        /// Helper method to draw the vehicle layer of a tile.
        /// </summary>
        /// <remarks>
        /// Rendering remains efficient to JIT inlining.
        /// </remarks>
        /// <param name="ctx">The <see cref="DrawingContext"/> object, on which the images appears.</param>
        /// <param name="visibleWorldRect">The visible area of the world, used for culling.</param>
        private void DrawVehiclesLayer(DrawingContext ctx, Rect visibleWorldRect)
        {
            if (Vehicles is null) return;

            const double LANE_OFFSET_PIXEL = 10.0;

            foreach (Vehicle vehicle in Vehicles)
            {
                double angle = vehicle.Angle;

                double rightAngleRad = (angle + 90.0) * (Math.PI / 180.0);

                double shiftX = Math.Cos(rightAngleRad) * LANE_OFFSET_PIXEL;
                double shiftY = Math.Sin(rightAngleRad) * LANE_OFFSET_PIXEL;

                double pixelX = (vehicle.X * TileSize) + shiftX;
                double pixelY = (vehicle.Y * TileSize) + shiftY;

                Rect vehicleRect = new(pixelX, pixelY, TileSize, TileSize);

                // Culling check
                if (!visibleWorldRect.IntersectsWith(vehicleRect)) continue;

                if (_vehicleTextures.TryGetValue(ConvertVehicleType(vehicle.Type), out var texture))
                {
                    // Calculate the rotation center, match the size to the given rectangle
                    double centerX = vehicleRect.X + (vehicleRect.Width / 2.0);
                    double centerY = vehicleRect.Y + (vehicleRect.Height / 2.0);

                    if (!_vehicleRotationCache.TryGetValue(vehicle.Id, out var transform))
                    {
                        transform = new RotateTransform(angle + 90, centerX, centerY);
                        _vehicleRotationCache[vehicle.Id] = transform;
                    }

                    transform.CenterX = centerX;
                    transform.CenterY = centerY;
                    transform.Angle = angle + 90;

                    ctx.PushTransform(transform);

                    ctx.DrawImage(texture, vehicleRect);

                    ctx.Pop();
                }
            }
        }

        /// <summary>
        /// Helper method to add hover effect on a tile
        /// </summary>
        /// <remarks>
        /// Rendering remains efficient to JIT inlining.
        /// </remarks>
        /// <param name="ctx">The <see cref="DrawingContext"/> object, on which the images appears.</param>
        /// <param name="baseRect">The <see cref="Rect"/> rectangle object, that tells where we draw the image on <see cref="DrawingContext"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddHoverEffectLayer(DrawingContext ctx, Rect baseRect)
        {
            // Don't modify outer state (maybe it is not needed because Rect is a struct)
            Rect hoverRect = baseRect;
            double penThickness = _highlightPen.Thickness / 2.0;
            hoverRect.Inflate(-penThickness, -penThickness);
            ctx.DrawRectangle(_highlightBrush, _highlightPen, hoverRect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="baseRect"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddSelectionEffectLayer(DrawingContext ctx, Rect baseRect)
        {
            Rect selectionRect = baseRect;
            double penThickness = _selectionPen.Thickness / 2.0;
            selectionRect.Inflate(-penThickness, -penThickness);
            ctx.DrawRectangle(null, _selectionPen, selectionRect);
        }

        /// <summary>
        /// Draws the route stops layer on the given <see cref="DrawingContext"/>.
        /// </summary>
        /// <param name="ctx">The <see cref="DrawingContext"/> object, on which the images appears.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawRouteStopsLayer(DrawingContext ctx)
        {
            if (RouteStops is null) return;

            foreach (var stop in RouteStops)
            {
                double pixelX = stop.X * TileSize;
                double pixelY = stop.Y * TileSize;
                Rect stopRect = new(pixelX, pixelY, TileSize, TileSize);

                // The text
                if (_stopTextures.TryGetValue(stop.Order, out BitmapSource? texture))
                {
                    ctx.DrawImage(texture, stopRect);
                }
                else
                {
                    // Cache new ones
                    BitmapSource generatedTexture = GenerateRouteStopTexture(stop.Order);
                    _stopTextures[stop.Order] = generatedTexture;
                    ctx.DrawImage(generatedTexture, stopRect);
                    Debug.WriteLine($"Cached a new image with order: {stop.Order}!");
                }
            }
        }
        #endregion
        #endregion

        #region Protected methods
        /// <summary>
        /// The render action that WPF calls whenever the control needs to be redrawn.
        /// It loops through the map and draws each tile at the correct position.
        /// </summary>
        protected override void OnRender(DrawingContext ctx)
        {
            base.OnRender(ctx);
            // Cache the map reference for performance
            IField[,] currentMap = Map;
            if (currentMap is null) return;

            int width = currentMap.GetLength(0);
            int height = currentMap.GetLength(1);

            // GPU transformations for zooming and panning
            ctx.PushTransform(_cameraTransformGroup);

            double visibleWorldWidth = ActualWidth / ZoomLevel;
            double visibleWorldHeight = ActualHeight / ZoomLevel;

            int startCol = Math.Max(0, (int)(CameraX / TileSize));
            int startRow = Math.Max(0, (int)(CameraY / TileSize));

            int endCol = Math.Min(width, (int)((CameraX + visibleWorldWidth) / TileSize) + 1);
            int endRow = Math.Min(height, (int)((CameraY + visibleWorldHeight) / TileSize) + 1);

            for (int y = startRow; y < endRow; y++)
            {
                for (int x = startCol; x < endCol; x++)
                {
                    IField currentField = currentMap[x, y];
                    Rect baseRect = new(x * TileSize, y * TileSize, TileSize, TileSize);
                    DrawTerrainLayer(ctx, currentField, baseRect);
                    DrawStructureLayer(ctx, currentField, baseRect);
                    DrawRoadLayer(ctx, currentField, baseRect);
                    DrawBridgeLayer(ctx, currentField, baseRect);
                    DrawTreesLayer(ctx, currentField, baseRect);

                    // Hover effect
                    if (x == HoverX && y == HoverY)
                    {
                        AddHoverEffectLayer(ctx, baseRect);
                    }

                    // Selection effect
                    if (currentField.X == SelectedTile?.X && currentField.Y == SelectedTile?.Y)
                    {
                        AddSelectionEffectLayer(ctx, baseRect);
                    }
                }
            }
            Rect visibleWorldRect = new(CameraX, CameraY, visibleWorldWidth, visibleWorldHeight);

            // Vehicle layer
            DrawVehiclesLayer(ctx, visibleWorldRect);

            // Stop order layer
            DrawRouteStopsLayer(ctx);

            ctx.Pop();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Removes the cached rotation for a vehicle with the given ID, if it exists.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle whose cached rotation should be removed.</param>
        public void RemoveVehicleFromCache(UInt64 vehicleId)
        {
            _vehicleRotationCache.Remove(vehicleId);
        }

        public void SetCameraView(double desiredX, double desiredY, double desiredZoom)
        {
            if (Map is null || ActualWidth <= 0.0 || ActualHeight <= 0.0) return;

            const double MAX_ZOOM = 4.0;

            double totalWorldWidth = Map.GetLength(0) * TileSize;
            double totalWorldHeight = Map.GetLength(1) * TileSize;

            double minZoomX = ActualWidth / totalWorldWidth;
            double minZoomY = ActualHeight / totalWorldHeight;

            double dynamicMinZoom = Math.Max(minZoomX, minZoomY);

            ZoomLevel = Math.Clamp(desiredZoom, dynamicMinZoom, MAX_ZOOM);

            double visibleWorldWidth = ActualWidth / ZoomLevel;
            double visibleWorldHeight = ActualHeight / ZoomLevel;

            double maxCameraX = Math.Max(0, (Map.GetLength(0) * TileSize) - visibleWorldWidth);
            double maxCameraY = Math.Max(0, (Map.GetLength(1) * TileSize) - visibleWorldHeight);

            CameraX = Math.Clamp(desiredX, 0, maxCameraX);
            CameraY = Math.Clamp(desiredY, 0, maxCameraY);

            _cameraTransform.X = -CameraX;
            _cameraTransform.Y = -CameraY;

            _zoomTransform.ScaleX = ZoomLevel;
            _zoomTransform.ScaleY = ZoomLevel;
        }

        /// <summary>
        /// This method forces a redraw.
        /// </summary>
        public void Redraw()
        {
            InvalidateVisual();
        }
        #endregion
    }
}
