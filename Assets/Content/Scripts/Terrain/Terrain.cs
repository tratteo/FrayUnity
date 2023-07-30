using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Terrain
{
    internal static class Terrain
    {
        public static int[,] PruneNotConnected(int[,] mat)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            bool[,] visited = new bool[width, height];

            var q = new Queue<Vector2Int>();
            q.Enqueue(new Vector2Int(width / 2, height / 2));
            while (q.TryDequeue(out var c))
            {
                if (c.x < 0 || c.x >= width || c.y < 0 || c.y >= height || mat[c.x, c.y] != 0 || visited[c.x, c.y] == true) continue;
                visited[c.x, c.y] = true;
                q.Enqueue(new Vector2Int(c.x + 1, c.y));
                q.Enqueue(new Vector2Int(c.x - 1, c.y));
                q.Enqueue(new Vector2Int(c.x, c.y - 1));
                q.Enqueue(new Vector2Int(c.x, c.y + 1));
            }
            int[,] res = new int[width, height];
            Array.Copy(mat, res, mat.Length);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!visited[i, j])
                    {
                        res[i, j] = 1;
                    }
                }
            }
            return res;
        }

        public static int[,] Erode(int[,] mat, int kernelSize, int border = 0)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            int[,] res = new int[width, height];
            Array.Copy(mat, res, mat.Length);
            for (int i = kernelSize / 2 + border; i < width - kernelSize / 2 - border; i++)
            {
                for (int j = kernelSize / 2 + border; j < height - kernelSize / 2 - border; j++)
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

        public static int[,] Dilate(int[,] mat, int kernelSize, int border = 0)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            int[,] res = new int[width, height];
            Array.Copy(mat, res, mat.Length);
            for (int i = kernelSize / 2 + border; i < width - kernelSize / 2 - border; i++)
            {
                for (int j = kernelSize / 2 + border; j < height - kernelSize / 2 - border; j++)
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