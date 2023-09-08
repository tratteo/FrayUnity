using System;
using UnityEngine;

namespace Fray.Terrain
{
    [CreateAssetMenu(fileName = "ca_bimatrix_composer", menuName = "Fray/Terrain/Composers/Cellular")]
    internal class CellularAutomataBimatrixComposer : TerrainBimatrixComposer
    {
        [SerializeField] private int iterations = 4;
        [SerializeField, Range(0F, 1F)] private float noiseDensity = 0.6F;

        protected override Bimatrix ComposeBehaviour(Bimatrix bimatrix)
        {
            bimatrix.Noisy(Rand, noiseDensity);
            for (int it = 0; it < iterations; it++)
            {
                Bimatrix buffer = bimatrix.Copy();
                for (int i = 0; i < bimatrix.Width; i++)
                    for (int j = 0; j < bimatrix.Height; j++)
                    {
                        var neighbours = buffer.GetNeighbours(i, j);
                        var walls = neighbours.FindAll(i => buffer[i] == Block).Count + (8 - neighbours.Count);
                        bimatrix[i, j] = walls > 4 ? Block : Empty;
                    }
            }
            return bimatrix;
        }
    }
}