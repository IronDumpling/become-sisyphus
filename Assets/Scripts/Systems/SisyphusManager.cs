using UnityEngine;
using System;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Systems
{
    public class SisyphusManager : MonoBehaviour, ISystem
    {
        public float MentalStrength { get; private set; }
        public int BrainCapacity { get; private set; }
        public int MaxBrainCapacity { get; private set; }
        public float MentalStrengthRegenRate { get; private set; }
        
        public event Action<float> OnMentalStrengthChanged;
        public event Action<int> OnBrainCapacityChanged;
        public event Action OnMentalStrengthDepleted;

        public void Initialize()
        {
            MentalStrength = 100f;
            MaxBrainCapacity = 100;
            BrainCapacity = MaxBrainCapacity;
            MentalStrengthRegenRate = 1f;
        }

        public void Update()
        {
            // 精神力自然恢复
            if (MentalStrength < 100f)
            {
                MentalStrength = Mathf.Min(100f, MentalStrength + MentalStrengthRegenRate * Time.deltaTime);
                OnMentalStrengthChanged?.Invoke(MentalStrength);
            }
        }

        public void ConsumeMentalStrength(float amount)
        {
            MentalStrength = Mathf.Max(0f, MentalStrength - amount);
            OnMentalStrengthChanged?.Invoke(MentalStrength);

            if (MentalStrength <= 0f)
            {
                OnMentalStrengthDepleted?.Invoke();
            }
        }

        public void ConsumeBrainCapacity(int amount)
        {
            BrainCapacity = Mathf.Max(0, BrainCapacity - amount);
            OnBrainCapacityChanged?.Invoke(BrainCapacity);
        }

        public void RestoreBrainCapacity(int amount)
        {
            BrainCapacity = Mathf.Min(MaxBrainCapacity, BrainCapacity + amount);
            OnBrainCapacityChanged?.Invoke(BrainCapacity);
        }

        public void Cleanup()
        {
            OnMentalStrengthChanged = null;
            OnBrainCapacityChanged = null;
            OnMentalStrengthDepleted = null;
        }
    }
} 