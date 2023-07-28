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
        [SerializeField] private Tile sideWall;
        [SerializeField] private GameObject charaPrefab;
        private GameObject charaInstance;

        private void Start()
        {
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
                    if (j - 1 >= 0 && j - 1 < mapSize.y && bimatrix[i, j] == 1 && bimatrix[i, j - 1] == 0)
                    {
                        tilemap.SetTile(new Vector3Int(i - (mapSize.x - 1) / 2, j - (mapSize.y - 1) / 2, 0), sideWall);
                    }
                }
            }
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.G))
            {
                tilemap.ClearAllTiles();
                Generate();
                if (!charaInstance)
                {
                    charaInstance = Instantiate(charaPrefab);
                }
                charaInstance.transform.localPosition = Vector3.zero;
            }
        }
    }
}