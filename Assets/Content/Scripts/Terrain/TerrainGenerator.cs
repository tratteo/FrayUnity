using Fray.Npc.Pathfinding;
using Fray.Terrain;
using System.Collections;
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
        [SerializeField] private bool generatePathfindTerrain = true;
        [SerializeField] private TerrainTiles tiles;
        [SerializeField] private GameObject characterPrefab;
        private PathfindTerrain pathfindTerrain;

        private void Awake()
        {
            pathfindTerrain = GetComponent<PathfindTerrain>();
        }

        private void Start()
        {
            _ = composer.Compose(mapSize.x, mapSize.y, seed, GenerateTerrain);
        }

        private void OnDrawGizmos()
        {
        }

        private void GenerateTerrain(Bimatrix bimatrix)
        {
            tilemap.ClearAllTiles();
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
            StartCoroutine(GeneratePathfindTerrain());
        }

        private IEnumerator GeneratePathfindTerrain()
        {
            yield return new WaitForEndOfFrame();
            if (generatePathfindTerrain && pathfindTerrain)
            {
                pathfindTerrain.size = mapSize * 2;
                pathfindTerrain.cellSize = 0.5F;
                pathfindTerrain.unitSize = 1.35F;
                pathfindTerrain.Bake();
            }
            //var spawnPoint = new Vector2(tilemap.cellSize.x * (composition.Spawn.x - mapSize.x / 2) + tilemap.cellSize.x / 2, tilemap.cellSize.y * (composition.Spawn.y - mapSize.y / 2) + tilemap.cellSize.y / 2);
            //Instantiate(characterPrefab, spawnPoint, Quaternion.identity);
            //var terrainComposition = new TerrainComposition(spawnPoint);
            //foreach (var listener in Gib.GetInterfacesOfType<IGenerationListener>()) listener.Generated(terrainComposition);
        }
    }
}