namespace Fray
{
    /// <summary>
    ///   Has a cooldown
    /// </summary>
    public interface ICooldownOwner
    {
        float GetCooldown();

        float GetCooldownPercentage();
    }
}