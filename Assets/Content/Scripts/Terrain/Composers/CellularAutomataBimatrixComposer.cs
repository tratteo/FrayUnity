using System;
using System.Linq;
using UnityEngine;

namespace Fray.Terrain
{
    [CreateAssetMenu(fileName = "ca_bimatrix_composer", menuName = "Fray/Terrain/Composers/Cellular")]
    internal class CellularAutomataBimatrixComposer : TerrainBimatrixComposer
    {
        [SerializeField] private int iterations = 4;
        [SerializeField, Range(0F, 1F)] private float noiseDensity = 0.6F;

        protected override int[,] ComposeBehaviour(int[,] mat, int width, int height)
        {
            mat = mat.Noise(Rand, noiseDensity);
            for (int it = 0; it < iterations; it++)
            {
                int[,] buffer = mat.Copy();
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        var neighbours = buffer.GetNeighborhood(i, j);
                        var walls = neighbours.FindAll(i => i == Block);
                        mat[i, j] = walls.Count() > 4 ? Block : Empty;
                    }
            }
            return mat;
        }
    }
}