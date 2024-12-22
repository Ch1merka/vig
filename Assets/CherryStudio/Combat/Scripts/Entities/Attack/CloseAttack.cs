using System;
using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Close combat attack (melee)
    /// </summary>
    [Serializable]
    public class CloseAttack : Attack
    {
        [Header("Maximum time to wait for collision after hit in seconds")]
        public float maxHitTimeSeconds = 0.3f;

        [Header("Range of the attack")]
        public float range = 0.5f;
    }
}
