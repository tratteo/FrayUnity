using Fray.FX;
using GibFrame;
using UnityEngine;

namespace Fray.Weapons
{
    /// <summary>
    ///   <see cref="ScriptableObject"/> grouping all the effects required by a <see cref="Sword"/>
    /// </summary>
    [CreateAssetMenu(fileName = "fx_suite", menuName = "Fray/Weapons/Fx Suite")]
    public class FxSuite : ScriptableObject
    {
        [Header("Visual")]
        [SerializeField] private Optional<VfxHandler> chargeParryVfx;
        [SerializeField] private Optional<VfxHandler> attackVfx;
        [SerializeField] private Optional<VfxHandler> chargeVfx;
        [SerializeField] private Optional<VfxHandler> parryVfx;
        [Header("Sound")]
        [SerializeField] private Optional<SfxHandler> hitSfx;
        [SerializeField] private Optional<SfxHandler> missSfx;
        [SerializeField] private Optional<SfxHandler> parrySfx;

        public Optional<VfxHandler> ChargeParryVfx => chargeParryVfx;

        public Optional<VfxHandler> AttackVfx => attackVfx;

        public Optional<VfxHandler> ChargeVfx => chargeVfx;

        public Optional<VfxHandler> ParryVfx => parryVfx;

        public Optional<SfxHandler> HitSfx => hitSfx;

        public Optional<SfxHandler> MissSfx => missSfx;

        public Optional<SfxHandler> ParrySfx => parrySfx;

        public void CacheAll(MonoBehaviour mono)
        {
            chargeParryVfx.Try(v => v.Cache(mono));
            attackVfx.Try(v => v.Cache(mono));
            chargeVfx.Try(v => v.Cache(mono));
            parryVfx.Try(v => v.Cache(mono));

            hitSfx.Try(v => v.Cache(mono));
            missSfx.Try(v => v.Cache(mono));
            parrySfx.Try(v => v.Cache(mono));
        }

        public void DisposeAll()
        {
            chargeParryVfx.Try(v => v.Dispose());
            attackVfx.Try(v => v.Dispose());
            chargeVfx.Try(v => v.Dispose());
            parryVfx.Try(v => v.Dispose());

            hitSfx.Try(v => v.Dispose());
            missSfx.Try(v => v.Dispose());
            parrySfx.Try(v => v.Dispose());
        }
    }
}