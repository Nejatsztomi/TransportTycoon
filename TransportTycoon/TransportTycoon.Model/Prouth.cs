using TransportTycoon.Model.Graph;

namespace TransportTycoon.Model
{
    /// <summary>
    /// A class representing a route between multiple stops in the transport network.
    /// </summary>
    public class Prouth
    {
        #region Properties
        /// <summary>
        /// The list of <see cref="Node"/> objects representing the stops along the route.
        /// </summary>
        public List<Node> Stops { get; private set; }
        #endregion

        #region Constructors
        public Prouth(List<Node> stops)
        {
            Stops = stops;
        }

        public Prouth()
        {
            Stops = [];
        }
        #endregion

        #region Public methods
        public void AddStop(Node stop)
        {
            Stops.Add(stop);
        }

        public void RemoveStop(Node stop)
        {
            Stops.Remove(stop);
        }
        #endregion
    }
}
