using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TransportTycoon.MapData;

namespace TransportTycoon.WPF.View.UserControls
{
    public class FastMapRenderer : FrameworkElement
    {
        #region Private constants
        private const int TileSize = 64;
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
        #endregion

        #region Properties
        public Field[,] Map
        {
            get => (Field[,])GetValue(MapProperty);
            set
            {
                SetValue(MapProperty, value);
            }
        }
        #endregion

        #region Constructors
        public FastMapRenderer()
        {
            _textures = new Dictionary<FieldType, BitmapImage>
            {
                { FieldType.Water, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/water2.png")) },
                { FieldType.Plain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/plain.png")) },
                { FieldType.Hill, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/hill.png")) },
                { FieldType.Mountain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/mountain.png")) },
                { FieldType.HighMountain, LoadTexture(new Uri("pack://application:,,,/Assets/Images/Terrain/highmountain.png")) },
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

            if (Map == null) return;

            int width = Map.GetLength(0);
            int height = Map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Field currentField = Map[x, y];

                    if (currentField != null && _textures.TryGetValue(currentField.FieldType, out BitmapImage? texture))
                    {
                        // The rectangle for the image
                        Rect destinationRectangle = new(x * TileSize, y * TileSize, TileSize, TileSize);
                        drawingContext.DrawImage(texture, destinationRectangle);
                    }
                }
            }
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
