using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace CherryStudio.Combat
{
    [Serializable]
    public class MinimumMaximum
    {
        public int minimum = 3;
        public int maximum = 6;
    }

    [Serializable]
    public class CriticalHit
    {
        [Header("Chance between 0-1 to get critical hit")]
        [Range(0f, 1f)]
        public float criticalHitProbability = 0;

        [Header("Extra damage range if the attack is critical hit")]
        public MinimumMaximum criticalExtraDamage = new MinimumMaximum { minimum = 0, maximum = 0 };

        [Header("[Optional] Actions to do on critical hit (effects can be triggered here)")]
        public UnityEvent onCriticalHit;
    }

    [Serializable]
    public abstract class Attack
    {
        [Header("Attack identifier")]
        public string name = "attack";

        [Header("Attack damage range")]
        public MinimumMaximum damage;

        [Header("[Optional] Tags that this attack can be applied to. Leave empty to consider all")]
        public List<string> attackTags;

        [Header("[Optional] If a hit can be critical, set its properties")]
        public CriticalHit criticalHitProperties;

        [Header("[Optional] How much mana the attack reduces")]
        public int manaCost;

        [Header("[Optional] Actions to apply before attacking")]
        public UnityEvent beforeAttack;

        [Header("[Optional] Actions to apply after attacking")]
        public UnityEvent afterAttack;

        public bool IsTagAttackable(string tag)
        {
            return attackTags == null
                || !attackTags.Any()
                || attackTags.Contains(tag);
        }

        /// <summary>
        /// Get the attack's damage, by calculating critical hits and the damange range
        /// </summary>
        /// <returns>Damage amount that considers all the attack's variables</returns>
        public int GetDamage()
        {
            var chance = Random.Range(0f, 1f);

            if (chance <= criticalHitProperties.criticalHitProbability)
            {
                criticalHitProperties.onCriticalHit?.Invoke();
                return GetCriticalHitDamage();
            }
            else
            {
                return GetNonCriticalDamage();
            }
        }

        /// <summary>
        /// Get damage according to basic damage range (ignore critical hit)
        /// </summary>
        /// <returns>Basic damage amount</returns>
        public int GetNonCriticalDamage()
        {
            return Random.Range(damage.minimum, damage.maximum);
        }

        /// <summary>
        /// Get critical hit damage, including basic damage
        /// </summary>
        /// <returns>Damage amount + extra critical hit damage</returns>
        public int GetCriticalHitDamage()
        {
            return GetNonCriticalDamage() + Random.Range(criticalHitProperties.criticalExtraDamage.minimum, criticalHitProperties.criticalExtraDamage.maximum);
        }
    }
}
