using System;
using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Targeted shot attack (directly to a given entity) from range
    /// </summary>
    [Serializable]
    public class RangedTargetedAttack : Attack
    {
        [Header("Properties of the shooted items")]
        public TargetedProjectileProperties projectileProperties;
    }
}
