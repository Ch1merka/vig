using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Entity that can deal and take damage
    /// </summary>
    public class Attacker : Entity
    {
        [Header("[Optional] Mana properties")]
        public ObservableBarStats manaStats;

        [Header("[Optional] Close attacks the entity can do")]
        public AttacksSet<CloseAttack> closeAttacksSet;

        [Header("[Optional] Ranged attacks the entity can do")]
        public AttacksSet<RangedAttack> rangedAttacksSet;

        [Header("[Optional] Targeted attacks the entity can do")]
        public AttacksSet<RangedTargetedAttack> targetedAttacksSet;

        [Header("[Optional] Actions to do after killing an entity")]
        public UnityEvent onKill;

        protected CloseAttackExecutorOnHit closeAttackExecutor;
        protected Dictionary<string, int> attacksAmmo = new Dictionary<string, int>();

        public bool HasCloseAttackPending => closeAttackExecutor?.TimeToLivePassed == false;

        protected virtual void Awake()
        {
            CollisionNotifier.RegisterChildren(gameObject, OnCollisionEnter);
        }

        public List<Attack> GetAllAttacks()
        {
            var attacks = new List<Attack>();

            attacks.AddRange(closeAttacksSet.attacks);
            attacks.AddRange(rangedAttacksSet.attacks);
            attacks.AddRange(targetedAttacksSet.attacks);

            return attacks;
        }

        public bool DoCloseAttack(CloseAttack attack)
        {
            if (CanAttack(attack))
            {
                attack.beforeAttack?.Invoke();
                manaStats?.Decrease(attack.manaCost);

                closeAttackExecutor = new CloseAttackExecutorOnHit(attack, this);
                closeAttackExecutor.onAttackHit += AfterAttackHit;

                return true;
            }

            return false;
        }

        public bool DoTargetedAttack(RangedTargetedAttack attack, Entity target, Vector3 startPosition)
        {
            return DoProjectileBasedAttack(
                attack,
                attack.projectileProperties,
                () => TargetedProjectile.Create(target, attack, startPosition, this, attack.projectileProperties.targetMaximumDistance));
        }

        public bool DoRangedAttack(RangedAttack attack, Vector3 startPosition, Vector3 direction)
        {
            return DoProjectileBasedAttack(
                attack,
                attack.projectileProperties,
                () => ForwardProjectile.Create(attack, startPosition, direction, this));
        }

        /// <summary>
        /// Refill mana bt given amount
        /// </summary>
        /// <param name="mana">Amount to restore</param>
        public void RestoreMana(int mana)
        {
            manaStats?.Increase(mana);
        }

        /// <summary>
        /// Refill mana to its maximum value
        /// </summary>
        public void RestoreManaFully()
        {
            manaStats?.RestoreFully();
        }

        public CloseAttack GetRandomCloseAttack()
        {
            return closeAttacksSet.GetRandomAttack();
        }

        public RangedAttack GetRandomRangedAttack()
        {
            return rangedAttacksSet.GetRandomAttack();
        }

        public RangedTargetedAttack GetRandomTargetedAttack()
        {
            return targetedAttacksSet.GetRandomAttack();
        }

        public Attack GetAttack(string name)
        {
            return GetAllAttacks().FirstOrDefault(attack => attack.name == name);
        }

        /// <param name="attack">Attack to get its ammo counte</param>
        /// <returns>Attack's ammo count</returns>
        public int GetAmmoCount(Attack attack)
        {
            float maxAmmo;
            string propertiesId;

            if (attack is RangedAttack rangedAttack)
            {
                maxAmmo = rangedAttack.projectileProperties.maxAmmo;
                propertiesId = rangedAttack.projectileProperties.Id;
            }
            else if (attack is RangedTargetedAttack targetedAttack)
            {
                maxAmmo = targetedAttack.projectileProperties.maxAmmo;
                propertiesId = targetedAttack.projectileProperties.Id;
            }
            else
            {
                return 0;
            }

            return attacksAmmo.TryGetValue(propertiesId, out var currentAmmo)
                ? currentAmmo
                : (int)maxAmmo;
        }

        /// <summary>
        /// Refill a spcific attack's ammo
        /// </summary>
        /// <param name="ammoCount">Fill ammo count</param>
        /// <param name="attack">Attack to fill its ammo</param>
        /// <returns>True if refilled, false if nothing was done</returns>
        public bool RefillAmmo(int ammoCount, Attack attack)
        {
            float maxAmmo;
            string propertiesId;

            if (attack is RangedAttack rangedAttack)
            {
                maxAmmo = rangedAttack.projectileProperties.maxAmmo;
                propertiesId = rangedAttack.projectileProperties.Id;
            }
            else if (attack is RangedTargetedAttack targetedAttack)
            {
                maxAmmo = targetedAttack.projectileProperties.maxAmmo;
                propertiesId = targetedAttack.projectileProperties.Id;
            }
            else
            {
                return false;
            }

            if (maxAmmo != Mathf.Infinity
                && maxAmmo != 0
                && attacksAmmo.TryGetValue(propertiesId, out var currentAmmo))
            {
                if (ammoCount == 0 || ammoCount == Mathf.Infinity)
                {
                    attacksAmmo[propertiesId] = (int)maxAmmo;
                }
                else
                {
                    attacksAmmo[propertiesId] = Mathf.Clamp(ammoCount + currentAmmo, 0, (int)maxAmmo);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Fill all ammos count to its maximum
        /// </summary>
        /// <returns>True if refilled, false if nothing was done</returns>
        public bool RefillAllAmmosReturnState()
        {
            var refilled = attacksAmmo.Any();

            RefillAllAmmos();

            return refilled;
        }

        /// <summary>
        /// Fill all ammos count to its maximum (void version that can be called from editor)
        /// </summary>
        public void RefillAllAmmos()
        {
            attacksAmmo.Clear();
        }

        protected override void UpdateEntity()
        {
            base.UpdateEntity();
        }

        private void OnCollisionEnter(Collision collision)
        {
            closeAttackExecutor?.DamageCollider(collision);
            closeAttackExecutor = null;
        }

        private void AfterAttackHit(AttackExecutorOnHit executor, Entity attackedEntity)
        {
            if (!attackedEntity.IsAlive)
            {
                onKill?.Invoke();
            }

            executor.onAttackHit -= AfterAttackHit;
        }

        private bool CanAttack(Attack attack)
        {
            if (attack == null)
            {
                Debug.LogError($"Cannot apply null attack!");
                return false;
            }

            if (!IsAlive)
            {
                return false;
            }

            if (manaStats == null || attack.manaCost == 0)
            {
                return true;
            }

            return manaStats.currentValue - attack.manaCost >= 0;
        }

        private bool DoProjectileBasedAttack<TAttack, TProjectileProperties, TProjectile>(TAttack attack, TProjectileProperties projectileProperties, Func<TProjectile> createFunc)
                   where TAttack : Attack
                   where TProjectile : Projectile
                   where TProjectileProperties : ProjectileProperties<TProjectile>
        {
            if (projectileProperties?.projectilePrefab == null)
            {
                Debug.LogError($"Couldn't find a prefab for projectile based attack {attack?.name ?? "null"} of type {typeof(TProjectile).Name}. Check attack -> projectileProperties -> projectilePrefab");
                return false;
            }

            if (CanAttack(attack) && HaveEnoughAmmo(projectileProperties))
            {
                var createdProjectile = createFunc.Invoke();

                if (createdProjectile != null)
                {
                    attack.beforeAttack?.Invoke();
                    createdProjectile.attackerOnHit.onAttackHit += AfterAttackHit;
                    manaStats?.Decrease(attack.manaCost);
                    DecreseAttackAmmo<TProjectileProperties, TProjectile>(projectileProperties);

                    return true;
                }

                return false;
            }

            return false;
        }

        private void DecreseAttackAmmo<TProjectileProperties, TProjectile>(TProjectileProperties projectileProperties)
            where TProjectileProperties : ProjectileProperties<TProjectile>
            where TProjectile : Projectile
        {
            if (projectileProperties.maxAmmo != Mathf.Infinity && projectileProperties.maxAmmo != 0)
            {
                if (attacksAmmo.TryGetValue(projectileProperties.Id, out var currentAmmo))
                {
                    attacksAmmo[projectileProperties.Id] = Mathf.Max(currentAmmo - 1, 0);
                }
                else
                {
                    attacksAmmo[projectileProperties.Id] = (int)projectileProperties.maxAmmo - 1;
                }
            }
        }

        private bool HaveEnoughAmmo<TProjectile>(ProjectileProperties<TProjectile> projectileProperties)
            where TProjectile : Projectile
        {
            if (projectileProperties.maxAmmo == 0
                || projectileProperties.maxAmmo == Mathf.Infinity)
            {
                return true;
            }

            if (!attacksAmmo.ContainsKey(projectileProperties.Id))
            {
                return true;
            }

            return attacksAmmo[projectileProperties.Id] > 0;
        }
    }
}
