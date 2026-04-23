using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// A static class for building the graph from the <see cref="GameTable"/>.
    /// </summary>
    public static class GraphBuilder
    {
        #region Public static methods
        public static Graph BuildGraph(GameTable table)
        {
            Queue<Walker> walkersQ = [];
            List<Edge> edges = [];
            List<Node> nodes = [];
            HashSet<(int X, int Y)> visitedFields = [];
            HashSet<(int X, int Y)> visitedJunctions = [];

            for (int x = 0; x < table.Width; x++)
            {
                for (int y = 0; y < table.Height; y++)
                {
                    IField middleField = table[x, y];

                    if (middleField is Stop && !visitedFields.Contains((x, y)))
                    {
                        Node startNode = new(x, y, middleField.FieldType);
                        nodes.Add(startNode);
                        visitedJunctions.Add((x, y));

                        (int dirx, int diry)[] directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];
                        foreach ((int dirx, int diry) in directions)
                        {
                            if (table.IsInBounds(x + dirx, y + diry))
                            {
                                IField neighbourField = table[x + dirx, y + diry];
                                if (neighbourField is Road
                                    || neighbourField is IBridge
                                    || neighbourField is Stop)
                                {
                                    Walker walker = new(startNode, neighbourField, table, visitedFields, visitedJunctions);
                                    walkersQ.Enqueue(walker);
                                }
                            }
                        }

                        visitedFields.Add((x, y));
                    }

                    while (walkersQ.Count > 0)
                    {
                        Walker walker = walkersQ.Dequeue();
                        WalkerResult result = walker.Walk();

                        // Add the created edges to the graph
                        edges.AddRange(result.CreatedEdges);

                        if (result.EndNode is not null)
                        {
                            nodes.Add(result.EndNode);
                        }

                        // Add the new walkers to the queue for further exploration
                        foreach (Walker newWalker in result.NewWalkers)
                        {
                            walkersQ.Enqueue(newWalker);
                        }
                    }
                }
            }

            return new Graph(nodes, edges);
        }
        #endregion
    }
}
