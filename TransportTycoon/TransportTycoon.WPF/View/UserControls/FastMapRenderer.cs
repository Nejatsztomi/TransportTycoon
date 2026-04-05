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
        /// A dictionary that link each <see cref="FieldType"/> to their corresponding image.
        /// </summary>
        /// <remarks>Must be set manually for each <see cref="FieldType"/>.</remarks>
        private readonly Dictionary<FieldType, BitmapImage> _textures;
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
        #endregion

        #region Constructors
        public FastMapRenderer()
        {
            // TODO: Later maybe JSON or .rex format
            _textures = new Dictionary<FieldType, BitmapImage>
            {
                // Terrain
                { FieldType.Water, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/water2.png")) },
                { FieldType.Plain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/plain.png")) },
                { FieldType.Hill, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/hill.png")) },
                { FieldType.Mountain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/mountain.png")) },
                { FieldType.HighMountain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/highmountain.png")) },
                // Structures
                { FieldType.Farm, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/farm.png"))  },
                { FieldType.LumberCamp, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/lumbercamp.png"))  },
                { FieldType.Mine, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/oil.jpg"))  },
                { FieldType.Mill, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/mill.png"))  },
                { FieldType.Plant, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/rubber.jpg"))  },
                { FieldType.Factory, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/factory.png"))  },
                {  FieldType.House, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Structures/house.jpg"))   },
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

            for (int x = startCol; x < endCol; x++)
            {
                for (int y = startRow; y < endRow; y++)
                {
                    Field currentField = currentMap[x, y];

                    if (currentField != null && _textures.TryGetValue(currentField.FieldType, out BitmapImage? texture))
                    {
                        // The rectangle for the image
                        Rect destinationRectangle = new(x * TileSize, y * TileSize, TileSize, TileSize);
                        drawingContext.DrawImage(texture, destinationRectangle);
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
            this.InvalidateVisual();
        }
        #endregion
    }
}
