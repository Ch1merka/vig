using System;
using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Properties of targeted attack (will follow the target)
    /// </summary>
    [Serializable]
    public class TargetedProjectileProperties : ProjectileProperties<TargetedProjectile>
    {
        [Header("[Optional] Maximum distance from target to apply attack. Set to 0 / Infinity if there is no limit")]
        public float targetMaximumDistance = Mathf.Infinity;
    }
}