using UnityEngine;

namespace Fray.Terrain
{

    public abstract class TerrainBimatrixComposer : ScriptableObject
    {
        [Header("Morphology")]
        [SerializeField] private int erosionKernelSize = 3;
        [SerializeField] private int dilationKernelSize = 3;
        [SerializeField] private bool denselyConnect = true;
        [SerializeField] private int border = 2;
        [SerializeField] private Vector2 spawnPointRatioPosition = Vector2.one * 0.5F;
        [SerializeField] private int spawnPointSize = 3;

        private System.Random rand;

        public const int Block = 1;
        public const int Empty = 0;
        protected System.Random Rand => rand;

        public int[,] Compose(int width, int height, int seed)
        {
            rand = new System.Random(seed);
            int[,] mat = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    mat[i, j] = Block;
                }
            }
            mat = ComposeBehaviour(mat, width, height);
            if (erosionKernelSize > 1) mat = mat.Erode(erosionKernelSize);
            if (dilationKernelSize > 1) mat = mat.Dilate(dilationKernelSize);
            mat = mat.Border(border);

            var center = new Vector2Int((int)(width * spawnPointRatioPosition.x), (int)(height * spawnPointRatioPosition.y));
            for (int i = -spawnPointSize; i <= spawnPointSize; i++)
                for (int j = -spawnPointSize; j <= spawnPointSize; j++) mat[i + center.x, j + center.y] = Empty;

            if (denselyConnect) mat = mat.PruneNotConnected(center.x, center.y);
            return mat;
        }

        protected abstract int[,] ComposeBehaviour(int[,] mat, int width, int height);
    }
}