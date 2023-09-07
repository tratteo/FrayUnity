using GibFrame.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fray.Terrain
{
    [CreateAssetMenu(fileName = "dla_bimatrix_composer", menuName = "Fray/Terrain/Composers/DLA")]
    internal class DiffusionLimitedAggregationBimatrixComposer : TerrainBimatrixComposer
    {
        [SerializeField] private int centreSize = 6;
        [SerializeField, Range(0F, 2F)] private float particleDirectionPerturbationMag = 0.75F;
        [SerializeField, Range(0F, 1F)] private float particleDirectionPerturbationProb = 0.5F;
        [SerializeField, Range(0F, 1F)] private float fillPercentage = 0.5F;

        protected override int[,] ComposeBehaviour(int[,] mat, int width, int height)
        {
            var occupied = 0;
            for (int i = (width / 2) - centreSize / 2; i < (width / 2) + centreSize / 2; i++)
            {
                for (int j = (height / 2) - centreSize / 2; j < (height / 2) + centreSize / 2; j++)
                {
                    mat[i, j] = Empty;
                    occupied++;
                }
            }

            var possibilities = new HashSet<(int, int)>();
            for (int i = 0; i < height; i++)
            {
                possibilities.Add((0, i));
                possibilities.Add((width - 1, i));
            }
            for (int i = 0; i < width; i++)
            {
                possibilities.Add((i, 0));
                possibilities.Add((i, height - 1));
            }

            var center = new Vector2(width / 2F, height / 2F);
            while (occupied < mat.Length * fillPercentage)
            {
                var randEdge = possibilities.ElementAt(Rand.Next(0, possibilities.Count));
                var dir = (center - new Vector2(randEdge.Item1, randEdge.Item2)).normalized;
                var p = new Particle(randEdge.Item1, randEdge.Item2, dir, width, height);
                if (mat[p.X, p.Y] == Empty) continue;
                while (p.CanMove(out var newX, out var newY))
                {
                    if (mat[newX, newY] == Empty)
                    {
                        mat[p.X, p.Y] = Empty;
                        occupied++;
                        break;
                    }
                    p.Move();
                    if (Rand.NextDouble() < particleDirectionPerturbationProb)
                        p.Perturbate(Rand, particleDirectionPerturbationMag);
                }
            }

            return mat;
        }

        private struct Particle
        {
            public Vector2 direction;

            private readonly int xBound;

            private readonly int yBound;

            private float x;

            private float y;

            public readonly int X => (int)x;

            public readonly int Y => (int)y;

            public Particle(int x, int y, Vector2 direction, int xBound, int yBound)
            {
                this.x = x;
                this.y = y;
                this.direction = direction.normalized;
                this.xBound = xBound;
                this.yBound = yBound;
            }

            public void Move()
            {
                x += direction.x;
                y += direction.y;
            }

            public void Perturbate(System.Random random, float magnitude) => direction = direction.Perturbate(Vector2.Perpendicular(direction), random, magnitude).normalized;

            public readonly bool CanMove(out int x, out int y)
            {
                x = (int)(this.x + direction.x);
                y = (int)(this.y + direction.y);
                return x >= 0 && x <= xBound - 1 && y >= 0 && y <= yBound - 1;
            }
        }
    }
}