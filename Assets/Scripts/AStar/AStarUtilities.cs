using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

namespace DreamLU.AStar
{
    public static class AStarUtilities
    {
        public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
        {
            startGridPosition -= (Vector3Int)room.InstancedRoom.TemplateLowerBounds;
            endGridPosition -= (Vector3Int)room.InstancedRoom.TemplateLowerBounds;

            List<AStarNode> openNodeList = new List<AStarNode>();
            HashSet<AStarNode> closeNodeList = new HashSet<AStarNode>();

            int width = room.InstancedRoom.TemplateUpperBounds.x - room.InstancedRoom.TemplateLowerBounds.x + 1;
            int height = room.InstancedRoom.TemplateUpperBounds.y - room.InstancedRoom.TemplateLowerBounds.y + 1;
            AStarGridNodes gridNodes = new AStarGridNodes(width, height);

            AStarNode startNode = gridNodes.GetGridNode(new Vector2Int(startGridPosition.x, startGridPosition.y));
            AStarNode targetNode = gridNodes.GetGridNode(new Vector2Int(endGridPosition.x, endGridPosition.y));

            AStarNode endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closeNodeList,
                room);
            
            if (endPathNode != null)
            {
                return CreatePathStack(endPathNode, room);
            }

            return null;
        }

        private static AStarNode FindShortestPath(AStarNode starNode, AStarNode targetNode, AStarGridNodes gridNodes,
            List<AStarNode> openNodeList, HashSet<AStarNode> closeNodeList, Room room)
        {
            openNodeList.Add(starNode);

            while (openNodeList.Count > 0)
            {
                openNodeList.Sort();

                AStarNode currentNode = openNodeList[0];
                openNodeList.RemoveAt(0);
                
                if (currentNode == targetNode)
                {
                    return currentNode;
                }

                closeNodeList.Add(currentNode);

                EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closeNodeList,
                    room);
            }

            return null;
        }

        private static void EvaluateCurrentNodeNeighbours(AStarNode currentNode, AStarNode targetNode, AStarGridNodes gridNodes, List<AStarNode> openNodeList, HashSet<AStarNode> closeNodeList, Room room)
        {
            Vector2Int currentNodeGridPosition = currentNode.gridPosition;

            AStarNode validNeighbourNode;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    
                    validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closeNodeList, room);
                    
                    if (validNeighbourNode != null)
                    {
                        // Calculate new gcost for neighbour
                        int newCostToNeighbour;

                        // Get the movement penalty
                        // Unwalkable paths have a value of 0. Default movement penalty is set in
                        // Settings and applies to other grid squares.
                        int movementPenaltyForGridSpace = room.AStarMovementPenalty[validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];

                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

                        bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                        if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                        {
                            validNeighbourNode.gCost = newCostToNeighbour;
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            validNeighbourNode.parentNode = currentNode;

                            if (!isValidNeighbourNodeInOpenList)
                            {
                                openNodeList.Add(validNeighbourNode);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evaluate a neighbour node at neighboutNodeXPosition, neighbourNodeYPosition, using the
        /// specified gridNodes, closedNodeHashSet, and instantiated room.  Returns null if the node isn't valid
        /// </summary>
        private static AStarNode GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, AStarGridNodes gridNodes, HashSet<AStarNode> closedNodeHashSet, Room room)
        {
            // If neighbour node position is beyond grid then return null
            if (neighbourNodeXPosition >= room.InstancedRoom.TemplateUpperBounds.x - room.InstancedRoom.TemplateLowerBounds.x || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= room.InstancedRoom.TemplateUpperBounds.y - room.InstancedRoom.TemplateLowerBounds.y || neighbourNodeYPosition < 0) {
                return null;
            }

            // Get neighbour node
            AStarNode neighbourNode = gridNodes.GetGridNode(new Vector2Int(neighbourNodeXPosition, neighbourNodeYPosition));

            // check for obstacle at that position
            int movementPenaltyForGridSpace = room.AStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];
            
            // check for moveable obstacle at that position
            int itemObstacleForGridSpace = room.AStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];


            // if neighbour is an obstacle or neighbour is in the closed list then skip
            if (movementPenaltyForGridSpace == 0 || itemObstacleForGridSpace == 0 || closedNodeHashSet.Contains(neighbourNode))
            {
                return null;
            }
            else
            {
                return neighbourNode;
            }
        }
        
        /// <summary>
        /// Returns the distance int between nodeA and nodeB
        /// </summary>
        private static int GetDistance(AStarNode nodeA, AStarNode nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);  // 10 used instead of 1, and 14 is a pythagoras approximation SQRT(10*10 + 10*10) - to avoid using floats
            return 14 * dstX + 10 * (dstY - dstX);
        }
        
        /// <summary>
        ///  Create a Stack<Vector3> containing the movement path 
        /// </summary>
        private static Stack<Vector3> CreatePathStack(AStarNode targetNode, Room room)
        {
            Stack<Vector3> movementPathStack = new Stack<Vector3>();

            AStarNode nextNode = targetNode;

            // Get mid point of cell
            Vector3 cellMidPoint = room.Grid.cellSize * 0.5f;
            cellMidPoint.z = 0f;

            while (nextNode != null)
            {
                // Convert grid position to world position
                Vector3 worldPosition = room.Grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.InstancedRoom.TemplateLowerBounds.x, nextNode.gridPosition.y + room.InstancedRoom.TemplateLowerBounds.y, 0));

                // Set the world position to the middle of the grid cell
                worldPosition += cellMidPoint;

                movementPathStack.Push(worldPosition);

                nextNode = nextNode.parentNode;
            }

            return movementPathStack;
        }
    }
}
