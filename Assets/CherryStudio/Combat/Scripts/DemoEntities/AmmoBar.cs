using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// GUI controller to show how many ammo the entity has on an attack
    /// </summary>
    public class AmmoBar : MonoBehaviour
    {
        [Header("[Optional] Attack name to show its ammo")]
        public string attackName;

        [Header("[Optional] Entity that owns the attack")]
        public Attacker entity;

        [Header("[Optional] GUI element of each bullet")]
        public GameObject bulletUnitPrefab;

        private int lastFrameCount;

        private void Update()
        {
            if (entity == null || bulletUnitPrefab == null || string.IsNullOrEmpty(attackName))
            {
                return;
            }

            var attack = entity.GetAttack(attackName);
            if (attack == null)
            {
                return;
            }

            var ammoCount = entity.GetAmmoCount(attack);

            if (lastFrameCount != ammoCount)
            {
                lastFrameCount = ammoCount;

                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                for (var i = 0; i < ammoCount; i++)
                {
                    Instantiate(bulletUnitPrefab, transform);
                }
            }
        }
    }
}
