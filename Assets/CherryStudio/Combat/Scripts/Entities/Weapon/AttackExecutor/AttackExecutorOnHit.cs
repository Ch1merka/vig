using UnityEngine;
using UnityEngine.Events;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Logic class to apply a hit once a collision occurred
    /// </summary>
    public class AttackExecutorOnHit
    {
        /// <summary>
        /// Attack to apply on hit
        /// </summary>
        public Attack attack { get; protected set; }

        /// <summary>
        /// The entity that is doing the attack
        /// </summary>
        public Attacker attacker { get; protected set; }

        /// <summary>
        /// Unity time in which this attack started
        /// </summary>
        public float startTime { get; }

        public delegate void OnAttackHit(AttackExecutorOnHit executor, Entity attacked);
        public event OnAttackHit onAttackHit;

        private UnityAction onHitAction;
        private bool hitApplied;

        public AttackExecutorOnHit(Attack originalAttack, Attacker attackingEntity, UnityAction onHitAction = null)
        {
            if (attackingEntity != null && originalAttack != null)
            {
                attack = originalAttack;
                attacker = attackingEntity;
                this.onHitAction = onHitAction;
                startTime = Time.time;
            }
            else
            {
                Debug.LogError($"Invalid {nameof(AttackExecutorOnHit)} was attempted to be constructed. {nameof(attackingEntity)}: {attackingEntity?.name ?? "null"}, {nameof(originalAttack)}: {originalAttack?.name ?? "null"}");
            }
        }

        /// <summary>
        /// Apply attack's damage to the entity that collided
        /// </summary>
        /// <param name="collision">Collision from OnCollision method</param>
        /// <returns>True if attack was apllied, false if nothing applied</returns>
        public virtual bool DamageCollider(Collision collision)
        {
            return DamageCollider(collision.collider);
        }

        /// <summary>
        /// Apply attack's damage to the entity collider
        /// </summary>
        /// <param name="collider">Collider from Collision.collider / OnTrigger method</param>
        /// <returns>True if attack was apllied, false if nothing applied</returns>
        public virtual bool DamageCollider(Collider collider)
        {
            var collisionEntity = collider.GetComponent<Entity>() ?? collider.GetComponentInParent<Entity>() ?? collider.GetComponentInChildren<Entity>();

            if (collisionEntity != null
                && collisionEntity != attacker
                && collisionEntity.IsAlive
                && (attack.IsTagAttackable(collisionEntity.transform.tag) || attack.IsTagAttackable(collider.tag)))
            {
                collisionEntity.Damage(attack);
                attack.afterAttack?.Invoke();
                onHitAction?.Invoke();
                onAttackHit?.Invoke(this, collisionEntity);

                return true;
            }

            return false;
        }
    }
}
