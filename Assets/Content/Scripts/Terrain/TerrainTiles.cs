using UnityEngine;

namespace Fray.Terrain
{
    [CreateAssetMenu(menuName = "Fray/Terrain/Tiles", fileName = "terrain_tiles")]
    public class TerrainTiles : ScriptableObject
    {
        [SerializeField] private TileOption floor;
        [SerializeField] private TileOption wall;
        [SerializeField] private TileOption baseboard;

        public TileOption Floor => floor;

        public TileOption Baseboard => baseboard;

        public TileOption Wall => wall;
    }
}