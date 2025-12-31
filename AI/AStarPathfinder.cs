using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using AiGame1.World; // To access Grid and Cell

namespace AiGame1.AI
{
    public class PathNode
    {
        public Vector2 GridPosition; // Position in the grid
        public PathNode Parent;      // Parent node for path reconstruction
        public float GCost;          // Cost from start to this node
        public float HCost;          // Heuristic cost from this node to end
        public float FCost => GCost + HCost; // Total cost

        public PathNode(Vector2 gridPosition)
        {
            GridPosition = gridPosition;
        }
    }

    public static class AStarPathfinder
    {
        public static List<Vector2> FindPath(Grid grid, Vector2 startGridPos, Vector2 targetGridPos)
        {
            // Validate start and target positions
            if (!IsValidGridPosition(grid, startGridPos) || !IsValidGridPosition(grid, targetGridPos))
            {
                return null; // Invalid positions
            }
            if (!grid.Cells[(int)startGridPos.X, (int)startGridPos.Y].IsWalkable ||
                !grid.Cells[(int)targetGridPos.X, (int)targetGridPos.Y].IsWalkable)
            {
                return null; // Start or target is unwalkable
            }
            if (startGridPos == targetGridPos)
            {
                return new List<Vector2> { startGridPos }; // Already at target
            }

            List<PathNode> openList = new List<PathNode>();
            HashSet<Vector2> closedList = new HashSet<Vector2>(); // Using HashSet for faster lookups

            PathNode startNode = new PathNode(startGridPos) { GCost = 0, HCost = GetDistance(startGridPos, targetGridPos) };
            openList.Add(startNode);

            // Dictionary to store the best PathNode for each grid position, for efficient updates
            Dictionary<Vector2, PathNode> allNodes = new Dictionary<Vector2, PathNode>
            {
                { startGridPos, startNode }
            };

            while (openList.Count > 0)
            {
                // Get the node with the lowest F Cost from the open list
                PathNode currentNode = openList.OrderBy(node => node.FCost).First();

                if (currentNode.GridPosition == targetGridPos)
                {
                    return ReconstructPath(currentNode); // Path found!
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode.GridPosition);

                foreach (PathNode neighbor in GetNeighbors(grid, currentNode))
                {
                    if (closedList.Contains(neighbor.GridPosition))
                    {
                        continue; // Already evaluated
                    }

                    // Calculate tentative gCost
                    float tentativeGCost = currentNode.GCost + GetMovementCost(currentNode.GridPosition, neighbor.GridPosition);

                    // If neighbor is not in allNodes or new path to neighbor is shorter
                    if (!allNodes.ContainsKey(neighbor.GridPosition) || tentativeGCost < allNodes[neighbor.GridPosition].GCost)
                    {
                        neighbor.Parent = currentNode;
                        neighbor.GCost = tentativeGCost;
                        neighbor.HCost = GetDistance(neighbor.GridPosition, targetGridPos);
                        
                        if (!allNodes.ContainsKey(neighbor.GridPosition))
                        {
                            allNodes.Add(neighbor.GridPosition, neighbor);
                            openList.Add(neighbor);
                        }
                        else
                        {
                             // Update existing node in allNodes and openList if path is shorter
                             // (This technically updates the PathNode in the dictionary, but if it was already in openList,
                             // its priority would need to be updated. For a List-based openList, we rely on OrderBy)
                            allNodes[neighbor.GridPosition].GCost = neighbor.GCost;
                            allNodes[neighbor.GridPosition].Parent = neighbor.Parent;
                            // HCost is already calculated when the node was first added to allNodes, no need to recalculate.
                        }
                    }
                }
            }

            return null; // No path found
        }

        private static bool IsValidGridPosition(Grid grid, Vector2 pos)
        {
            return pos.X >= 0 && pos.X < grid.Width && pos.Y >= 0 && pos.Y < grid.Height;
        }

        private static List<PathNode> GetNeighbors(Grid grid, PathNode currentNode)
        {
            List<PathNode> neighbors = new List<PathNode>();
            int x = (int)currentNode.GridPosition.X;
            int y = (int)currentNode.GridPosition.Y;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // Skip current node

                    int neighborX = x + dx;
                    int neighborY = y + dy;
                    Vector2 neighborPos = new Vector2(neighborX, neighborY);

                    if (!IsValidGridPosition(grid, neighborPos)) continue; // Out of bounds
                    if (!grid.Cells[neighborX, neighborY].IsWalkable) continue; // Not walkable

                    // Corner-cutting prevention for diagonal moves
                    if (dx != 0 && dy != 0) // It's a diagonal move
                    {
                        // Check if the two adjacent cardinal tiles are walkable
                        // E.g., moving from (x,y) to (x+1, y+1) requires (x+1, y) AND (x, y+1) to be walkable.
                        if (!grid.Cells[x + dx, y].IsWalkable || !grid.Cells[x, y + dy].IsWalkable)
                        {
                            continue; // Cannot cut corner
                        }
                    }
                    
                    neighbors.Add(new PathNode(neighborPos));
                }
            }
            return neighbors;
        }

        // Heuristic function (Chebyshev distance for 8-directional movement)
        private static float GetDistance(Vector2 posA, Vector2 posB)
        {
            float dx = System.Math.Abs(posA.X - posB.X);
            float dy = System.Math.Abs(posA.Y - posB.Y);
            return System.Math.Max(dx, dy); 
        }

        // Actual movement cost (1 for cardinal, sqrt(2) for diagonal)
        private static float GetMovementCost(Vector2 a, Vector2 b)
        {
            float dx = System.Math.Abs(a.X - b.X);
            float dy = System.Math.Abs(a.Y - b.Y);

            if (dx == 1 && dy == 1) // Diagonal move
            {
                return 1.41421356f; // sqrt(2)
            }
            else if (dx == 1 || dy == 1) // Cardinal move
            {
                return 1f;
            }
            return 0f; // Should not happen for neighbors
        }

        private static List<Vector2> ReconstructPath(PathNode endNode)
        {
            List<Vector2> path = new List<Vector2>();
            PathNode currentNode = endNode;
            while (currentNode != null)
            {
                path.Add(currentNode.GridPosition);
                currentNode = currentNode.Parent;
            }
            path.Reverse(); // Path is built backwards, so reverse it
            return path;
        }
    }
}
