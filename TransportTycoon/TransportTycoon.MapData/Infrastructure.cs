namespace TransportTycoon.MapData
{
    /// <summary>
    /// Represents an abstract base class for infrastructure fields that can be placed or removed within the system.
    /// </summary>
    /// <remarks>This class provides common functionality for infrastructure-related fields and defines a
    /// contract for retrieving the price of an infrastructure type. Derived classes must implement the static Price
    /// property to specify the cost associated with the specific infrastructure.</remarks>
    public abstract class Infrastructure : Field
    {
        #region Public methods
        /// <summary>
        /// Performs the primary placement operation. The specific behavior depends on the implementation.
        /// </summary>
        public void Place() { }

        /// <summary>
        /// Removes the current item from the collection.
        /// </summary>
        public void Remove() { }
        #endregion
    }
}
