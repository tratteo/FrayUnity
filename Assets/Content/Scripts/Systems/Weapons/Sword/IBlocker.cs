namespace Fray.Systems.Weapons
{
    /// <summary>
    ///   Can block
    /// </summary>
    public interface IBlocker
    {
        bool IsBlocking();

        void Blocked();
    }
}