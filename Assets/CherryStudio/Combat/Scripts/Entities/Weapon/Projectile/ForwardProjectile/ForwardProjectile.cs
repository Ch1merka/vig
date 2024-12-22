using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Projectile controller for free ranged attacks. Will move forwards until destroyed
    /// </summary>
    public class ForwardProjectile : Projectile
    {
        /// <summary>
        /// Instantiates a forward projectile
        /// </summary>
        /// <param name="attack">Ranged attack to perform</param>
        /// <param name="startPosition">Where to set the projectile (usually the gun tip's position)</param>
        /// <param name="direction">Forward of this projectile (direction to move to)</param>
        /// <param name="attacker">Attacking entity</param>
        /// <returns>Created projectile</returns>
        public static ForwardProjectile Create(RangedAttack attack, Vector3 startPosition, Vector3 direction, Attacker attacker)
        {
            var created = Instantiate(
                attack.projectileProperties.projectilePrefab,
                startPosition,
                Quaternion.identity,
                attack.projectileProperties.projectilesParent);

            created.attackerOnHit = new AttackExecutorOnHit(attack, attacker);
            created.transform.forward = direction;

            return created;
        }

        protected override void MovementUpdate()
        {
            transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        }
    }
}