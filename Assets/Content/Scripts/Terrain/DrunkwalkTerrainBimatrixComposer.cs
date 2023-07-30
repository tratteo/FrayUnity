using System;
using UnityEngine;

namespace Fray.Terrain
{
    [CreateAssetMenu(fileName = "drunkwalk_bimatrix_composer", menuName = "Fray/Terrain/Drunkwalk")]
    internal class DrunkwalkTerrainBimatrixComposer : TerrainBimatrixComposer
    {
        [SerializeField] private Vector2Int drunkardMaxDistance;
        [SerializeField, Range(0F, 1F)] private float randomDrunkardPosProb = 0.25F;
        [SerializeField] private int dilationKernelSize = 3;
        [SerializeField] private int erosionKernelSize = 3;
        [SerializeField, Range(0F, 1F)] private float fillPercentage = 0.5F;
        [SerializeField] private int border = 2;
        [SerializeField] private bool denselyConnect = true;
        private int drunkX;
        private int drunkY;

        protected override int[,] ComposeBehaviour(int[,] mat)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            drunkX = width / 2;
            drunkY = height / 2;
            for (int i = drunkX - 2; i <= drunkX + 2; i++)
            {
                for (int j = drunkY - 2; j <= drunkY + 2; j++)
                {
                    mat[i, j] = 0;
                }
            }
            var occupied = 0;
            while (occupied < mat.Length * fillPercentage)
            {
                var drunkDistance = Rand.Next(drunkardMaxDistance.x, drunkardMaxDistance.y);
                while (drunkDistance > 0)
                {
                    drunkDistance--;
                    var dir = Rand.Next(4);
                    switch (dir)
                    {
                        case 0:
                            drunkY = Mathf.Clamp(drunkY + 1, border, height - 1 - border);
                            break;

                        case 1:
                            drunkY = Mathf.Clamp(drunkY - 1, border, height - 1 - border);
                            break;

                        case 2:
                            drunkX = Mathf.Clamp(drunkX + 1, border, width - 1 - border);
                            break;

                        case 3:
                            drunkX = Mathf.Clamp(drunkX - 1, border, width - 1 - border);
                            break;
                    }

                    if (mat[drunkX, drunkY] != 0)
                    {
                        occupied++;
                        mat[drunkX, drunkY] = 0;
                    }
                    if (Rand.NextDouble() < randomDrunkardPosProb)
                    {
                        var current = (Rand.Next(width), Rand.Next(height));
                        drunkX = current.Item1;
                        drunkY = current.Item2;
                    }
                }
            }
            mat = Terrain.Erode(mat, erosionKernelSize, border: border);
            mat = Terrain.Dilate(mat, dilationKernelSize, border: border);
            if (denselyConnect)
            {
                mat = Terrain.PruneNotConnected(mat);
            }

            return mat;
        }
    }
}