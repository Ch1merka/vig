using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Simple script for the demo's player movement - moving according to rows and rotating to mouse
    /// </summary>
    public class ArrowsMover : MonoBehaviour
    {
        public Entity player;
        public bool isInteracting = true;
        public float speed = 1f;

        private Camera mainCamera;

        public void SetIsInteracting(bool newValue)
        {
            isInteracting = newValue;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!isInteracting || player?.IsAlive == false)
            {
                return;
            }

            LookToMouse();

            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(-Vector3.forward * speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(-Vector3.right * speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
        }

        private void LookToMouse()
        {
            var playerDistanceFromCamera = Vector3.Distance(mainCamera.transform.position, transform.position);
            var mousePosition = Input.mousePosition;
            mousePosition.z = playerDistanceFromCamera;

            var screenToWorld = mainCamera.ScreenToWorldPoint(mousePosition);
            var direction = new Vector3(screenToWorld.x, transform.position.y, screenToWorld.z);

            transform.LookAt(direction);
        }
    }
}
