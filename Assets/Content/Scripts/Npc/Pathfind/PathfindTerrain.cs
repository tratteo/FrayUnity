using Fray.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fray.Npc.Pathfinding
{
    public class PathfindTerrain : MonoBehaviour
    {
        public enum TerrainMaterialPolicy
        { Sum, Lowest, Highest }

        [Header("Grid")]
        public Vector2Int size;
        public float cellSize = 1F;
        public float unitSize = 1.25F;
        [Header("Layers")]
        public LayerMask obstacleLayerMask;
        public int obstaclesProximityPenalty = 10;
        public TerrainMaterialPolicy terrainPolicy = TerrainMaterialPolicy.Sum;
        [Header("Advanced")]
        public bool blurPenalties = true;
        public int blurKernelSize = 1;
        [Header("Debug")]
        public bool showPenaltiesHeatmap = false;
        private Dictionary<int, TerrainMaterial> terrainsDict;
        private Cell[,] cells;
        [SerializeField] private bool generateOnAwake = true;
        [SerializeField] private TerrainMaterial[] terrains;
        [SerializeField] private bool showTerrainGizmos = true;

        private int minPenalty = int.MaxValue;
        private int maxPenalty = int.MinValue;

        public int SizeX => size.x;

        public int SizeY => size.y;

        public Cell this[int x, int y] => cells[x, y];

        public bool IsInBounds(int x, int y) => x >= 0 && x < size.x && y >= 0 && y < size.y;

        public Cell CellFromWorldPos(Vector2 pos)
        {
            float x = (size.x) * Mathf.Clamp01((pos.x + (cellSize * Mathf.RoundToInt(size.x / 2))) / (size.x * cellSize));
            float y = (size.y) * Mathf.Clamp01((pos.y + (cellSize * Mathf.RoundToInt(size.y / 2))) / (size.y * cellSize));
            int coordX = Mathf.Clamp(Mathf.FloorToInt(x), 0, size.x - 1);
            int coordY = Mathf.Clamp(Mathf.FloorToInt(y), 0, size.y - 1);
            return cells[coordX, coordY];
        }

        public void Generate()
        {
            cells = new Cell[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2 worldPoint = PosFromCoordinates(x, y);
                    bool walkable = Physics2D.OverlapBox(worldPoint, unitSize * Vector2.one, 0, obstacleLayerMask) == null;
                    int penalty = 0;
                    RaycastHit2D[] res = Physics2D.RaycastAll(worldPoint, Vector2.up, 0.1F);
                    penalty = ComputePenalty(res);
                    if (!walkable) penalty += obstaclesProximityPenalty;
                    cells[x, y] = new Cell(x, y, walkable, worldPoint, GetNeighboursIndexes(x, y), penalty: penalty);
                }
            }
            if (terrainsDict.Count > 0)
            {
                var ordered = terrainsDict.Values.OrderBy(t => t.Penalty);
                minPenalty = ordered.First().Penalty;
                maxPenalty = ordered.Last().Penalty;
            }

            if (blurPenalties) BlurPenaltyMap(blurKernelSize);
        }

        public Vector2 PosFromCoordinates(int x, int y)
        {
            var btmLeft = (Vector2)transform.position + (size.x / 2 * cellSize * Vector2.left) + (size.y / 2 * cellSize * Vector2.down);
            return btmLeft + Vector2.right * (x * cellSize + (cellSize / 2F)) + Vector2.up * (y * cellSize + (cellSize / 2F));
        }

        private int ComputePenalty(RaycastHit2D[] hits)
        {
            int penalty = 0;
            foreach (RaycastHit2D hit in hits)
            {
                if (!terrainsDict.TryGetValue(hit.collider.gameObject.layer, out var material)) continue;
                switch (terrainPolicy)
                {
                    case TerrainMaterialPolicy.Lowest:
                        penalty = Mathf.Min(penalty, material.Penalty);
                        break;

                    case TerrainMaterialPolicy.Highest:
                        penalty = Mathf.Max(penalty, material.Penalty);
                        break;

                    case TerrainMaterialPolicy.Sum:
                        penalty += material.Penalty;
                        break;
                }
            }
            return penalty;
        }

        private void OnDrawGizmos()
        {
            if (!showTerrainGizmos) return;
            Gizmos.color = Color.cyan.WithOpacity(0.15F);
            if (cells != null)
            {
                foreach (var cell in cells)
                {
                    if (showPenaltiesHeatmap)
                    {
                        Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(minPenalty, maxPenalty, cell.Penalty));
                        Gizmos.DrawCube(cell.WorldPos, cellSize * Vector3.one);
                    }
                    else
                    {
                        Gizmos.color = cell.Walkable ? Color.green.WithOpacity(0.15F) : Color.red.WithOpacity(0.15F);
                        Gizmos.DrawCube(cell.WorldPos, 0.95F * cellSize * Vector3.one);
                    }
                }
            }
            else
            {
                for (int i = 0; i < size.x; i++)
                    for (int j = 0; j < size.y; j++)
                        Gizmos.DrawWireCube(PosFromCoordinates(i, j), Vector3.one * cellSize);
            }
        }

        private void Awake()
        {
            terrainsDict = new Dictionary<int, TerrainMaterial>();
            foreach (var t in terrains)
            {
                terrainsDict.Add((int)Mathf.Log(t.Mask.value, 2), t);
            }
            if (generateOnAwake) Generate();
        }

        private void BlurPenaltyMap(int blurSize)
        {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;

            int[,] horizontalPass = new int[size.x, size.y];
            int[,] verticalPass = new int[size.x, size.y];

            for (int y = 0; y < size.y; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sX = Mathf.Clamp(x, 0, kernelExtents);
                    horizontalPass[0, y] += cells[sX, y].Penalty;
                }
                for (int x = 1; x < size.x; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, size.x);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, size.x - 1);
                    horizontalPass[x, y] = horizontalPass[x - 1, y] - cells[removeIndex, y].Penalty + cells[addIndex, y].Penalty;
                }
            }
            for (int x = 0; x < size.x; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sY = Mathf.Clamp(y, 0, kernelExtents);
                    verticalPass[x, 0] += horizontalPass[x, sY];
                }
                int blurredPenalty = Mathf.RoundToInt((float)verticalPass[x, 0] / (kernelSize * kernelSize));
                cells[x, 0].Penalty = blurredPenalty;
                for (int y = 1; y < size.y; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, size.y);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, size.y - 1);
                    verticalPass[x, y] = verticalPass[x, y - 1] - horizontalPass[x, removeIndex] + horizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)verticalPass[x, y] / (kernelSize * kernelSize));
                    cells[x, y].Penalty = blurredPenalty;
                    if (blurredPenalty < minPenalty) minPenalty = blurredPenalty;
                    if (blurredPenalty > maxPenalty) maxPenalty = blurredPenalty;
                }
            }
        }

        private List<(int, int)> GetNeighboursIndexes(int coordX, int coordY)
        {
            List<(int, int)> cells = new List<(int, int)>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    int x = coordX + i;
                    int y = coordY + j;
                    if (x >= 0 && x < size.x && y >= 0 && y < size.y)
                    {
                        cells.Add((x, y));
                    }
                }
            }
            return cells;
        }
    }
}