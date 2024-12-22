using System;
using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Free shot attack that is ranged
    /// </summary>
    [Serializable]
    public class RangedAttack : Attack
    {
        [Header("Properties of the shooted items")]
        public ForwardProjectileProperties projectileProperties;
    }
}
