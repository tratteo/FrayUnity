using System;

namespace Fray.Terrain
{
    internal static class Terrain
    {
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