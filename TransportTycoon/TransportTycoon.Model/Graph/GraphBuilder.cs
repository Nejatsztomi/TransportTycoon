using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// A static class for building the graph from the <see cref="GameTable"/>.
    /// </summary>
    public static class GraphBuilder
    {
        #region Private static fields
        private static readonly (int dx, int dy)[] _directions = [(0, -1), (0, 1), (-1, 0), (1, 0)];
        #endregion

        #region Public static methods
        public static Graph BuildGraph(GameTable table)
        {
            PathTracer tracer = new(table);

            Queue<Node> nodesToExplore = [];

            var graph = new Graph([], []);

            HashSet<(int X, int Y)> visitedRoadTiles = [];

            for (int x = 0; x < table.Width; x++)
            {
                for (int y = 0; y < table.Height; y++)
                {
                    IField field = table[x, y];
                    if (field is Stop)
                    {
                        Node startNode = new(x, y, field.GetType());
                        graph.AddNode(startNode);
                        nodesToExplore.Enqueue(startNode);
                    }
                }
            }

            while (nodesToExplore.Count > 0)
            {
                var currentNode = nodesToExplore.Dequeue();
                IField currentTile = table[currentNode.X, currentNode.Y];

                foreach (var dir in _directions)
                {
                    int startStepX = currentTile.X + dir.dx;
                    int startStepY = currentTile.Y + dir.dy;

                    if (!table.IsInBounds(startStepX, startStepY)) continue;
                    if (table[startStepX, startStepY] is not IInfrastructure) continue;
                    if (visitedRoadTiles.Contains((startStepX, startStepY))) continue;

                    TraceResult result = tracer.TraceSegment(currentTile, dir);

                    if (result.Status == TraceStatus.FoundIntersection)
                    {
                        var destTile = result.EndTile;
                        if (destTile is null) continue;

                        var destinationNode = graph.GetNodeAt(destTile.X, destTile.Y) ?? new Node(destTile.X, destTile.Y, destTile.GetType());

                        if (!graph.ContainsNode(destinationNode))
                        {
                            graph.AddNode(destinationNode);
                            nodesToExplore.Enqueue(destinationNode);
                        }

                        foreach (var tile in result.PathTaken)
                        {
                            visitedRoadTiles.Add((tile.X, tile.Y));
                        }

                        // 4. Create Twin Edges Instantly!
                        SharedRoadSequence sharedSequence = new(result.PathTaken);

                        Edge forwardEdge = new(currentNode, destinationNode, sharedSequence.ForwardEnumerator(), result.ForwardCost);
                        Edge backwardEdge = new(destinationNode, currentNode, sharedSequence.BackwardEnumerator(), result.BackwardCost);

                        graph.AddEdge(currentNode, forwardEdge);
                        graph.AddEdge(destinationNode, backwardEdge);
                    }
                }
            }
            return graph;
        }
        #endregion
    }
}
