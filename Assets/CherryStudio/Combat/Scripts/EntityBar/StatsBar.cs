using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CherryStudio.Combat
{
    /// <summary>
    /// GUI controller to show a given stats bar (like the life bar) using a slider
    /// </summary>
    public class StatsBar : MonoBehaviour
    {
        public Slider slider;
        public Gradient gradient;
        public Image fill;

        [Header("Referred stats")]
        public ObservableBarStats stats;

        [Range(0f, 2f)]
        [Header("Time to change life interface. To turn off set to 0")]
        public float fillChangeTime = 0.2f;

        [Header("[Optional] Text of the stats (for example: 56 / 100)")]
        public Text statsText;

        [Header("[Optional] Change the text color to black / white according to fill color")]
        public bool changeTextColor;

        [Header("[Optional] Actions when starting")]
        public UnityEvent onStart;

        [Header("[Optional] Time that if value didn't change, will trigger below actions")]
        public float idleTime = Mathf.Infinity;

        [Header("[Optional] Actions to do when value is same for a while")]
        public UnityEvent onIdle;

        [Header("[Optional] Actions to do when value changes after a while")]
        public UnityEvent onOutOfIdle;

        private float targetValue;
        private float oldValue;
        private float timeSinceValueChange;
        private bool appliedOnIdleActions;
        private bool appliedOutOfIdleActions;

        protected virtual void Start()
        {
            Validate(printError: true);

            var frameValue = Mathf.Clamp((float)stats.currentValue / stats.maximumValue, 0f, 1f);
            SetSliderValue(frameValue);

            onStart?.Invoke();
            appliedOnIdleActions = true;
            appliedOutOfIdleActions = false;
        }

        private void Update()
        {
            if (!Validate(printError: false))
            {
                return;
            }

            var frameValue = Mathf.Clamp((float)stats.currentValue / stats.maximumValue, 0f, 1f);

            if (frameValue != targetValue)
            {
                oldValue = slider.value;
                targetValue = frameValue;
                timeSinceValueChange = 0;
            }

            if (slider.value != targetValue)
            {
                if (fillChangeTime <= 0)
                {
                    SetSliderValue(targetValue);
                }
                else
                {
                    timeSinceValueChange += Time.deltaTime;
                    SetSliderValue(Mathf.Lerp(oldValue, targetValue, timeSinceValueChange / fillChangeTime));
                }

                if (Mathf.Abs(slider.value - targetValue) <= 0.0001f)
                {
                    SetSliderValue(targetValue);
                    oldValue = frameValue;
                }
            }

            if (idleTime != Mathf.Infinity && !appliedOnIdleActions)
            {
                if (stats.idleTime >= idleTime)
                {
                    appliedOutOfIdleActions = false;
                    appliedOnIdleActions = true;
                    onIdle?.Invoke();
                }
            }
        }

        private void SetSliderValue(float newValue)
        {
            if (slider != null)
            {
                slider.value = Mathf.Clamp(newValue, 0f, 1f);

                if (fill != null && gradient != null)
                {
                    fill.color = gradient.Evaluate(slider.value);
                }

                if (statsText != null)
                {
                    if (changeTextColor && fill != null)
                    {
                        var blackDiff = CalculateColorDifference(Color.black, fill.color);
                        var whiteDiff = CalculateColorDifference(Color.white, fill.color);

                        statsText.color = blackDiff > whiteDiff ? Color.black : Color.white;
                    }

                    statsText.text = $"{stats.currentValue} / {stats.maximumValue}";
                }

                if (idleTime != Mathf.Infinity && !appliedOutOfIdleActions)
                {
                    onOutOfIdle?.Invoke();
                    appliedOnIdleActions = false;
                    appliedOutOfIdleActions = true;
                }
            }
        }

        private float CalculateColorDifference(Color left, Color right)
        {
            return Mathf.Abs(
                Average(
                    left.r - right.r,
                    left.g - right.g,
                    left.b - right.b));
        }

        private float Average(params float[] arguments)
        {
            if (arguments?.Length == 0)
            {
                return 0;
            }

            return arguments.Sum(a => a) / arguments.Length;
        }

        private bool Validate(bool printError)
        {
            if (stats == null)
            {
                if (printError)
                {
                    Debug.LogError($"{name} at {transform.position:F5} {nameof(StatsBar)}: Missing stats property");
                }

                return false;
            }

            if (slider == null)
            {
                if (printError)
                {
                    Debug.LogError($"{name} at {transform.position:F5} {nameof(StatsBar)}: Missing slider property");
                }

                return false;
            }

            return true;
        }
    }
}
