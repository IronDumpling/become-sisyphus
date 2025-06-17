using System;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Managers.Systems
{
    public class SisyphusManager : ISystem
    {
        public float MentalStrength { get; private set; }
        public int BrainCapacity { get; private set; }
        public int MaxBrainCapacity { get; private set; }
        public float MentalStrengthRegenRate { get; private set; }
        
        public event Action<float> OnMentalStrengthChanged;
        public event Action<int> OnBrainCapacityChanged;
        public event Action OnMentalStrengthDepleted;

        public SisyphusManager(float mentalStrength, int maxBrainCapacity, float mentalStrengthRegenRate)
        {
            MentalStrength = mentalStrength;
            MaxBrainCapacity = maxBrainCapacity;
            BrainCapacity = maxBrainCapacity;
            MentalStrengthRegenRate = mentalStrengthRegenRate;
        }

        public void Initialize() { /* Initial values are now set by the constructor */ }

        public void Update() { /* Parameterless Update for ISystem */ }

        public void Update(float deltaTime)
        {
            // 精神力自然恢复
            if (MentalStrength < 100f)
            {
                MentalStrength = Math.Min(100f, MentalStrength + MentalStrengthRegenRate * deltaTime);
                OnMentalStrengthChanged?.Invoke(MentalStrength);
            }
        }

        public void ConsumeMentalStrength(float amount)
        {
            MentalStrength = Math.Max(0f, MentalStrength - amount);
            OnMentalStrengthChanged?.Invoke(MentalStrength);

            if (MentalStrength <= 0f)
            {
                OnMentalStrengthDepleted?.Invoke();
            }
        }

        public void ConsumeBrainCapacity(int amount)
        {
            BrainCapacity = Math.Max(0, BrainCapacity - amount);
            OnBrainCapacityChanged?.Invoke(BrainCapacity);
        }

        public void RestoreBrainCapacity(int amount)
        {
            BrainCapacity = Math.Min(MaxBrainCapacity, BrainCapacity + amount);
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