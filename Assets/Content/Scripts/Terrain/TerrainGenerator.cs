using Fray.Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Fray
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Vector2Int mapSize = new Vector2Int(20, 20);
        [SerializeField] private TerrainBimatrixComposer composer;
        [SerializeField] private int seed;
        [Header("Tiles")]
        [SerializeField] private Tile floor;
        [SerializeField] private Tile wall;

        private void Start()
        {
            Generate();
        }

        private Tile GetTileById(int id)
        {
            return id switch
            {
                0 => floor,
                1 => wall,
                _ => wall,
            };
        }

        private void Generate()
        {
            var bimatrix = composer.Compose(mapSize.x, mapSize.y, seed);
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    tilemap.SetTile(new Vector3Int(i - (mapSize.x - 1) / 2, j - (mapSize.y - 1) / 2, 0), GetTileById(bimatrix[i, j]));
                }
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
            {
                tilemap.ClearAllTiles();
                Generate();
            }
        }
    }
}