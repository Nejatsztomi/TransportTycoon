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
        /// <summary>
        /// Adds a new stop to the Stops lists
        /// </summary>
        /// <param name="stop"></param>
        public void AddStop(Node stop)
        {
            Stops.Add(stop);
        }
        /// <summary>
        /// Removes a new stop to the Stops lists
        /// </summary>
        /// /// <param name="stop"></param>
        public void RemoveStop(Node stop)
        {
            Stops.Remove(stop);
        }
        #endregion
    }

    public static class ProuthUtil
    {
        /// <summary>
        /// Converts a collection of stop tiles to their corresponding nodes in the specified graph.
        /// </summary>
        /// <remarks>If a stop tile does not map to a valid node in the graph, it is omitted from the
        /// returned list.</remarks>
        /// <param name="stopTiles">A list of stop tiles to convert. Each stop tile must specify valid X and Y coordinates.</param>
        /// <param name="graph">The graph from which to retrieve nodes based on the coordinates of the stop tiles. Must not be null.</param>
        /// <returns>A list of nodes corresponding to the provided stop tiles. Only nodes that exist at the specified coordinates
        /// are included in the result.</returns>
        public static List<Node> ConvertStopTilesToNodes(List<Stop> stopTiles, Graph.Graph graph)
        {
            return [.. stopTiles
                .Select(stop => graph.GetNodeAt(stop.X, stop.Y))
                .OfType<Node>()
                ];
        }

        /// <summary>
        /// Converts a collection of nodes to a list of stop tiles based on their positions within the specified game
        /// table.
        /// </summary>
        /// <remarks>Only nodes that correspond to stop tiles in the game table are included in the
        /// result. Nodes that do not map to a stop tile are ignored.</remarks>
        /// <param name="nodes">The list of nodes representing positions on the game table to be converted to stop tiles. Cannot be null.</param>
        /// <param name="game">The game table used to retrieve stop tiles corresponding to the provided nodes. Cannot be null.</param>
        /// <returns>A list of stop tiles corresponding to the provided nodes. The list is empty if no valid stop tiles are
        /// found.</returns>
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
