using UnityEngine;

namespace Fray
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private bool spawnAfterGeneration = true;
        [SerializeField] private GameObject prefab;
        private GameObject spawnedInstance;

        public void Spawn()
        {
            if (spawnedInstance)
            {
                Destroy(spawnedInstance);
            }
            spawnedInstance = Instantiate(prefab, transform.position, Quaternion.identity);
        }

        private void Awake()
        {
            if (spawnAfterGeneration)
            {
                var terrain = FindFirstObjectByType<TerrainGenerator>();
                if (terrain)
                {
                    terrain.OnGenerate += Spawn;
                }
            }
        }
    }
}