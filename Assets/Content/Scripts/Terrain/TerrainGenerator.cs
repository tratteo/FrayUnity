using Fray.Terrain;
using GibFrame;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Fray
{
    [RequireComponent(typeof(AstarPath))]
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Vector2Int mapSize = new Vector2Int(20, 20);
        [SerializeField] private TerrainBimatrixComposer composer;
        [SerializeField] private int seed;
        [SerializeField] private UnityEvent onGenerated;
        [SerializeField] private bool generateNavMesh = true;
        [SerializeField] private TerrainTiles tiles;
        [SerializeField] private bool alreadyGenerated = false;
        private AstarPath astar;

        public event Action OnGenerate = delegate { };

        private void Awake()
        {
            astar = GetComponent<AstarPath>();
        }

        private void Start()
        {
            if (!alreadyGenerated)
            {
                GenerateTerrain();
            }
            else
            {
                BakeNavMesh();
            }
        }

        private void GenerateTerrain()
        {
            tilemap.ClearAllTiles();
            var bimatrix = composer.Compose(mapSize.x, mapSize.y, seed);
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    if (j - 1 >= 0 && j - 1 < mapSize.y && bimatrix[i, j] == 1 && bimatrix[i, j - 1] == 0)
                    {
                        tilemap.SetTile(new Vector3Int(i - (mapSize.x) / 2, j - (mapSize.y) / 2, 0), tiles.Baseboard.GetRandomTile());
                    }
                    else if (bimatrix[i, j] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(i - (mapSize.x) / 2, j - (mapSize.y) / 2, 0), tiles.Wall.GetRandomTile());
                    }
                    else if (bimatrix[i, j] == 0)
                    {
                        tilemap.SetTile(new Vector3Int(i - (mapSize.x) / 2, j - (mapSize.y) / 2, 0), tiles.Floor.GetRandomTile());
                    }
                }
            }
            new Timer(this, 0.25F, true, true, new Callback(BakeNavMesh));
        }

        private void BakeNavMesh()
        {
            if (generateNavMesh)
            {
                astar.data.gridGraph.SetDimensions(mapSize.x, mapSize.y, 1);
                astar.Scan();
            }
            onGenerated?.Invoke();
            OnGenerate?.Invoke();
        }
    }
}