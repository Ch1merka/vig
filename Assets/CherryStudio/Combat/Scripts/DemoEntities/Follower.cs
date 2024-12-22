using UnityEngine;

namespace Cherry.RunnerMovement
{
    /// <summary>
    /// Simple camera movement script that will follow the player
    /// </summary>
    public class Follower : MonoBehaviour
    {
        [Header("Speed to move to followed position")]
        public float speed = 1;

        [Header("Follow entity")]
        public Transform followed;

        [Header("Is following the entity")]
        public bool isFollowing = true;

        private Vector3 distanceFromFollowed;

        public void SetIsFollowing(bool newValue)
        {
            isFollowing = newValue;
        }

        private void Start()
        {
            distanceFromFollowed = transform.position;

            if (followed == null)
            {
                Debug.LogError($"{nameof(followed)} is empty!");
                SetIsFollowing(false);
            }
        }

        void FixedUpdate()
        {
            if (isFollowing)
            {
                var targetPosition = followed.position + distanceFromFollowed;

                transform.position = targetPosition;
            }
        }
    }
}
