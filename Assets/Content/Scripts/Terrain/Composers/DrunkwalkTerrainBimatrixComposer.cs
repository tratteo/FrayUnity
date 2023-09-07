using System;
using UnityEngine;

namespace Fray.Terrain
{
    [CreateAssetMenu(fileName = "drunkwalk_bimatrix_composer", menuName = "Fray/Terrain/Composers/Drunkwalk")]
    internal class DrunkwalkTerrainBimatrixComposer : TerrainBimatrixComposer
    {
        [SerializeField] private Vector2Int drunkardMaxDistance;
        [SerializeField, Range(0F, 1F)] private float randomDrunkardPosProb = 0.25F;
        [SerializeField, Range(0F, 1F)] private float fillPercentage = 0.5F;
        private int drunkX;
        private int drunkY;

        protected override int[,] ComposeBehaviour(int[,] mat, int width, int height)
        {
            drunkX = width / 2;
            drunkY = height / 2;
            var occupied = 0;
            for (int i = drunkX - 2; i <= drunkX + 2; i++)
            {
                for (int j = drunkY - 2; j <= drunkY + 2; j++)
                {
                    mat[i, j] = 0;
                    occupied++;
                }
            }
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
                            drunkY = Mathf.Clamp(drunkY + 1, 0, height - 1);
                            break;

                        case 1:
                            drunkY = Mathf.Clamp(drunkY - 1, 0, height - 1);
                            break;

                        case 2:
                            drunkX = Mathf.Clamp(drunkX + 1, 0, width - 1);
                            break;

                        case 3:
                            drunkX = Mathf.Clamp(drunkX - 1, 0, width - 1);
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

            return mat;
        }
    }
}