using System.Collections.Generic;
using UnityEngine;

namespace Fray.Terrain
{
    public class Clique
    {
        public List<Vector2Int> Coords { get; private set; }

        public int Type { get; private set; }

        public int Size => Coords.Count;

        public Clique(List<Vector2Int> coords, int type)
        {
            Coords = coords;
            Type = type;
        }
    }
}