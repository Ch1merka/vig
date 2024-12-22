using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// An enemy that will start to attack if the player hit him or if it's close to him
    /// </summary>
    public class FightingRangeEnemy : FightingBackEnemy
    {
        [Header("Radius to start attacking at")]
        [Range(0f, 200f)]
        public float rangeToAttack = 5f;

        void Update()
        {
            base.UpdateAttackerEnemy();

            if (player == null || !player.IsAlive)
            {
                return;
            }

            if (Vector3.Distance(player.transform.position, transform.position) <= rangeToAttack
                && !Physics.Linecast(transform.position, player.transform.position, hidingLayer))
            {
                AttackPlayer();
            }
        }
    }
}
