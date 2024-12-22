using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CherryStudio.Combat
{
    [Serializable]
    public class BelowValueAction
    {
        [Range(0f, 1f)]
        [Header("Bar percentage value that once current value is less or eqauls to it, apply action")]
        public float value;
        public UnityEvent action;
    }

    /// <summary>
    /// Logic to track stats and notify when it changes
    /// </summary>
    public class ObservableBarStats : MonoBehaviour
    {
        public delegate void ValueChangedDelegate(float oldValue, float newValue);
        public event ValueChangedDelegate CurrentValueChanged;
        public event ValueChangedDelegate MaximumValueChanged;

        [Header("[Optional] Actions to apply when current value is less or equal to specified value")]
        public List<BelowValueAction> belowValueAction;

        /// <summary>
        /// Maximum stats value
        /// </summary>
        public int maximumValue = 100;

        /// <summary>
        /// Current stats value
        /// </summary>
        public int currentValue { get; private set; }

        /// <summary>
        /// Time that stats didn't change
        /// </summary>
        public float idleTime { get; private set; }

        private float lastFrameMaximumValue;
        private float lastFrameCurrentValue;
        private float lastCurrentValueEventRaised;
        private float lastMaximumValueEventRaised;

        public bool IsFull => maximumValue == currentValue;

        private void Awake()
        {
            maximumValue = Mathf.Abs(maximumValue);
            currentValue = maximumValue;

            lastFrameMaximumValue = maximumValue;
            lastFrameCurrentValue = currentValue;

            lastMaximumValueEventRaised = maximumValue;
            lastCurrentValueEventRaised = currentValue;
        }

        /// <summary>
        /// Set Idle time to 0
        /// </summary>
        public void ResetIdleTime()
        {
            idleTime = 0;
        }

        /// <summary>
        /// Can be called each Update to listen to changes each frame
        /// </summary>
        /// <param name="deltaTime">Time passed since last frame (usually Time.deltaTime or Time.fixedDeltaTime)</param>
        public void UpdateStatsFrame(float deltaTime)
        {
            idleTime = Mathf.Clamp(idleTime + deltaTime, 0f, float.MaxValue);

            if (lastFrameCurrentValue != currentValue && lastCurrentValueEventRaised != currentValue)
            {
                CurrentValueChanged?.Invoke(lastFrameCurrentValue, currentValue);
                ResetIdleTime();
            }

            if (lastFrameMaximumValue != maximumValue && lastMaximumValueEventRaised != maximumValue)
            {
                MaximumValueChanged?.Invoke(lastFrameMaximumValue, maximumValue);
            }

            lastFrameMaximumValue = maximumValue;
            lastFrameCurrentValue = currentValue;
        }

        /// <summary>
        /// Change the stats current value
        /// </summary>
        /// <param name="newValue">New value to set to</param>
        public void SetCurrentValue(int newValue)
        {
            if (maximumValue == 0)
            {
                Debug.LogError($"Stats maximum value must be greater than 0. Maximum value: {maximumValue}");
                return;
            }

            if (currentValue != newValue)
            {
                var oldValue = currentValue;
                currentValue = Mathf.Clamp(newValue, 0, maximumValue);
                CurrentValueChanged?.Invoke(oldValue, currentValue);
                ResetIdleTime();
                lastCurrentValueEventRaised = currentValue;

                var statsPercentage = (float)newValue / maximumValue;
                var actions = belowValueAction.Where(v => statsPercentage <= v.value).Select(v => v.action);
                foreach (var action in actions)
                {
                    action.Invoke();
                }
            }
        }

        /// <summary>
        /// Fill stats to its maximum value
        /// </summary>
        public void RestoreFully()
        {
            SetCurrentValue(maximumValue);
        }

        /// <summary>
        /// Change maximum value, and set current value accordingly (keep it relative)
        /// </summary>
        /// <param name="newValue">New maximum</param>
        public void SetMaximumValue(int newValue)
        {
            if (newValue <= 0)
            {
                Debug.LogError($"Stats maximum value must be greater than 0. Given value: {newValue}");
                return;
            }

            if (currentValue != newValue)
            {
                // Set current value relatively to the newValue to keep bar percentage
                var oldValue = maximumValue;
                currentValue = (currentValue / maximumValue) * newValue;
                maximumValue = Mathf.Abs(newValue);
                MaximumValueChanged?.Invoke(oldValue, currentValue);
                lastMaximumValueEventRaised = maximumValue;
            }
        }

        /// <summary>
        /// Substract from current value (usually used to apply damage)
        /// </summary>
        /// <param name="decrease">Amount to decrease</param>
        public void Decrease(int decrease)
        {
            SetCurrentValue(currentValue - decrease);
        }

        /// <summary>
        /// Add to current value (usually used to heal)
        /// </summary>
        /// <param name="increase">Amount to increase</param>
        public void Increase(int increase)
        {
            SetCurrentValue(currentValue + increase);
        }
    }
}
