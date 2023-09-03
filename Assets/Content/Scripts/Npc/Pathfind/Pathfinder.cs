using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Npc.Pathfinding
{
    public partial class Pathfinder : MonoBehaviour
    {
        [SerializeField, Tooltip("Smooth path and add waypoints only if the direction changes")] private bool prunePath = true;
        private PathfindTerrain terrain;

        public void Pathfind(PathRequest request) => FindPath(request, terrain);

        public Cell GetClosestCellTo(Vector2 pos, bool walkableOnly = true)
        {
            var res = terrain.CellFromWorldPos(pos);
            if (!walkableOnly || res.Walkable) return res;
            int counter = 0;
            int kernelSize = 1;
            while (counter < terrain.SizeX * terrain.SizeY)
            {
                for (int i = -kernelSize; i <= kernelSize; i++)
                    for (int j = -kernelSize; j <= kernelSize; j++)
                    {
                        int x = res.X + i;
                        int y = res.Y + j;
                        if (terrain.IsInBounds(x, y) && terrain[x, y].Walkable) return terrain[x, y];
                    }
                kernelSize++;
            }

            return null;
        }

        private void Awake()
        {
            terrain = FindFirstObjectByType<PathfindTerrain>();
        }

        private int GetDistance(Cell a, Cell b)
        {
            int dstX = Mathf.Abs(a.X - b.X);
            int dstY = Mathf.Abs(a.Y - b.Y);
            return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
        }

        private Vector2[] RetracePath(Cell start, Cell target)
        {
            List<Cell> path = new();
            var current = target;
            while (current != start)
            {
                path.Add(current);
                current = current.parent;
            }
            var points = PrunePath(path);
            Array.Reverse(points);
            return points;
        }

        private Vector2[] PrunePath(List<Cell> path)
        {
            List<Vector2> points = new();
            if (!prunePath) return path.ConvertAll(p => p.WorldPos).ToArray();

            Vector2 dir = Vector2.zero;
            if (path.Count == 1)
            {
                points.Add(path[0].WorldPos);
            }
            for (int i = 1; i < path.Count; i++)
            {
                Vector2 newDir = new Vector2(path[i - 1].X - path[i].X, path[i - 1].Y - path[i].Y).normalized;
                if (i == 1 || (newDir - dir).magnitude > 1E-3)
                {
                    points.Add(path[i - 1].WorldPos);
                    dir = newDir;
                }
            }
            return points.ToArray();
        }

        private void FindPath(PathRequest request, PathfindTerrain terrain)
        {
            Vector2[] cellPoints = new Vector2[0];
            bool success = false;

            var startCell = terrain.CellFromWorldPos(request.start);
            var targetCell = terrain.CellFromWorldPos(request.end);

            if (startCell.Walkable && targetCell.Walkable)
            {
                Heap<Cell> openSet = new(terrain.SizeX * terrain.SizeY);
                openSet.Add(startCell);
                HashSet<Cell> closedSet = new();

                while (openSet.Count > 0)
                {
                    var current = openSet.Peek();

                    closedSet.Add(current);
                    if (current == targetCell)
                    {
                        success = true;
                        break;
                    }

                    foreach (var (x, y) in current.NeighboursIndexes)
                    {
                        var neighbour = terrain[x, y];
                        if (!neighbour.Walkable || closedSet.Contains(neighbour)) continue;
                        int costToNeighbour = current.gCost + GetDistance(current, neighbour) + neighbour.Penalty;
                        if (costToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = costToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetCell);
                            neighbour.parent = current;
                            if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                            else openSet.Update(neighbour);
                        }
                    }
                }
            }
            if (success)
            {
                cellPoints = RetracePath(startCell, targetCell);
            }
            request.callback?.Invoke(cellPoints, success);
        }
    }
}