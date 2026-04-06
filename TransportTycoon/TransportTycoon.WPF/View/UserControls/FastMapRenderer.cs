using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TransportTycoon.MapData;

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
        private readonly Dictionary<object, BitmapImage> _vehicleTextures;

        /// <summary>
        /// A cached brush for highlighting the hovered tile.
        /// </summary>
        private readonly Brush _highlightBrush = new SolidColorBrush(Color.FromArgb(100, 0, 150, 255));
        #endregion

        #region Bindings
        /// <summary>
        /// A binding for the map tiles.
        /// </summary>
        /// <remarks>Since views can't access viewmodels we need to bind it.</remarks>
        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register(
                nameof(Map),
                typeof(Field[,]),
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
        /// By default it is set to -1, which means out of bounds and thus no tile is hovered.
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
        /// By default it is set to -1, which means out of bounds and thus no tile is hovered. 
        /// </summary>
        /// <remarks>Every time this value changes it causes a rerender to take effect.</remarks>
        public static readonly DependencyProperty HoverYProperty =
            DependencyProperty.Register(
                nameof(HoverY),
                typeof(int),
                typeof(FastMapRenderer),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Properties
        /// <summary>
        /// The underlying property for <see cref="MapProperty"/>.
        /// </summary>
        public Field[,] Map
        {
            get => (Field[,])GetValue(MapProperty);
            set
            {
                SetValue(MapProperty, value);
            }
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
        #endregion

        #region Constructors
        public FastMapRenderer()
        {
            // Freeze the brush (just like with images)
            _highlightBrush.Freeze();

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

            _vehicleTextures = [];
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
        /// <remarks>Rendering remains efficient to JIT inlining.</remarks>
        /// <param name="ctx">The <see cref="DrawingContext"/> object, where we draw the image.</param>
        /// <param name="field">The <see cref="Field"/> object, which we want to draw</param>
        /// <param name="baseRect">The <see cref="Rect"/> rectangle object where we draw the image</param>
        private void DrawTerrainLayer(DrawingContext ctx, Field field, Rect baseRect)
        {
            if (_terrainTextures.TryGetValue((FieldType)field.Height, out BitmapImage? texture))
            {
                ctx.DrawImage(texture, baseRect);
            }
        }

        private void DrawStructureLayer(DrawingContext ctx, Field field, Rect baseRect)
        {
            if (_structureTextures.TryGetValue(field.FieldType, out BitmapImage? texture))
            {
                ctx.DrawImage(texture, baseRect);
            }
        }

        private void DrawRoadLayer(DrawingContext ctx, Field field, Rect baseRect)
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

        private void DrawBridgeLayer(DrawingContext ctx, Field field, Rect baseRect)
        {
            if (field is not null && field.FieldType == FieldType.Bridge && field is Bridge bridge)
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
                    int rotaion = bridge.BridgeType switch
                    {
                        BridgeType.HorizontalGreenBridge or BridgeType.HorizontalYellowBridge or BridgeType.HorizontalRedBridge => 0,
                        _ => 90
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

        private void DrawTreesLayer(DrawingContext ctx, Field field, Rect baseRect)
        {
            if (field.GetTrees() > 0 && _treesTextures.TryGetValue(field.GetTrees(), out BitmapImage? texture))
            {
                ctx.DrawImage(texture, baseRect);
            }
        }

        private void AddHoverEffectLayer(DrawingContext ctx, Rect baseRect)
        {
            ctx.DrawRectangle(_highlightBrush, null, baseRect);
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// The render action that WPF calls whenever the control needs to be redrawn.
        /// It loops through the map and draws each tile at the correct position.
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            // Cache the map reference for performance
            Field[,] currentMap = this.Map;
            if (Map == null) return;

            int width = currentMap.GetLength(0);
            int height = currentMap.GetLength(1);

            // GPU transformations for zooming and panning
            drawingContext.PushTransform(new ScaleTransform(ZoomLevel, ZoomLevel));
            drawingContext.PushTransform(new TranslateTransform(-CameraX, -CameraY));

            double visibleWorldWidth = this.ActualWidth / ZoomLevel;
            double visibleWorldHeight = this.ActualHeight / ZoomLevel;

            int startCol = Math.Max(0, (int)(CameraX / TileSize));
            int startRow = Math.Max(0, (int)(CameraY / TileSize));

            int endCol = Math.Min(width, (int)((CameraX + visibleWorldWidth) / TileSize) + 1);
            int endRow = Math.Min(height, (int)((CameraY + visibleWorldHeight) / TileSize) + 1);

            for (int y = startRow; y < endRow; y++)
            {
                for (int x = startCol; x < endCol; x++)
                {
                    Field currentField = currentMap[x, y];
                    Rect baseRect = new(x * TileSize, y * TileSize, TileSize, TileSize);
                    DrawTerrainLayer(drawingContext, currentField, baseRect);
                    DrawStructureLayer(drawingContext, currentField, baseRect);
                    DrawRoadLayer(drawingContext, currentField, baseRect);
                    DrawBridgeLayer(drawingContext, currentField, baseRect);
                    DrawTreesLayer(drawingContext, currentField, baseRect);

                    // Hover effect
                    if (x == HoverX && y == HoverY)
                    {
                        AddHoverEffectLayer(drawingContext, baseRect);
                    }
                }
            }

            drawingContext.Pop();
            drawingContext.Pop();
        }
        #endregion

        #region Public methods
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
