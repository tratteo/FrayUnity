using Fray.Systems.Weapons;
using GibFrame;
using GibFrame.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Npc
{
    [RequireComponent(typeof(ManagedRigidbody))]
    public class MeleeNpc : Npc, IWeaponOwner, ITargetOwner
    {
        [SerializeField] private RandomizedFloat targetPaddingDistance;
        [Header("Weapon")]
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private RandomizedFloat attackDelay;
        [SerializeField] private float parryProbability = 0.25F;
        private Sword sword;
        private bool attacking = false;

        public Weapon GetWeapon() => sword;

        public void EquipWeapon(Weapon weapon)
        {
            sword = weapon as Sword;
            sword.AttackPerformed += (atk) => attacking = false;
        }

        protected override void OnDetected(IEnumerable<Collider2D> enumerable)
        {
            foreach (var collider in enumerable)
            {
                if (collider.CompareTag("Character"))
                {
                    Target = collider.gameObject;

                    sword.SetTarget(Target.transform);
                    return;
                }
            }
        }

        protected override void Awake() => base.Awake();

        protected override Vector3 GetTargetPosition()
        {
            var randX = Mathf.Sign(transform.position.x - Target.transform.position.x) * Random.value;
            var randY = Mathf.Sign(transform.position.y - Target.transform.position.y) * Random.value;
            return Pathfinder.GetClosestCellTo(Target.transform.position + new Vector3(randX, randY, 0F).normalized * targetPaddingDistance.GetNext()).WorldPos;
        }

        protected override void Start()
        {
            base.Start();
            if (swordPrefab)
            {
                Weapon.TryApply(swordPrefab.GetComponent<Sword>(), gameObject, out var _);
            }
        }

        private void FixedUpdate()
        {
            if (Target && sword)
            {
                var range = sword.CachedAttack ? sword.CachedAttack.Range : 4F;
                if (Vector2.Distance(Target.transform.position, transform.position) <= range * 0.75F && !attacking)
                {
                    attacking = true;
                    new Timer(this, attackDelay, false, true, new Callback(() => sword.Trigger()));
                }
                //TODO change automatic parry method
                if (!sword.IsParrying && !sword.IsPreparingAttack && Target.TryGetComponent<IWeaponOwner>(out var owner) && owner.GetWeapon() is Sword enemySword && enemySword.IsPreparingAttack && Random.value < parryProbability)
                {
                    sword.EnableParry();
                }
            }
        }
    }
}