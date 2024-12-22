using System;
using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Settings of a projectile
    /// </summary>
    /// <typeparam name="T">Projectile class</typeparam>
    [Serializable]
    public abstract class ProjectileProperties<T> where T : Projectile
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        [Header("Prefab of each projectile")]
        public T projectilePrefab;

        [Header("[Optional] Parent transform of the projectiles")]
        public Transform projectilesParent;

        [Header("[Optional] Maximum ammo capacity. Set to 0 / Infinity for no limitation")]
        public float maxAmmo = Mathf.Infinity;
    }
}
