using UnityEngine;
using UnityEngine.Events;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Apply a close attack when DamageCollider is called
    /// </summary>
    public class CloseAttackExecutorOnHit : AttackExecutorOnHit
    {
        public CloseAttackExecutorOnHit(Attack originalAttack, Attacker attackingEntity, UnityAction onHitAction = null)
            : base(originalAttack, attackingEntity, onHitAction)
        {
        }

        public bool TimeToLivePassed =>
            !(attack is CloseAttack closeAttack)
            || Time.time >= startTime + (Mathf.Max(closeAttack.maxHitTimeSeconds, 0.3f));

        /// <summary>
        /// Apply attack's damage to the entity collider
        /// </summary>
        /// <param name="collider">Collider from Collision.collider / OnTrigger method</param>
        /// <returns>True if attack was apllied, false if nothing applied</returns>
        public override bool DamageCollider(Collider collider)
        {
            if (TimeToLivePassed)
            {
                return false;
            }

            return base.DamageCollider(collider);
        }
    }
}