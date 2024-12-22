using System;
using System.Collections.Generic;
using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// AI of enemies. It can:
    /// - Get close to the player (with a limit range)
    /// - Attack the player with close, free ranged and targeted attacks
    /// - Take damage and die
    /// - Deal damage and kill the player
    /// - Lose player's track if - it is too far away, it died, it is behind a certain layer or if too much time passed (ALL is configurable via editor / public variables
    /// - Move back to start when stopping to attack player (configurable)
    /// </summary>
    public class AttackerEnemy : Attacker
    {
        [Header("Player to follow and attack")]
        public Entity player;

        [Header("[Optional] Ranged attack starting point transform. Leave empty to use this transform's position")]
        public Transform rangedAttackStart;

        [Header("Closest distance to player")]
        [Range(1f, 20f)]
        public float minFollowDistance = 2f;

        [Header("Speed to move to player")]
        [Range(0f, 15)]
        public float speed = 0.6f;

        [Header("Speed to move to turn to movement direction")]
        [Range(0f, 20f)]
        public float turnSpeed = 2f;

        [Header("Seconds to wait between attacks")]
        public float secondsBetweenAttacks = 3f;

        [Header("Distance to stop follow target")]
        public float maxFollowDistance = Mathf.Infinity;

        [Header("Time to stop follow target (seconds)")]
        public float maxFollowTime = Mathf.Infinity;

        [Header("Layer that will make enemy lose player if it hides behind it")]
        public LayerMask hidingLayer = 0;

        [Header("Should move back to start position when stopping to attack")]
        public bool backToStartOnStopAttack = true;

        [Header("Time to delay moving back to start when stopping to attack")]
        public float backToStartDelay = 3f;

        private bool isAttackingTarget;
        private float secondsSinceStartedFollowing;
        private float secondsSinceLastAttacked;
        private float secondsSinceMoveToStart;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private bool isMovingBackToStart;
        private Quaternion targetRotationToStart;

        /// <summary>
        /// Stop attacking the player. Can also move back to start if backToStartOnStopAttack is true
        /// </summary>
        public void StopAttackingPlayer()
        {
            isMovingBackToStart = isMovingBackToStart || isAttackingTarget;
            secondsSinceMoveToStart = 0;

            isAttackingTarget = false;

            var relativePosition = startPosition - transform.position;

            if (relativePosition != Vector3.zero)
            {
                targetRotationToStart = Quaternion.LookRotation(relativePosition);
            }
            else
            {
                targetRotationToStart = transform.rotation;
            }
        }

        /// <summary>
        /// Start attacking the player
        /// </summary>
        public void AttackPlayer()
        {
            if (!isAttackingTarget && player?.IsAlive == true)
            {
                isAttackingTarget = true;
                secondsSinceStartedFollowing = 0;
                secondsSinceLastAttacked = 0;
            }
        }

        private void Start()
        {
            player ??= EntitiesLocator.Get<Entity>(EntitiesLocator.Player); // replace this with your DI / referencer to entity, or assign the target property (not recommended. This can be easily done automatically! see mincode asset)
            player ??= GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();

            if (player == null)
            {
                Debug.LogError($"{name} at {transform.position:F5}: Can't find a player to reference to");
            }

            player.onEntityDied += Player_onEntityDied;

            if (rangedAttackStart == null)
            {
                rangedAttackStart = transform;
            }

            startPosition = transform.position;
            startRotation = transform.rotation;
            var playerAttackTags = new List<string> { player.tag };

            foreach (var attack in GetAllAttacks())
            {
                attack.attackTags = playerAttackTags;
            }
        }

        private void Player_onEntityDied(Entity entity)
        {
            StopAttackingPlayer();
        }

        private void FixedUpdate()
        {
            UpdateAttackerEnemy();
        }

        protected virtual void UpdateAttackerEnemy()
        {
            base.UpdateEntity();

            if (player == null || !IsAlive)
            {
                return;
            }

            if (!isAttackingTarget)
            {
                if (backToStartOnStopAttack)
                {
                    BackToStart();
                }

                return;
            }

            secondsSinceStartedFollowing += Time.fixedDeltaTime;

            var distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (distanceToPlayer >= maxFollowDistance
                || secondsSinceStartedFollowing >= maxFollowTime
                || Physics.Linecast(transform.position, player.transform.position, hidingLayer))
            {
                StopAttackingPlayer();
                return;
            }

            LerpLookAt(player.transform.position);
            AttackMove(distanceToPlayer);
        }

        private void AttackMove(float distanceToPlayer)
        {
            var rangedAttack = GetRandomRangedAttack();
            var targetedAttack = GetRandomTargetedAttack();
            var closeAttack = GetRandomCloseAttack();

            if (lifeStats != null)
            {
                lifeStats.ResetIdleTime();
            }

            if (manaStats != null)
            {
                manaStats.ResetIdleTime();
            }

            if (distanceToPlayer >= Mathf.Min(minFollowDistance, closeAttack?.range ?? float.MaxValue))
            {
                if (!AttackIfNeeded(rangedAttack, targetedAttack, distanceToPlayer)
                    && !AttackIfNeeded(closeAttack, distanceToPlayer))
                {
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.fixedDeltaTime);
                }
            }
            else if (closeAttack != null)
            {
                AttackIfNeeded(closeAttack, distanceToPlayer);
            }
            else if (targetedAttack != null || rangedAttack != null)
            {
                AttackIfNeeded(rangedAttack, targetedAttack, distanceToPlayer);
            }
        }

        private void BackToStart()
        {
            if (!isMovingBackToStart)
            {
                return;
            }

            secondsSinceMoveToStart += Time.deltaTime;
            if (secondsSinceMoveToStart <= backToStartDelay)
            {
                return;
            }

            if (transform.position != startPosition)
            {
                if (transform.rotation != targetRotationToStart)
                {
                    LerpLookToStart();
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.fixedDeltaTime);

                    if (Vector3.Distance(startPosition, transform.position) <= 0.001f)
                    {
                        transform.position = startPosition;
                    }
                }
            }
            else if (transform.rotation != startRotation)
            {
                LerpRotate(startRotation);
            }
            else
            {
                isMovingBackToStart = false;
            }
        }

        private bool AttackIfNeeded(RangedAttack rangedAttack, RangedTargetedAttack targetedAttack, float distanceToPlayer)
        {
            if (rangedAttack == null && targetedAttack == null)
            {
                return false;
            }

            if (rangedAttack == null)
            {
                return AttackIfNeeded(targetedAttack, distanceToPlayer);
            }

            if (targetedAttack == null)
            {
                return AttackIfNeeded(rangedAttack, distanceToPlayer);
            }

            // attack a random ranged / targeted attack
            return UnityEngine.Random.Range(0, 2) == 0
                ? AttackIfNeeded(rangedAttack, distanceToPlayer)
                : AttackIfNeeded(targetedAttack, distanceToPlayer);
        }

        private bool AttackIfNeeded(Attack attack, float distanceToPlayer)
        {
            if (attack != null
                && secondsSinceLastAttacked >= secondsBetweenAttacks
                && !HasCloseAttackPending)
            {
                bool attacked = false;

                if (attack is RangedTargetedAttack targetedAttack)
                {
                    attacked = DoTargetedAttack(targetedAttack, player, rangedAttackStart.position);
                }
                else if (attack is RangedAttack rangedAttack)
                {
                    attacked = DoRangedAttack(rangedAttack, rangedAttackStart.position, transform.forward);
                }
                else if (attack is CloseAttack closeAttack && closeAttack.range >= distanceToPlayer)
                {
                    attacked = DoCloseAttack(closeAttack);
                }

                secondsSinceLastAttacked = attacked
                    ? 0
                    : secondsSinceLastAttacked + Time.fixedDeltaTime;

                return attacked;
            }
            else
            {
                secondsSinceLastAttacked += Time.fixedDeltaTime;

                return false;
            }
        }

        private bool LerpLookAt(Vector3 toLookAt)
        {
            toLookAt.y = transform.position.y;
            var relativePosition = toLookAt - transform.position;
            var toRotation = Quaternion.LookRotation(relativePosition);

            return LerpRotate(toRotation);
        }

        private bool LerpLookToStart()
        {
            return LerpRotate(targetRotationToStart);
        }

        private bool LerpRotate(Quaternion targetRotation)
        {
            if (targetRotation == transform.rotation)
            {
                return false;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) <= 1f)
            {
                transform.rotation = targetRotation;
            }

            return true;
        }
    }
}
