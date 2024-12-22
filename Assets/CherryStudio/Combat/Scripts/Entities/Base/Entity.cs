using UnityEngine;
using UnityEngine.Events;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Basic entity that can take damage (to spawn an attacking entity, use Attacker)
    /// </summary>
    public class Entity : MonoBehaviour
    {
        [Header("[Optional] Life properties")]
        public ObservableBarStats lifeStats;

        [Header("[Optional] Actions when life bar empty")]
        public UnityEvent onDeath;

        [Header("[Optional] Actions when taking damage")]
        public UnityEvent onAttacked;

        public delegate void OnEntityDied(Entity entity);

        public event OnEntityDied onEntityDied;

        public bool IsAlive => lifeStats == null || lifeStats?.currentValue > 0;

        /// <summary>
        /// Apply an attack on this entity
        /// </summary>
        /// <param name="attack">Attack to apply its damage on this entity</param>
        public void Damage(Attack attack)
        {
            var damage = attack.GetDamage();

            Damage(damage);
        }

        /// <summary>
        /// Apply damage count to this entity
        /// </summary>
        /// <param name="damage">How much damage to do to this entity</param>
        public void Damage(int damage)
        {
            lifeStats?.Decrease(damage);
            onAttacked?.Invoke();
            AfterDamaged();

            if (lifeStats?.currentValue == 0)
            {
                onDeath?.Invoke();
                onEntityDied?.Invoke(this);
                AfterDeath();
            }
        }

        /// <summary>
        /// Restore health
        /// </summary>
        /// <param name="life">Health points count</param>
        public void Heal(int life)
        {
            lifeStats?.Increase(life);
        }

        /// <summary>
        /// Restore health to its maximum
        /// </summary>
        public void HealFully()
        {
            lifeStats?.RestoreFully();
        }

        protected virtual void UpdateEntity()
        {
            lifeStats?.UpdateStatsFrame(Time.deltaTime);
        }

        protected virtual void AfterDamaged()
        {
        }

        protected virtual void AfterDeath()
        {
        }

        private void Update()
        {
            UpdateEntity();
        }
    }
}
