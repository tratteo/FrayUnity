namespace Fray
{
    public static class Layers
    {
        public const string Environment = "Environment";
        public const string Faction1 = "Friendly";
        public const string Faction2 = "Enemy";

        public static string ProjectileOf(string layer) => $"{layer}Proj";
    }
}