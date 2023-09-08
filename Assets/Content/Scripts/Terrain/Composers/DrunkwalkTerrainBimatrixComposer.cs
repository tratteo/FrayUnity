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

        protected override Bimatrix ComposeBehaviour(Bimatrix bimatrix)
        {
            var empty = 0;
            drunkX = bimatrix.Width / 2;
            drunkY = bimatrix.Height / 2;

            while (empty < bimatrix.Length * fillPercentage)
            {
                var drunkDistance = Rand.Next(drunkardMaxDistance.x, drunkardMaxDistance.y);
                while (drunkDistance > 0)
                {
                    drunkDistance--;
                    var dir = Rand.Next(4);
                    switch (dir)
                    {
                        case 0:
                            drunkY = Mathf.Clamp(drunkY + 1, 0, bimatrix.Height - 1);
                            break;

                        case 1:
                            drunkY = Mathf.Clamp(drunkY - 1, 0, bimatrix.Height - 1);
                            break;

                        case 2:
                            drunkX = Mathf.Clamp(drunkX + 1, 0, bimatrix.Width - 1);
                            break;

                        case 3:
                            drunkX = Mathf.Clamp(drunkX - 1, 0, bimatrix.Width - 1);
                            break;
                    }

                    if (bimatrix[drunkX, drunkY] != Empty)
                    {
                        empty++;
                        bimatrix[drunkX, drunkY] = Empty;
                    }
                    if (Rand.NextDouble() < randomDrunkardPosProb)
                    {
                        var current = (Rand.Next(bimatrix.Width), Rand.Next(bimatrix.Height));
                        drunkX = current.Item1;
                        drunkY = current.Item2;
                    }
                }
            }

            return bimatrix;
        }
    }
}