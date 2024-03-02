using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU.AStar
{
    public class AStarGridNodes
    {
        private int width;
        private int height;

        private AStarNode[,] gridAStarNodes;

        public AStarGridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;

            gridAStarNodes = new AStarNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridAStarNodes[x, y] = new AStarNode(new Vector2Int(x, y));
                }
            }
        }

        public AStarNode GetGridNode(Vector2Int position)
        {
            if (position.x < width && position.y < height)
            {
                return gridAStarNodes[position.x, position.y];
            }
            else
            {
                return null;
            }
        }
    }
}
