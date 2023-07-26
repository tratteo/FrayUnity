namespace Fray.Systems.Weapons
{
    /// <summary>
    ///   Has a <see cref="Weapon"/>
    /// </summary>
    public interface IWeaponOwner
    {
        Weapon GetWeapon();

        void EquipWeapon(Weapon weapon);
    }
}