using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CherryStudio.Combat
{
    /// <summary>
    /// When collided, restores stats to the colliding entity (usually used in potions)
    /// </summary>
    public class StatsRestorer : MonoBehaviour
    {
        [Header("Stats affected")]
        public ObservableBarStats statsToRestore;

        [Header("Actions to apply when collected.")]
        public UnityEvent onCollected;

        [Header("How much will be restored")]
        public int amount = 10;

        [Header("Tags that can collect this")]
        public List<string> tags = new List<string> { "Player" };

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
            if (!statsToRestore.IsFull && (tags == null || tags.Count == 0 || tags.Contains(collider.tag)))
            {
                onCollected?.Invoke();
                statsToRestore?.Increase(amount);
            }
        }
    }
}
