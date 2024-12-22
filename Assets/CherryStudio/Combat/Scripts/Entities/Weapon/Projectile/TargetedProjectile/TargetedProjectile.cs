using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Projectile controller for targeted attacks. Will follow the target until destroyed
    /// </summary>
    public class TargetedProjectile : Projectile
    {
        private Entity target;

        /// <summary>
        /// Instantiates a forward projectile
        /// </summary>
        /// <param name="attack">Ranged attack to perform</param>
        /// <param name="startPosition">Where to set the projectile (usually the gun tip's position)</param>
        /// <param name="direction">Forward of this projectile (direction to move to)</param>
        /// <param name="attacker">Attacking entity</param>
        /// <param name="targetedAttackMaximumDistance">Maximum target that this targeted attack can be applied from</param>
        /// <returns>Created projectile. Will be null if nothing instantiated</returns>
        public static TargetedProjectile Create(Entity target, RangedTargetedAttack attack, Vector3 startPosition, Attacker attacker, float targetedAttackMaximumDistance)
        {
            if (target == null
                || attack == null
                || attacker == null
                || (targetedAttackMaximumDistance != 0
                    && targetedAttackMaximumDistance != Mathf.Infinity
                    && Vector3.Distance(target.transform.position, attacker.transform.position) > targetedAttackMaximumDistance))
            {
                return null;
            }

            var created = Instantiate(
                attack.projectileProperties.projectilePrefab,
                startPosition,
                Quaternion.identity,
                attack.projectileProperties.projectilesParent);

            created.attackerOnHit = new AttackExecutorOnHit(attack, attacker);
            created.transform.LookAt(target.transform);
            created.target = target;

            return created;
        }

        protected override void MovementUpdate()
        {
            if (!target.IsAlive)
            {
                DestroySelf();
            }

            if (target != null)
            {
                transform.LookAt(target.transform);
            }

            transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        }
    }
}