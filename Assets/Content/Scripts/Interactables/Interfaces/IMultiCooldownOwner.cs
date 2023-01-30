namespace Fray
{
    /// <summary>
    ///   Has multiple cooldowns (charges)
    /// </summary>
    public interface IMultiCooldownOwner : ICooldownOwner
    {
        int GetResourcesAmount();
    }
}