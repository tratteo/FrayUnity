using GibFrame;
using GibFrame.Extensions;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Fray.Terrain
{
    [Serializable]
    public class TileOption
    {
        [SerializeField] private TileEntry[] tiles;

        public Tile GetRandomTile()
        {
            tiles.NormalizeProbabilities();
            var random = tiles.SelectWithProbability();
            return random.Tile;
        }
    }

    [Serializable]
    public class TileEntry : IProbSelectable
    {
        [SerializeField] private Tile tile;
        [SerializeField, Range(0F, 1F)] private float influence;

        public Tile Tile => tile;

        public float ProvideSelectProbability() => influence;

        public void SetSelectProbability(float prob) => influence = prob;
    }
}