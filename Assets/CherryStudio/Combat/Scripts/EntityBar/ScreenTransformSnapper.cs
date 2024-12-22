using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Set the transform's position according to another transform, for screen components (usually used to snap stats bars like the life bar).
    /// This is a simple demonstration script that can be extended to make the GUI look better.
    /// </summary>
    public class ScreenTransformSnapper : MonoBehaviour
    {
        [Header("Transform to snap to in world")]
        public Transform snap;

        [Header("[Optional] Offset from snap position")]
        public Vector3 offset;

        private Camera mainCamera;

        protected void Awake()
        {
            if (snap == null)
            {
                Debug.LogError($"{nameof(ScreenTransformSnapper)} '{name}' Missing snap configuration");
                return;
            }

            mainCamera = Camera.main;
            SetPosition();
        }

        private void LateUpdate()
        {
            if (snap == null)
            {
                return;
            }

            SetPosition();
        }

        private void SetPosition()
        {
            var worldPosition = snap.position + offset;
            transform.position = mainCamera.WorldToScreenPoint(worldPosition);
        }
    }
}
