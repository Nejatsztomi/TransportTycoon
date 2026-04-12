using TransportTycoon.MapData;

namespace TransportTycoon.Model.Graph
{
    public record WalkerResult(
    List<Edge> CreatedEdges,
    List<Walker> NewWalkers,
    Node? EndNode
    );

    public class Walker
    {
        #region Private fields
        private readonly Node _startNode;
        private readonly HashSet<(int X, int Y)> _visitedFields;
        private readonly List<Field> _roads;
        private readonly GameTable _gameTable;
        #endregion

        #region Constructors
        public Walker(Node startNode, GameTable gameTable, HashSet<(int X, int Y)> visitedFields)
        {
            _startNode = startNode;
            _gameTable = gameTable;
            _visitedFields = visitedFields;
            _roads = [];
        }
        #endregion

        #region Public methods
        public WalkerResult Walk()
        {
            (int dirx, int diry)[] directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];
            Node? terminatingNode = null;
            Node currentNode = _startNode;
            (double forwardCost, double backwardCost) = (0.0, 0.0);

            while (terminatingNode is null)
            {
                Field? nextField = null;
                foreach ((int dirx, int diry) in directions)
                {
                    nextField = GetNextValidField(currentNode.X + dirx, currentNode.Y + diry);
                    if (nextField is not null)
                    {
                        break;
                    }
                }

                if (nextField is null)
                {
                    break;
                }

                _visitedFields.Add((nextField.X, nextField.Y));
                _roads.Add(nextField);

                int heightDifference = nextField.Height - _gameTable.Table[currentNode.X, currentNode.Y].Height;
                (double fc, double bc) = CalculateRoadCost(heightDifference);
                forwardCost += fc;
                backwardCost += bc;

                Node nextNode = new(nextField.X, nextField.Y, nextField.FieldType);

                if (nextField.FieldType == FieldType.Stop)
                {
                    terminatingNode = nextNode;
                }
                else if (nextField.FieldType == FieldType.Road && nextField is Road road)
                {
                    if (road.RoadType == RoadType.XRoad
                        || road.RoadType == RoadType.DownTRoad
                        || road.RoadType == RoadType.UpperTRoad
                        || road.RoadType == RoadType.LeftTRoad
                        || road.RoadType == RoadType.RightTRoad)
                    {
                        terminatingNode = nextNode;
                    }
                }

                currentNode = nextNode;
            }

            List<Edge> edges = [];
            List<Walker> newWalkers = [];
            Node? endNode = null;

            if (terminatingNode is not null)
            {
                SharedRoadSequence sharedRoadSequence = new(_roads);
                edges.Add(new(_startNode, terminatingNode, sharedRoadSequence.ForwardEnumerator(), forwardCost));
                edges.Add(new(terminatingNode, _startNode, sharedRoadSequence.BackwardEnumerator(), forwardCost));

                foreach ((int dirx, int diry) in directions)
                {
                    Field? field = GetNextValidField(dirx, diry);
                    if (field is null)
                    {
                        continue;
                    }
                    newWalkers.Add(new Walker(terminatingNode, _gameTable, _visitedFields));
                }

                endNode = terminatingNode;
            }

            return new WalkerResult(edges, newWalkers, endNode);
        }
        #endregion

        #region Private methods
        private bool InBounds(int x, int y) => 0 <= x && x < _gameTable.Width && 0 <= y && y < _gameTable.Height;

        private bool IsValidFieldType(FieldType fieldType) => fieldType == FieldType.Road || fieldType == FieldType.Bridge || fieldType == FieldType.Stop;

        private (double forwardCost, double backwardCost) CalculateRoadCost(int heightDifference)
        {
            if (heightDifference > 0)
            {
                return (2.0, 0.8);
            }
            else if (heightDifference < 0)
            {
                return (0.8, 2.0);
            }
            return (1.0, 1.0);
        }

        private Field? GetNextValidField(int x, int y)
        {
            if (!InBounds(x, y))
            {
                return null;
            }
            Field nextField = _gameTable.Table[x, y];
            if (!IsValidFieldType(nextField.FieldType))
            {
                return null;
            }
            if (_visitedFields.Contains((nextField.X, nextField.Y)))
            {
                return null;
            }
            return nextField;
        }
        #endregion
    }
}
