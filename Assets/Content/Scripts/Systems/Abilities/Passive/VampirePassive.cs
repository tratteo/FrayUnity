using Fray.Systems.Weapons;
using GibFrame.Meta;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Systems.Abilities
{
    [Target(typeof(Sword), IncludeChildren = true)]
    public class VampirePassive : PassiveAbility
    {
        private HealthSystem healthSystem;
        private Sword sword;
        [SerializeField, Range(0, 1)] private float multiplier;

        protected override void Initialize()
        {
            healthSystem = ParentObj.GetComponent<HealthSystem>();
            sword = ParentObj.GetComponent<IWeaponOwner>().GetWeapon() as Sword;
            sword.Hit += SwordHit;
        }

        protected override void Dispose() => sword.Hit -= SwordHit;

        private void SwordHit(IReadOnlyCollection<GameObject> obj)
        {
            if (obj.Count == 0) return;
            var lifeStealAmount = sword.GetDamage() * multiplier;
            healthSystem.Increase(lifeStealAmount * obj.Count);
        }
    }
}