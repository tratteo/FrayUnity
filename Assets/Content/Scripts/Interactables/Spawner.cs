using UnityEngine;

namespace Fray
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private bool spawnAfterGeneration = true;
        [SerializeField] private GameObject prefab;
        private GameObject spawnedInstance;

        public void Spawn(Vector2 spawnPoint)
        {
            if (spawnedInstance)
            {
                Destroy(spawnedInstance);
            }
            spawnedInstance = Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}