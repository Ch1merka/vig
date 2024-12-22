using UnityEngine;

namespace CherryStudio.Combat
{
    /// <summary>
    /// Simple GUI to show the damage done. Replace TextMesh with TextMeshPro for better looking GUI. TextMesh to keep this package free of dependencies
    /// </summary>
    public class StatsChangePopup : MonoBehaviour
    {
        public float timeToLive = 1f;
        public float timeToFade = 0.2f;
        public TextMesh text;

        private Color fadeTargetColor;
        private Color fadeStartColor;
        private Color originalColor;
        private float timeAlive;
        private float alphaLerpTime;
        private bool isFading;
        private Camera mainCamera;

        public void DestroySelf()
        {
            if (this != null)
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            text ??= GetComponent<TextMesh>() ?? GetComponentInChildren<TextMesh>();
            mainCamera = Camera.main;
            timeToFade = Mathf.Max(0.001f, timeToFade);
            originalColor = text.color;
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, a: 0);

            fadeStartColor = text.color;
            fadeTargetColor = originalColor;
            alphaLerpTime = 0;
            timeAlive = 0;
            isFading = true;
        }

        public void Update()
        {
            // horizontal to camera
            transform.LookAt(mainCamera.transform.position);
            transform.eulerAngles = new Vector3(-transform.eulerAngles.x, mainCamera.transform.eulerAngles.y + 90, transform.eulerAngles.z + 90);

            if (timeToFade == 0)
            {
                text.color = new Color(originalColor.r, originalColor.g, originalColor.b, a: originalColor.a);
            }

            timeAlive += Time.deltaTime;

            if (alphaLerpTime >= timeToFade)
            {
                isFading = false;
                text.color = fadeTargetColor;
            }

            if (isFading)
            {
                alphaLerpTime += Time.deltaTime;
                text.color = Color.Lerp(fadeStartColor, fadeTargetColor, alphaLerpTime / timeToFade);
            }
            else if (timeAlive >= timeToLive)
            {
                DestroySelf();
            }
            else if (timeAlive + timeToFade >= timeToLive)
            {
                isFading = true;
                alphaLerpTime = 0f;
                fadeStartColor = text.color;
                fadeTargetColor = new Color(fadeStartColor.r, fadeStartColor.g, fadeStartColor.b, a: 0);
            }
        }
    }
}