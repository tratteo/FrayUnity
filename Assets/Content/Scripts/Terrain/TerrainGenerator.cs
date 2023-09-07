using Fray.Npc.Pathfinding;
using Fray.Terrain;
using GibFrame;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Fray
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Vector2Int mapSize = new Vector2Int(20, 20);
        [SerializeField] private TerrainBimatrixComposer composer;
        [SerializeField] private int seed;
        [SerializeField] private UnityEvent onGenerated;
        [SerializeField] private bool generatePathfindTerrain = true;
        [SerializeField] private TerrainTiles tiles;
        private PathfindTerrain terrain;

        public event Action OnGenerate = delegate { };

        private void Awake()
        {
            terrain = GetComponent<PathfindTerrain>();
        }

        private void Start()
        {
            GenerateTerrain();
        }

        private void GenerateTerrain()
        {
            tilemap.ClearAllTiles();
            var bimatrix = composer.Compose(mapSize.x, mapSize.y, seed);
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    if (j - 1 >= 0 && j - 1 < mapSize.y && bimatrix[i, j] == TerrainBimatrixComposer.Block && bimatrix[i, j - 1] == TerrainBimatrixComposer.Empty)
                    {
                        tilemap.SetTile(new Vector3Int(i - (mapSize.x) / 2, j - (mapSize.y) / 2, 0), tiles.Baseboard.GetRandomTile());
                    }
                    else if (bimatrix[i, j] == TerrainBimatrixComposer.Block)
                    {
                        tilemap.SetTile(new Vector3Int(i - (mapSize.x) / 2, j - (mapSize.y) / 2, 0), tiles.Wall.GetRandomTile());
                    }
                    else if (bimatrix[i, j] == TerrainBimatrixComposer.Empty)
                    {
                        tilemap.SetTile(new Vector3Int(i - (mapSize.x) / 2, j - (mapSize.y) / 2, 0), tiles.Floor.GetRandomTile());
                    }
                }
            }
            new Timer(this, 0.25F, true, true, new Callback(GeneratePathfindTerrain));
        }

        private void GeneratePathfindTerrain()
        {
            if (generatePathfindTerrain && terrain)
            {
                terrain.size = mapSize * 2;
                terrain.cellSize = 0.5F;
                terrain.unitSize = 1.35F;
                terrain.Generate();
            }
            onGenerated?.Invoke();
            OnGenerate?.Invoke();
        }
    }
}