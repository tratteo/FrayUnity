using UnityEngine;

namespace Fray.Npc.Pathfinding
{
    [System.Serializable]
    public struct TerrainMaterial
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField, Min(0F)] private int penalty;

        public readonly LayerMask Mask => layerMask;

        public readonly int Penalty => penalty;
    }
}