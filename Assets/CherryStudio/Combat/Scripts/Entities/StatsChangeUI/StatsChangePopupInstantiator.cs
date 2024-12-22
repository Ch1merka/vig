using UnityEngine;
using System;

namespace CherryStudio.Combat
{
    [Serializable]
    public enum StatsChangeDetection
    {
        Decrese = 0,
        Increase = 1,
        Both = 2
    }

    /// <summary>
    /// Listens to stats, and when it changes, creates a popup GUI that will show the change in stats
    /// </summary>
    public class StatsChangePopupInstantiator : MonoBehaviour
    {
        public Vector3 minimumPosition;
        public Vector3 maximumPosition;
        public StatsChangePopup popupPrefab;
        public ObservableBarStats stats;
        public StatsChangeDetection changeDetection;

        private void Start()
        {
            stats ??= GetComponentInParent<ObservableBarStats>();

            if (stats == null)
            {
                Debug.LogError($"{name} Stats Change Popup Instantiator is missing stats reference.");
            }
            else
            {
                stats.CurrentValueChanged += Stats_CurrentValueChanged;
            }
        }

        private void Stats_CurrentValueChanged(float oldValue, float newValue)
        {
            var delta = newValue - oldValue;

            if (delta == 0)
            {
                return;
            }

            switch (changeDetection)
            {
                case StatsChangeDetection.Decrese:
                    CreatePopup(delta < 0, delta);
                    break;
                case StatsChangeDetection.Increase:
                    CreatePopup(delta > 0, delta);
                    break;
                case StatsChangeDetection.Both:
                    CreatePopup(shouldCreate: true, delta);
                    break;
            }
        }

        private void CreatePopup(bool shouldCreate, float delta)
        {
            if (shouldCreate)
            {
                var position = new Vector3(
                    GetRandomAxisValue(v => v.x),
                    GetRandomAxisValue(v => v.y),
                    GetRandomAxisValue(v => v.z));

                var popup = Instantiate(popupPrefab, transform);
                var textPrefix = delta < 0 ? "-" : "+";
                popup.transform.localPosition = position;
                popup.text.text = $"{textPrefix}{Mathf.Abs(delta):N0}";
            }
        }

        private float GetRandomAxisValue(Func<Vector3, float> axisDescriptor)
        {
            return UnityEngine.Random.Range(axisDescriptor(minimumPosition), axisDescriptor(maximumPosition));
        }
    }
}
