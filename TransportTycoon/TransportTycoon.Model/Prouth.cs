using TransportTycoon.MapData;
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

    public static class ProuthUtil
    {
        public static List<Node> ConvertStopTilesToNodes(List<Stop> stopTiles, Graph.Graph graph)
        {
            return [.. stopTiles
                .Select(stop => graph.GetNodeAt(stop.X, stop.Y))
                .OfType<Node>()
                ];
        }

        public static List<Stop> ConvertNodestoStopTiles(List<Node> nodes, GameTable game)
        {
            return [.. nodes
                .Select(node => game.Table[node.X, node.Y])
                .Where(field => field is not null && field is Stop)
                .Cast<Stop>()
                ];
        }
    }
}
