using UnityEngine;

namespace Fray.Terrain
{
    public abstract class TerrainBimatrixComposer : ScriptableObject
    {
        private System.Random rand;

        protected System.Random Rand => rand;

        public int[,] Compose(int width, int height, int seed)
        {
            rand = new System.Random(seed);
            int[,] mat = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    mat[i, j] = 1;
                }
            }
            return ComposeBehaviour(mat);
        }

        protected abstract int[,] ComposeBehaviour(int[,] mat);
    }
}