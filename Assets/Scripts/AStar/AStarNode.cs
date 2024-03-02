using System;
using UnityEngine;

namespace DreamLU.AStar
{
    public class AStarNode: IComparable<AStarNode>
    {
        public Vector2Int gridPosition;
        public int gCost = 0; // Distance from starting node
        public int hCost = 0; // Distance from finishing node
        public AStarNode parentNode;

        public AStarNode(Vector2Int gridPosition)
        {
            this.gridPosition = gridPosition;
            parentNode = null;
        }
        
        public int FCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public int CompareTo(AStarNode aStarNodeToCompare)
        {
            int compare = FCost.CompareTo(aStarNodeToCompare.FCost);

            if (compare == 0)
            {
                compare = hCost.CompareTo(aStarNodeToCompare.hCost);
            }

            return compare;
        }
    }
}
