namespace Fray
{
    /// <summary>
    ///   Is stunnable
    /// </summary>
    public interface IStunnable
    {
        bool IsStunned { get; }

        void Stun(float duration);

        void Unstun();
    }
}