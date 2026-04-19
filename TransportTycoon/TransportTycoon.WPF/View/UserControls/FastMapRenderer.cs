using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TransportTycoon.MapData;
using TransportTycoon.Model;
using TransportTycoon.WPF.Utils;

namespace TransportTycoon.WPF.View.UserControls
{
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
        private readonly Dictionary<FieldType, BitmapImage> _terrainTextures;

        /// <summary>
        /// A dictionary that link each <see cref="FieldType"/> to their structure image.
        /// </summary>
        /// <remarks>Must be set manually for each <see cref="FieldType"/>.</remarks>
        private readonly Dictionary<FieldType, BitmapImage> _structureTextures;

        /// <summary>
        /// A dictionary that link each <see cref="RoadType"/> to their road image.
        /// </summary>
        /// <remarks>Must be set manually for each <see cref="RoadType"/>.</remarks>
        private readonly Dictionary<string, BitmapImage> _roadTextures;

        /// <summary>
        /// A dictionary that link each <see cref="BridgeType"/> to their bridge image.
        /// </summary>
        /// <remarks>Must be set manually for each <see cref="BridgeType"/>.</remarks>
        private readonly Dictionary<string, BitmapImage> _bridgeTextures;

        /// <summary>
        /// A dictionary that link integers 1 to 4 to their tree image.
        /// </summary>
        /// <remarks>Must be set manually for each tree type.</remarks>
        private readonly Dictionary<int, BitmapImage> _treesTextures;

        /// <summary>
        /// A dictionary that link each vehicle type to their image.
        /// </summary>
        /// <remarks>Must be set manually for each vehicle type. Not currently implemented.</remarks>
        private readonly Dictionary<VehicleType, BitmapImage> _vehicleTextures;

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
        public static readonly DependencyProperty SelectedXProperty =
            DependencyProperty.Register(
                nameof(SelectedX),
                typeof(int),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedYProperty =
            DependencyProperty.Register(
                nameof(SelectedY),
                typeof(int),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender));

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
        /// The underlying property for <see cref="SelectedXProperty"/>.
        /// </summary>
        public int SelectedX
        {
            get => (int)GetValue(SelectedXProperty);
            set => SetValue(SelectedXProperty, value);
        }

        /// <summary>
        /// The underlying property for <see cref="SelectedYProperty"/>.
        /// </summary>
        public int SelectedY
        {
            get => (int)GetValue(SelectedYProperty);
            set => SetValue(SelectedYProperty, value);
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

            // Generate the stop textures
#pragma warning disable IDE0028 // Simplify collection initialization
            _stopTextures = new(20);
#pragma warning restore IDE0028 // Simplify collection initialization
            GenerateRouteStopTextures(0, 20);

            // TODO: Later maybe JSON or .rex format
            _terrainTextures = new Dictionary<FieldType, BitmapImage>
            {
                { FieldType.Water, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/water2.png")) },
                { FieldType.Plain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/plain.png")) },
                { FieldType.Hill, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/hill.png")) },
                { FieldType.Mountain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/mountain.png")) },
                { FieldType.HighMountain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/highmountain.png")) },
            };

            _structureTextures = new Dictionary<FieldType, BitmapImage>
            {
                { FieldType.Farm, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/farm.png")) },
                { FieldType.LumberCamp, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/lumbercamp.png")) },
                { FieldType.Mine, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/oil.jpg")) },
                { FieldType.Mill, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/mill.png")) },
                { FieldType.Plant, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/rubber.jpg")) },
                { FieldType.Factory, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/factory.png")) },
                { FieldType.House, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/house.jpg")) },
                { FieldType.Stop, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Stop/stop.png")) },
            };

            _treesTextures = new Dictionary<int, BitmapImage>
            {
                {1, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Trees/tree1.png"))  },
                {2, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Trees/tree2.png")) },
                {3, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Trees/tree3.png")) },
                {4, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Trees/tree4.png")) },
            };

            _roadTextures = new Dictionary<string, BitmapImage>
            {
                { "crossX", LoadTexture(new Uri("pack://application:,,,/Assets/Images/Road/crossX.png")) },
                { "crossT", LoadTexture(new Uri("pack://application:,,,/Assets/Images/Road/crossT.png")) },
                { "road", LoadTexture(new Uri("pack://application:,,,/Assets/Images/Road/road.png")) },
                { "turn", LoadTexture(new Uri("pack://application:,,,/Assets/Images/Road/turn.png")) },
            };

            _bridgeTextures = new Dictionary<string, BitmapImage>
            {
                { "green", LoadTexture(new Uri("pack://application:,,,/Assets/Images/Bridge/greenBridge.png")) },
                { "red", LoadTexture(new Uri("pack://application:,,,/Assets/Images/Bridge/redBridge.png")) },
                { "yellow", LoadTexture(new Uri("pack://application:,,,/Assets/Images/Bridge/yellowBridge.png")) },
            };

            _vehicleTextures = new Dictionary<VehicleType, BitmapImage>
            {
                { VehicleType.SmallBus, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Vehicle/smallBus.png")) },
                { VehicleType.BigBus, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Vehicle/largeBus.png")) },
            };
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Loads an image from an <see cref="Uri"/> and prepares it for rendering as a tile.
        /// Forces the size to <see cref="TileSize"/> x <see cref="TileSize"/> and freezes the bitmap for performance."/>
        /// </summary>
        /// <param name="uri">The URI to load the picture from.</param>
        /// <returns>The loaded <see cref="BitmapImage"/> object.</returns>
        private BitmapImage LoadTexture(Uri uri)
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
            if (_terrainTextures.TryGetValue((FieldType)field.Height, out BitmapImage? texture))
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
            if (_structureTextures.TryGetValue(field.FieldType, out BitmapImage? texture))
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
            if (field is not null && field.FieldType == FieldType.Road && field is Road road)
            {
                string roadType = "road";
                if (road.RoadType == RoadType.RightTurn || road.RoadType == RoadType.LeftTurn || road.RoadType == RoadType.UpperRightTurn || road.RoadType == RoadType.UpperLeftTurn)
                    roadType = "turn";
                else if (road.RoadType == RoadType.UpperTRoad || road.RoadType == RoadType.RightTRoad || road.RoadType == RoadType.DownTRoad || road.RoadType == RoadType.LeftTRoad)
                    roadType = "crossT";
                else if (road.RoadType == RoadType.XRoad)
                    roadType = "crossX";
                if (_roadTextures.TryGetValue(roadType, out BitmapImage? texture))
                {
                    int rotaion = road.RoadType switch
                    {
                        RoadType.Vertical or RoadType.UpperTRoad or RoadType.UpperRightTurn => 90,
                        RoadType.LeftTRoad or RoadType.UpperLeftTurn => 180,
                        RoadType.DownTRoad or RoadType.LeftTurn => 270,
                        _ => 0
                    };
                    if (rotaion != 0)
                    {
                        // Calculate the rotaion center, match the size to the given rectangle
                        double centerX = baseRect.X + (baseRect.Width / 2);
                        double centerY = baseRect.Y + (baseRect.Height / 2);
                        // Add the rotation
                        ctx.PushTransform(new RotateTransform(rotaion, centerX, centerY));
                        ctx.DrawImage(texture, baseRect);
                        // Remove the rotation
                        ctx.Pop();
                    }
                    else
                    {
                        ctx.DrawImage(texture, baseRect);
                    }
                }
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
            if (field is not null && field.FieldType == FieldType.Bridge && field is IBridge bridge)
            {
                string? bridgeType = bridge.BridgeType switch
                {
                    BridgeType.VerticalYellowBridge or BridgeType.HorizontalYellowBridge => "green",
                    BridgeType.VerticalGreenBridge or BridgeType.HorizontalGreenBridge => "red",
                    BridgeType.VerticalRedBridge or BridgeType.HorizontalRedBridge => "yellow",
                    _ => null
                };

                if (bridgeType is not null && _bridgeTextures.TryGetValue(bridgeType, out BitmapImage? texture))
                {
                    if (bridge.BridgeType == BridgeType.HorizontalGreenBridge
                        || bridge.BridgeType == BridgeType.HorizontalYellowBridge
                        || bridge.BridgeType == BridgeType.HorizontalRedBridge)
                    {
                        // Calculate the rotation center, match the size to the given rectangle
                        double centerX = baseRect.X + (baseRect.Width / 2);
                        double centerY = baseRect.Y + (baseRect.Height / 2);
                        // Add the rotation
                        ctx.PushTransform(new RotateTransform(90, centerX, centerY));
                        ctx.DrawImage(texture, baseRect);
                        // Remove the rotation
                        ctx.Pop();
                    }
                    else
                    {
                        ctx.DrawImage(texture, baseRect);
                    }
                }
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
            if (field.GetTrees() > 0 && _treesTextures.TryGetValue(field.GetTrees(), out BitmapImage? texture))
            {
                ctx.DrawImage(texture, baseRect);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        private void DrawVehiclesLayer(DrawingContext ctx, Rect visibleWorldRect)
        {
            if (Vehicles == null) return;

            foreach (Vehicle vehicle in Vehicles)
            {
                Rect vehicleRect = new(vehicle.X * TileSize, vehicle.Y * TileSize, TileSize, TileSize);

                // Culling check
                if (!visibleWorldRect.IntersectsWith(vehicleRect)) return;

                if (_vehicleTextures.TryGetValue(vehicle.Type, out BitmapImage? texture))
                {
                    int rotation = vehicle.Direction switch
                    {
                        Direction.Up => 0,
                        Direction.Down => 180,
                        Direction.Right => 90,
                        Direction.Left => 270,
                        _ => throw new NotImplementedException(),
                    };

                    // Calculate the rotation center, match the size to the given rectangle
                    double centerX = vehicleRect.X + (vehicleRect.Width / 2);
                    double centerY = vehicleRect.Y + (vehicleRect.Height / 2);
                    ctx.PushTransform(new RotateTransform(rotation, centerX, centerY));

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
        private void AddSelectionEffectLayer(DrawingContext ctx, Rect baseRect)
        {
            Rect selectionRect = baseRect;
            double penThickness = _selectionPen.Thickness / 2.0;
            selectionRect.Inflate(-penThickness, -penThickness);
            ctx.DrawRectangle(null, _selectionPen, selectionRect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        private void DrawRouteStopsLayer(DrawingContext ctx)
        {
            if (RouteStops == null) return;

            //Debug.WriteLineIf(RouteStops.Count() == 0, "Empty stops");

            foreach (var stop in RouteStops)
            {
                //Debug.WriteLine("Teszt");
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
                }
            }
        }

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
        /// 
        /// </summary>
        private void GenerateRouteStopTextures(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                _stopTextures[i] = GenerateRouteStopTexture(i);
            }
        }
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
            ctx.PushTransform(new ScaleTransform(ZoomLevel, ZoomLevel));
            ctx.PushTransform(new TranslateTransform(-CameraX, -CameraY));

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

                    DrawRouteStopsLayer(ctx);

                    // Hover effect
                    if (x == HoverX && y == HoverY)
                    {
                        AddHoverEffectLayer(ctx, baseRect);
                    }

                    // Selection effect
                    if (x == SelectedX && y == SelectedY)
                    {
                        AddSelectionEffectLayer(ctx, baseRect);
                    }
                }
            }

            Rect visibleWorldRect = new(CameraX, CameraY, visibleWorldWidth, visibleWorldHeight);

            // Vehicle layer
            DrawVehiclesLayer(ctx, visibleWorldRect);

            ctx.Pop();
            ctx.Pop();
        }
        #endregion

        #region Public methods
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
