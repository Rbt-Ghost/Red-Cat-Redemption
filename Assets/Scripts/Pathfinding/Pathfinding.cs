using UnityEngine;
using System.Collections.Generic;

public class Pathfinding
{
    // The API you should use to get path
    // grid: grid to search in.
    // startPos: starting position.
    // targetPos: ending position.
    public static List<Point> FindPath(Grid grid, Point startPos, Point targetPos)
    {
        // find path
        List<Node> nodes_path = _ImpFindPath(grid, startPos, targetPos);

        // convert to a list of points and return
        List<Point> ret = new List<Point>();
        if (nodes_path != null)
        {
            foreach (Node node in nodes_path)
            {
                ret.Add(new Point(node.gridX, node.gridY));
            }
        }
        return ret;
    }

    // internal function to find path, don't use this one from outside
    private static List<Node> _ImpFindPath(Grid grid, Point startPos, Point targetPos)
    {
        // Safety check to ensure start/end are within bounds
        if (startPos.x < 0 || startPos.x >= grid.nodes.GetLength(0) || startPos.y < 0 || startPos.y >= grid.nodes.GetLength(1) ||
            targetPos.x < 0 || targetPos.x >= grid.nodes.GetLength(0) || targetPos.y < 0 || targetPos.y >= grid.nodes.GetLength(1))
        {
            return null;
        }

        Node startNode = grid.nodes[startPos.x, startPos.y];
        Node targetNode = grid.nodes[targetPos.x, targetPos.y];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(grid, startNode, targetNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                // --- NEW CODE: Prevent Corner Cutting ---
                // If moving diagonally, check if the adjacent straight tiles are blocked.
                int dx = neighbour.gridX - currentNode.gridX;
                int dy = neighbour.gridY - currentNode.gridY;

                // Check if the move is diagonal (both x and y change)
                if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1)
                {
                    // Check the two cardinal nodes sharing this diagonal
                    // Example: Moving Top-Right (1,1) checks Right (1,0) and Top (0,1)
                    bool walkX = grid.nodes[currentNode.gridX + dx, currentNode.gridY].walkable;
                    bool walkY = grid.nodes[currentNode.gridX, currentNode.gridY + dy].walkable;

                    // If either adjacent tile is a wall, block this diagonal move
                    if (!walkX || !walkY)
                    {
                        continue;
                    }
                }
                // ----------------------------------------

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) * (int)(10.0f * neighbour.penalty);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    private static List<Node> RetracePath(Grid grid, Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}