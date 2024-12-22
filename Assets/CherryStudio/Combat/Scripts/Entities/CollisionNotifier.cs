using System;
using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Find's game objects collider and passes collisions to it. You can use it on any game object.
    /// It is useful when you want to separate the scripts that handle collisions from the actual collider game object.
    /// </summary>
    public class CollisionNotifier : MonoBehaviour
    {
        private Action<Collider> onTrigger;
        private Action<Collision> onCollision;

        public static void RegisterChildren(GameObject registerTo, Action<Collider> descriptor)
        {
            RegisterChildren(registerTo, (collisionNotifier) => collisionNotifier.SetCallback(descriptor));
        }

        public static void RegisterChildren(GameObject registerTo, Action<Collision> descriptor)
        {
            RegisterChildren(registerTo, (collisionNotifier) => collisionNotifier.SetCallback(descriptor));
        }

        public void SetCallback(Action<Collision> descriptor)
        {
            onCollision = descriptor;
        }

        public void SetCallback(Action<Collider> descriptor)
        {
            onTrigger = descriptor;
        }

        private static void RegisterChildren(GameObject gameObject, Action<CollisionNotifier> setMethod)
        {
            var childrenColliders = gameObject.GetComponentsInChildren<Collider>();

            foreach (var childCollider in childrenColliders)
            {
                var collisionNotifier = childCollider.gameObject.AddComponent<CollisionNotifier>();
                setMethod.Invoke(collisionNotifier);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            onCollision?.Invoke(collision);
            onTrigger?.Invoke(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            onTrigger?.Invoke(other);
        }
    }
}
