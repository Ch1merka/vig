using UnityEngine;
using UnityEngine.Events;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Base projectiles logic class, that handles collisions and damaging
    /// </summary>
    public abstract class Projectile : MonoBehaviour
    {
        [Header("Movement speed")]
        public float speed = 2;

        [Header("Should auto destroy self after hit")]
        public bool destroyAfterHit = true;

        [Header("[Optional] Actions to apply upon hit")]
        public UnityEvent onDamaging;

        [Header("[Optional] Distance that after passed the projectile will destroy itself")]
        public float maximumDistance = Mathf.Infinity;

        public AttackExecutorOnHit attackerOnHit { get; protected set; }

        private Vector3 positionAtStart;
        private bool wasHit;

        public void DestroySelf()
        {
            if (this != null)
            {
                Destroy(gameObject);
            }
        }

        protected abstract void MovementUpdate();

        private void Awake()
        {
            positionAtStart = transform.position;
        }

        private void FixedUpdate()
        {
            if (wasHit)
            {
                return;
            }

            if (maximumDistance == Mathf.Infinity
                || Vector3.Distance(transform.position, positionAtStart) <= maximumDistance)
            {
                MovementUpdate();
            }
            else
            {
                DestroySelf();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            DamageCollider(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            DamageCollider(other);
        }

        private void DamageCollider(Collider collider)
        {
            if (attackerOnHit == null)
            {
                Debug.LogError($"{nameof(Projectile)} '{name}' is missing {nameof(attackerOnHit)}. Make sure it was initialized or use the static Create method");
                return;
            }

            if (collider.transform == attackerOnHit.attacker?.transform)
            {
                return;
            }

            if (attackerOnHit.DamageCollider(collider))
            {
                wasHit = true;
                onDamaging?.Invoke();

                if (destroyAfterHit)
                {
                    DestroySelf();
                }
            }
        }
    }
}