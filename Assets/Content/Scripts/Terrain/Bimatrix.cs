using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Terrain
{
    public class Bimatrix
    {
        private readonly int[,] mat;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Length => Width * Height;

        public Bimatrix(int width, int height)
        {
            mat = new int[width, height];
            Width = width;
            Height = height;
        }

        private Bimatrix(int[,] mat)
        {
            this.mat = mat;
            Width = mat.GetLength(0);
            Height = mat.GetLength(1);
        }

        public int this[int x, int y]
        {
            get => mat[x, y];
            set => mat[x, y] = value;
        }

        public int this[Vector2Int coord]
        {
            get => mat[coord.x, coord.y];
            set => mat[coord.x, coord.y] = value;
        }

        public List<Vector2Int> GetNeighbours(int x, int y)
        {
            List<Vector2Int> res = new List<Vector2Int>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    var coord = new Vector2Int(x + i, y + j);
                    if (InBound(coord)) res.Add(coord);
                }
            }

            return res;
        }

        public Bimatrix PruneCliques(int minSize, int type, int newType)
        {
            var cliques = GetCliques(type);
            foreach (var clique in cliques)
                if (clique.Size < minSize) clique.Coords.ForEach(x => mat[x.x, x.y] = newType);
            return this;
        }

        public Bimatrix Noisy(System.Random random, float density)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                    this[i, j] = random.NextDouble() < density ? TerrainBimatrixComposer.Block : TerrainBimatrixComposer.Empty;
            }
            return this;
        }

        public bool InBound(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        public bool InBound(Vector2Int coord) => InBound(coord.x, coord.y);

        public Bimatrix Copy()
        {
            var newMat = new int[Width, Height];
            Array.Copy(mat, newMat, Length);
            return new Bimatrix(newMat);
        }

        public Bimatrix AddBorder(int border)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int b = 0; b < border; b++)
                {
                    this[b, i] = TerrainBimatrixComposer.Block;
                    this[Width - 1 - b, i] = TerrainBimatrixComposer.Block;
                }
            }
            for (int i = 0; i < Width; i++)
            {
                for (int b = 0; b < border; b++)
                {
                    this[i, b] = TerrainBimatrixComposer.Block;
                    this[i, Height - 1 - b] = TerrainBimatrixComposer.Block;
                }
            }
            return this;
        }

        public Bimatrix Dilate(int kernelSize)
        {
            if (kernelSize < 2) return this;
            Bimatrix temp = Copy();
            for (int i = kernelSize / 2; i < temp.Width - kernelSize / 2; i++)
            {
                for (int j = kernelSize / 2; j < temp.Height - kernelSize / 2; j++)
                {
                    if (this[i, j] != TerrainBimatrixComposer.Block) temp[i, j] = this[i, j];
                    else
                    {
                        for (int ki = -kernelSize / 2; ki < kernelSize / 2; ki++)
                        {
                            for (int kj = -kernelSize / 2; kj < kernelSize / 2; kj++)
                            {
                                temp[i + ki, j + kj] = TerrainBimatrixComposer.Block;
                            }
                        }
                    }
                }
            }
            Array.Copy(temp.mat, mat, temp.Length);
            return temp;
        }

        public Bimatrix Erode(int kernelSize)
        {
            if (kernelSize < 2) return this;
            Bimatrix temp = Copy();
            for (int i = kernelSize / 2; i < temp.Width - kernelSize / 2; i++)
            {
                for (int j = kernelSize / 2; j < temp.Height - kernelSize / 2; j++)
                {
                    if (this[i, j] != TerrainBimatrixComposer.Empty) temp[i, j] = this[i, j];
                    else
                    {
                        for (int ki = -kernelSize / 2; ki < kernelSize / 2; ki++)
                        {
                            for (int kj = -kernelSize / 2; kj < kernelSize / 2; kj++)
                            {
                                temp[i + ki, j + kj] = TerrainBimatrixComposer.Empty;
                            }
                        }
                    }
                }
            }
            Array.Copy(temp.mat, mat, temp.Length);
            return this;
        }

        public List<Clique> GetCliques(int type)
        {
            var cliques = new List<Clique>();

            bool[,] flags = new bool[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (!flags[i, j] && this[i, j] == type)
                    {
                        var clique = new Clique(Flood(new Vector2Int(i, j)), type);
                        cliques.Add(clique);
                        foreach (var c in clique.Coords) flags[c.x, c.y] = true;
                    }
                }
            }
            return cliques;
        }

        private List<Vector2Int> Flood(Vector2Int start)
        {
            var coords = new List<Vector2Int>();
            bool[,] flags = new bool[Width, Height];
            int type = this[start.x, start.y];
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(start);
            flags[start.x, start.y] = true;
            while (queue.Count > 0)
            {
                var tile = queue.Dequeue();
                coords.Add(tile);
                var neighbours = GetNeighbours(tile.x, tile.y);
                foreach (var neighbor in neighbours)
                {
                    if (this[neighbor] == type && !flags[neighbor.x, neighbor.y])
                    {
                        flags[neighbor.x, neighbor.y] = true;
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return coords;
        }
    }
}