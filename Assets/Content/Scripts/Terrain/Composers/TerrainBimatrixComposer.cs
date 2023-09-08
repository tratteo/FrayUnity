using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Fray.Terrain
{
    public abstract class TerrainBimatrixComposer : ScriptableObject
    {
        public const int Block = 1;
        public const int Empty = 0;
        [Header("Morphology")]
        [SerializeField] private int erosionKernelSize = 3;
        [SerializeField] private int dilationKernelSize = 3;
        [SerializeField] private int border = 2;
        [SerializeField] private int minRoomSize = 9;
        [SerializeField] private int minWallSize = 4;
        private System.Random rand;

        protected System.Random Rand => rand;

        public Task<Bimatrix> Compose(int width, int height, int seed, Action<Bimatrix> onComplete = null)
        {
            rand = new System.Random(seed);
            var bimatrix = ComposeBehaviour(new Bimatrix(width, height))
                .Erode(erosionKernelSize)
                .Dilate(dilationKernelSize)
                .PruneCliques(minWallSize, Block, Empty)
                .PruneCliques(minRoomSize, Empty, Block)
                .AddBorder(border);
            onComplete?.Invoke(bimatrix);
            return Task.FromResult(bimatrix);
        }

        protected abstract Bimatrix ComposeBehaviour(Bimatrix bimatrix);
    }
}