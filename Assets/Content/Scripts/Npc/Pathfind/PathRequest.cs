using System;
using UnityEngine;

namespace Fray.Npc.Pathfinding
{
    public struct PathRequest
    {
        public Vector2 start;
        public Vector2 end;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 start, Vector2 end, Action<Vector2[], bool> callback)
        {
            this.start = start;
            this.end = end;
            this.callback = callback;
        }
    }
}