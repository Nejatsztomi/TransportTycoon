namespace TransportTycoon.MapData
{
    /// <summary>
    /// Represents an abstract base class for a field with position and dimension properties, providing a contract for
    /// derived types to define specific field behavior.
    /// </summary>
    /// <remarks>Derived classes should implement or override members to specify field-specific logic, such as
    /// the number of trees or whether the field is modifiable. This class serves as a foundation for representing
    /// spatial fields in applications such as games or simulations.</remarks>
    public abstract class Field
    {
        #region Properties
        /// <summary>
        /// Gets the value of X. Can only be set within this class or derived classes.
        /// </summary>
        public int X { get; protected set; }

        /// <summary>
        /// Gets the value of Y. Can only be set within this class or derived classes.
        /// </summary>
        public int Y { get; protected set; }

        /// <summary>
        /// Gets the value of Height. Can only be set within this class or derived classes.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current object can be modified.
        /// </summary>
        public virtual bool Modifiable
        {
            get => true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Retrieves the number of trees on the field. This method can be overridden by implementing classes to provide the actual number of trees.
        /// </summary>
        /// <returns>The number of trees on the field.</returns>
        public virtual int GetTrees() => 0;
        #endregion
    }

    /// <summary>
    /// Represents a water field at a specific location within the environment.
    /// </summary>
    /// <remarks>Water fields are typically non-modifiable and may be used to indicate impassable or special
    /// terrain within a grid or map. This class provides properties to access the field's coordinates and
    /// height.</remarks>
    public class Water : Field
    {
        #region Properties
        public override bool Modifiable => false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Water class at the specified coordinates.
        /// </summary>
        /// <param name="x">The horizontal coordinate of the water instance.</param>
        /// <param name="y">The vertical coordinate of the water instance.</param>
        public Water(int x, int y)
        {
            X = x;
            Y = y;
            Height = 0;
        }
        #endregion
    }
}
