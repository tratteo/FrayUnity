using System.Collections.Generic;
using UnityEngine;

namespace Fray.Npc.Pathfinding
{
    public class Cell : IHeapItem<Cell>
    {
        public int gCost;
        public int hCost;
        public Cell parent;

        public int Penalty { get; set; }

        public int FCost => gCost + hCost;

        public bool Walkable { get; private set; }

        public Vector2 WorldPos { get; private set; }

        public int X { get; private set; }

        public List<(int x, int y)> NeighboursIndexes { get; private set; }

        public int Y { get; private set; }

        public int HeapIndex { get; set; }

        public Cell(int x, int y, bool walkable, Vector2 worldPos, List<(int, int)> neighbours, int penalty = 0)
        {
            Walkable = walkable;
            WorldPos = worldPos;
            NeighboursIndexes = neighbours;
            X = x;
            Y = y;
            Penalty = penalty;
        }

        public int CompareTo(Cell other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0) compare = hCost.CompareTo(other.hCost);
            return -compare;
        }
    }
}