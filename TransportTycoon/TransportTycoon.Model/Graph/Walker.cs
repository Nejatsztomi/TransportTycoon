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
        private readonly Field _startField;
        private readonly GameTable _gameTable;
        private readonly HashSet<(int X, int Y)> _visitedFields;
        private readonly HashSet<(int X, int Y)> _visitedJunctions;
        private readonly List<Field> _roads = [];
        private (int X, int Y)? _previousFieldCoords = null;
        private readonly (int dirx, int diry)[] _directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];
        private (double forwardCost, double backwardCost) _roadCost = (0.0, 0.0);
        private Field _currentField;
        #endregion

        #region Constructors
        public Walker(Node startNode, Field startField, GameTable gameTable, HashSet<(int X, int Y)> visitedFields, HashSet<(int X, int Y)> visitedJunctions)
        {
            _startNode = startNode;
            _startField = startField;
            _currentField = startField;
            _gameTable = gameTable;
            _visitedFields = visitedFields;
            _visitedJunctions = visitedJunctions;
        }
        #endregion

        #region Public methods
        public WalkerResult Walk()
        {
            _roads.Add(_gameTable[_startNode.X, _startNode.Y]);
            Node? terminatingNode = Step(_startField);

            while (terminatingNode is null)
            {
                Field? nextField = null;
                foreach ((int dirx, int diry) in _directions)
                {
                    nextField = GetNextValidField(_currentField.X + dirx, _currentField.Y + diry);
                    if (nextField is not null)
                    {
                        break;
                    }
                }

                if (nextField is null)
                {
                    break;
                }

                terminatingNode = Step(nextField);
            }

            List<Edge> edges = [];
            List<Walker> newWalkers = [];
            Node? endNode = null;

            if (terminatingNode is not null)
            {
                SharedRoadSequence sharedRoadSequence = new(_roads);
                edges.Add(new(_startNode, terminatingNode, sharedRoadSequence.ForwardEnumerator(), _roadCost.forwardCost));
                edges.Add(new(terminatingNode, _startNode, sharedRoadSequence.BackwardEnumerator(), _roadCost.backwardCost));

                if (!_visitedJunctions.Contains((terminatingNode.X, terminatingNode.Y)))
                {
                    _visitedJunctions.Add((terminatingNode.X, terminatingNode.Y));

                    foreach ((int dirx, int diry) in _directions)
                    {
                        Field? field = GetNextValidField(terminatingNode.X + dirx, terminatingNode.Y + diry);
                        if (field is null)
                        {
                            continue;
                        }
                        newWalkers.Add(new Walker(terminatingNode, field, _gameTable, _visitedFields, _visitedJunctions));
                    }
                }

                endNode = terminatingNode;
            }

            return new WalkerResult(edges, newWalkers, endNode);
        }
        #endregion

        #region Private methods
        private Node? Step(Field nextField)
        {
            _visitedFields.Add((nextField.X, nextField.Y));
            _roads.Add(nextField);

            int heightDifference = nextField.Height - _gameTable.Table[_currentField.X, _currentField.Y].Height;
            CalculateRoadCost(heightDifference);

            if (IsTerminatingField(nextField))
            {
                return new Node(nextField.X, nextField.Y, nextField.FieldType);
            }

            _previousFieldCoords = (_currentField.X, _currentField.Y);
            _currentField = nextField;
            return null;
        }

        private bool IsValidFieldType(FieldType fieldType) => fieldType == FieldType.Road || fieldType == FieldType.Bridge || fieldType == FieldType.Stop;

        private void CalculateRoadCost(int heightDifference)
        {
            if (heightDifference > 0)
            {
                _roadCost.forwardCost += 2.0;
                _roadCost.backwardCost += 0.8;
            }
            else if (heightDifference < 0)
            {
                _roadCost.forwardCost += 0.8;
                _roadCost.backwardCost += 2.0;
            }
            _roadCost.forwardCost += 1.0;
            _roadCost.backwardCost += 1.0;
        }

        private Field? GetNextValidField(int x, int y)
        {
            // Prevent the walker from stepping back onto the starting node.
            if (x == _startNode.X && y == _startNode.Y)
            {
                return null;
            }

            if (!_gameTable.IsInBounds(x, y))
            {
                return null;
            }
            Field nextField = _gameTable.Table[x, y];
            if (!IsValidFieldType(nextField.FieldType))
            {
                return null;
            }

            if (_previousFieldCoords.HasValue && (x, y) == _previousFieldCoords.Value)
            {
                return null;
            }

            // Height - check
            if (Math.Abs(nextField.Height - _gameTable.Table[_currentField.X, _currentField.Y].Height) > 1)
            {
                return null;
            }

            // Allow the walker to step on junctions again.
            if (!IsTerminatingField(nextField) && _visitedFields.Contains((nextField.X, nextField.Y)))
            {
                return null;
            }
            return nextField;
        }

        private bool IsJunction(Field field)
        {
            if (field.FieldType != FieldType.Road || field is not Road road)
            {
                return false;
            }
            return road.RoadType == RoadType.XRoad
                || road.RoadType == RoadType.DownTRoad
                || road.RoadType == RoadType.UpperTRoad
                || road.RoadType == RoadType.LeftTRoad
                || road.RoadType == RoadType.RightTRoad;
        }

        private bool IsTerminatingField(Field field)
        {
            return field.FieldType == FieldType.Stop || IsJunction(field);
        }
        #endregion
    }
}
