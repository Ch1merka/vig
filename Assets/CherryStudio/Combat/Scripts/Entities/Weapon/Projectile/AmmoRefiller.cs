using UnityEngine;
using UnityEngine.Events;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Refill ammo on collision
    /// </summary>
    public class AmmoRefiller : MonoBehaviour
    {
        [Header("Ammo count that this will refill. Set to 0 / Infinity to refill all")]
        public int ammoCount;

        [Header("[Optional] Attack that this ammo matches. Leave empty to match all")]
        public string attackName;

        [Header("[Optional] Attacker that can collect this. Leave empty to allow all")]
        public Attacker attacker;

        [Header("[Optional] Actions to do after collected")]
        public UnityEvent onCollected;

        private bool wasCollected;

        public void DestroySelf()
        {
            if (this != null)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Collect(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            Collect(other);
        }

        private void Collect(Collider collider)
        {
            var collector = collider?.GetComponent<Attacker>() ?? collider?.GetComponentInParent<Attacker>() ?? collider?.GetComponentInChildren<Attacker>();

            if (!wasCollected
                && collector != null
                && (attacker == null || collector == attacker))
            {
                bool wasRefilled;

                if (string.IsNullOrEmpty(attackName))
                {
                    wasRefilled = collector.RefillAllAmmosReturnState();
                }
                else
                {
                    wasRefilled = collector.RefillAmmo(ammoCount, collector.GetAttack(attackName));
                }

                if (wasRefilled)
                {
                    onCollected?.Invoke();
                    wasCollected = true;
                }
            }
        }
    }
}
