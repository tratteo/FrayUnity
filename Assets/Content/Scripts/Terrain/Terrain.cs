using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Terrain
{
    internal static class Terrain
    {
        public static bool[,] FloodFill(int[,] mat)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            bool[,] res = new bool[width, height];

            var q = new Queue<Vector2Int>();
            q.Enqueue(new Vector2Int(width / 2, height / 2));
            while (q.Count > 0)
            {
                var c = q.Dequeue();
                if (mat[c.x, c.y] != 0) continue;
                res[c.x, c.y] = true;
                for (var i = c.x - 1; i <= c.x + 1; i++)
                {
                    for (var j = c.y - 1; j <= c.y + 1; j++)
                    {
                        if (i == c.x || j == c.y) continue;
                        if (i < 0 || i >= width || j < 0 || j >= height) continue;
                        q.Enqueue(new Vector2Int(i, j));
                    }
                }
            }

            return res;
        }

        public static int[,] Erode(int[,] mat, int kernelSize)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            int[,] res = new int[width, height];
            Array.Copy(mat, res, mat.Length);
            for (int i = kernelSize / 2; i < width - kernelSize / 2; i++)
            {
                for (int j = kernelSize / 2; j < height - kernelSize / 2; j++)
                {
                    if (mat[i, j] != 0) res[i, j] = mat[i, j];
                    else
                    {
                        for (int ki = -kernelSize / 2; ki < kernelSize / 2; ki++)
                        {
                            for (int kj = -kernelSize / 2; kj < kernelSize / 2; kj++)
                            {
                                res[i + ki, j + kj] = 0;
                            }
                        }
                    }
                }
            }
            return res;
        }

        public static int[,] Dilate(int[,] mat, int kernelSize)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            int[,] res = new int[width, height];
            Array.Copy(mat, res, mat.Length);
            for (int i = kernelSize / 2; i < width - kernelSize / 2; i++)
            {
                for (int j = kernelSize / 2; j < height - kernelSize / 2; j++)
                {
                    if (mat[i, j] != 1) res[i, j] = mat[i, j];
                    else
                    {
                        for (int ki = -kernelSize / 2; ki < kernelSize / 2; ki++)
                        {
                            for (int kj = -kernelSize / 2; kj < kernelSize / 2; kj++)
                            {
                                res[i + ki, j + kj] = 1;
                            }
                        }
                    }
                }
            }
            return res;
        }
    }
}