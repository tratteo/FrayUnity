using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Terrain
{
    public static class TerrainExtensions
    {
        public static int[,] PruneNotConnected(this int[,] mat, int x, int y)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            bool[,] visited = new bool[width, height];

            var q = new Queue<Vector2Int>();
            q.Enqueue(new Vector2Int(x, y));
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
                        res[i, j] = TerrainBimatrixComposer.Block;
                    }
                }
            }
            return res;
        }

        public static int[,] Noise(this int[,] mat, System.Random random, float density)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                    mat[i, j] = random.NextDouble() < density ? TerrainBimatrixComposer.Block : TerrainBimatrixComposer.Empty;
            }
            return mat;
        }

        public static int[,] Border(this int[,] mat, int border)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            for (int i = 0; i < height; i++)
            {
                for (int b = 0; b < border; b++)
                {
                    mat[b, i] = TerrainBimatrixComposer.Block;
                    mat[width - 1 - b, i] = TerrainBimatrixComposer.Block;
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int b = 0; b < border; b++)
                {
                    mat[i, b] = TerrainBimatrixComposer.Block;
                    mat[i, height - 1 - b] = TerrainBimatrixComposer.Block;
                }
            }
            return mat;
        }

        public static int[,] Erode(this int[,] mat, int kernelSize)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            int[,] res = new int[width, height];
            Array.Copy(mat, res, mat.Length);
            for (int i = kernelSize / 2; i < width - kernelSize / 2; i++)
            {
                for (int j = kernelSize / 2; j < height - kernelSize / 2; j++)
                {
                    if (mat[i, j] != TerrainBimatrixComposer.Empty) res[i, j] = mat[i, j];
                    else
                    {
                        for (int ki = -kernelSize / 2; ki < kernelSize / 2; ki++)
                        {
                            for (int kj = -kernelSize / 2; kj < kernelSize / 2; kj++)
                            {
                                res[i + ki, j + kj] = TerrainBimatrixComposer.Empty;
                            }
                        }
                    }
                }
            }
            return res;
        }

        public static List<int> GetNeighborhood(this int[,] mat, int x, int y)
        {
            List<int> res = new List<int>();
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    var coordX = x + i;
                    var coordY = y + j;
                    if (coordX >= 0 && coordX < width && coordY >= 0 && coordY < height)
                    {
                        res.Add(mat[coordX, coordY]);
                    }
                    else
                    {
                        res.Add(TerrainBimatrixComposer.Block);
                    }
                }
            }
            return res;
        }

        public static int[,] Copy(this int[,] mat)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            var newMat = new int[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    newMat[i, j] = mat[i, j];
            return newMat;
        }

        public static int[,] Dilate(this int[,] mat, int kernelSize)
        {
            var width = mat.GetLength(0);
            var height = mat.GetLength(1);
            int[,] res = new int[width, height];
            Array.Copy(mat, res, mat.Length);
            for (int i = kernelSize / 2; i < width - kernelSize / 2; i++)
            {
                for (int j = kernelSize / 2; j < height - kernelSize / 2; j++)
                {
                    if (mat[i, j] != TerrainBimatrixComposer.Block) res[i, j] = mat[i, j];
                    else
                    {
                        for (int ki = -kernelSize / 2; ki < kernelSize / 2; ki++)
                        {
                            for (int kj = -kernelSize / 2; kj < kernelSize / 2; kj++)
                            {
                                res[i + ki, j + kj] = TerrainBimatrixComposer.Block;
                            }
                        }
                    }
                }
            }
            return res;
        }
    }
}