using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    /// <summary>
    /// A class for building the graph from the <see cref="GameTable"/>.
    /// </summary>
    public class GraphBuilder
    {
        #region Constructors
        public GraphBuilder() { }
        #endregion

        #region Public methods
        public Graph BuildGraph(GameTable table)
        {
            Queue<Walker> walkersQ = [];
            List<Edge> edges = [];
            List<Node> nodes = [];
            HashSet<(int X, int Y)> visitedFields = [];

            for (int x = 0; x < table.Width; x++)
            {
                for (int y = 0; y < table.Height; y++)
                {
                    if (table[x, y].FieldType == FieldType.Stop && !visitedFields.Contains((x, y)))
                    {
                        Node startNode = new(x, y, table[x, y].FieldType);
                        nodes.Add(startNode);
                        Walker initialWalker = new(startNode, table, visitedFields);
                        visitedFields.Add((x, y));
                        walkersQ.Enqueue(initialWalker);
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
