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
        public void Place() { }

        public void Remove() { }
        #endregion
    }
}
