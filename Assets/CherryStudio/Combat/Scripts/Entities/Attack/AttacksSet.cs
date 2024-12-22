using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Set of attack moves
    /// </summary>
    /// <typeparam name="T">Attack type (like CloseAttack, RangedAttack)</typeparam>
    [Serializable]
    public class AttacksSet<T> where T : Attack
    {
        [Header("Attack moves")]
        public List<T> attacks;
        private Dictionary<string, T> attacksByName;

        public void InitializeAttacks()
        {
            var attackGroupedByName = attacks.GroupBy(attack => attack.name);

            attacksByName = attackGroupedByName
                .ToDictionary(
                k => k.Key,
                v =>
                {
                    if (v.Count() > 1)
                    {
                        Debug.LogWarning($"Found duplicate attacks with name '{v.Key}'. Will reference only the first one.");
                    }

                    return v.First();
                });
        }

        public T GetRandomAttack()
        {
            if (!attacks.Any())
            {
                return null;
            }

            var index = Random.Range(0, attacks.Count);

            return attacks[index];
        }

        public T GetAttack(string attackName)
        {
            T attack = null;

            if (attacksByName == null)
            {
                Debug.LogWarning($"{nameof(AttacksSet<T>)} was not initialized. Attacks finding without initialization is not performant! Please invoke {nameof(InitializeAttacks)}");
                attack = attacks.FirstOrDefault(attack => attack.name == attackName);
            }
            else if (!attacksByName.TryGetValue(attackName, out attack))
            {
                attack = attacks.FirstOrDefault(attack => attack.name == attackName);
                if (attack != null)
                {
                    Debug.LogWarning($"{nameof(AttacksSet<T>)} was changed in runtime and was not initialized. Attacks finding without initialization is not performant! Please invoke {nameof(InitializeAttacks)}");
                }
            }

            if (attack == null)
            {
                Debug.LogError($"{nameof(AttacksSet<T>)}: Can't find attack with name '{attackName}'");
            }

            return attack;
        }

        public T GetAttack(int attackIndex)
        {
            var validIndex = Mathf.Clamp(attackIndex, 0, attacks.Count - 1);

            if (validIndex != attackIndex)
            {
                Debug.LogError($"{nameof(AttacksSet<T>)}: Can't find attack index '{attackIndex}'. Using index {validIndex}");
            }

            return attacks[validIndex];
        }
    }
}
