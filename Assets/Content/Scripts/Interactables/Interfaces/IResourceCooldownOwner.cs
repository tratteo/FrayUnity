namespace Fray
{
    /// <summary>
    ///   Has multiple cooldowns (charges)
    /// </summary>
    public interface IResourceCooldownOwner : ICooldownOwner
    {
        int GetResourcesAmount();
    }
}