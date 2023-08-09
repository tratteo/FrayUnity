namespace Fray.Systems.Animation
{
    public readonly struct AnimatorDriverData
    {
        public readonly string id;

        public readonly object arg;

        public readonly bool mirrored;

        public AnimatorDriverData(string id, bool horizontalRotated = false, object args = null)
        {
            this.mirrored = horizontalRotated;
            this.id = id;
            arg = args;
        }

        public T GetArgAs<T>() => (T)arg;
    }
}