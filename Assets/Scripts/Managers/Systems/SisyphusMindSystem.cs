using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Managers.Systems
{
    public class SisyphusMindSystem : ISystem
    {
        private float maxMentalStrength = 100f;
        private float mentalStrengthRegenRate = 1f;
        private float mentalStrengthRegenDelay = 5f;

        private float currentMentalStrength;
        private float lastActionTime;
        private Dictionary<string, MindAbility> abilities = new Dictionary<string, MindAbility>();
        private float inspiration;

        public event Action<float> OnMentalStrengthChanged;
        public event Action<MindAbility> OnAbilityLevelUp;
        public event Action<float> OnInspirationChanged;

        public float MentalStrength => currentMentalStrength;
        public float MaxMentalStrength => maxMentalStrength;
        public float Inspiration => inspiration;

        public SisyphusMindSystem(float maxMentalStrength, float mentalStrengthRegenRate, float mentalStrengthRegenDelay)
        {
            this.maxMentalStrength = maxMentalStrength;
            this.mentalStrengthRegenRate = mentalStrengthRegenRate;
            this.mentalStrengthRegenDelay = mentalStrengthRegenDelay;
        }

        public void Initialize()
        {
            currentMentalStrength = maxMentalStrength;
            lastActionTime = 0f;
            InitializeAbilities();
        }

        private void InitializeAbilities()
        {
            abilities["Perception"] = new MindAbility { id = "Perception", name = "Perception", description = "感知力", level = 1, experience = 0, experienceToNextLevel = 100, isUnlocked = true };
            abilities["Memory"] = new MindAbility { id = "Memory", name = "Memory", description = "记忆力", level = 1, experience = 0, experienceToNextLevel = 100, isUnlocked = true };
            abilities["Insight"] = new MindAbility { id = "Insight", name = "Insight", description = "洞察力", level = 1, experience = 0, experienceToNextLevel = 100, isUnlocked = true };
            foreach (var ability in abilities.Values)
            {
                ability.OnLevelUp += OnAbilityLevelUp;
            }
        }

        public void Update() { }

        public void Update(float deltaTime, float time)
        {
            if (time - lastActionTime > mentalStrengthRegenDelay)
            {
                RegenerateMentalStrength(deltaTime);
            }
        }

        public void ConsumeMentalStrength(float amount, float time)
        {
            currentMentalStrength = Math.Max(0, currentMentalStrength - amount);
            lastActionTime = time;
            OnMentalStrengthChanged?.Invoke(currentMentalStrength);
        }

        private void RegenerateMentalStrength(float deltaTime)
        {
            if (currentMentalStrength < maxMentalStrength)
            {
                currentMentalStrength = Math.Min(maxMentalStrength, currentMentalStrength + mentalStrengthRegenRate * deltaTime);
                OnMentalStrengthChanged?.Invoke(currentMentalStrength);
            }
        }

        public void AddAbilityExperience(string abilityId, float amount)
        {
            if (abilities.TryGetValue(abilityId, out MindAbility ability))
            {
                ability.AddExperience(amount);
            }
        }

        public void AddInspiration(float amount)
        {
            inspiration += amount;
            OnInspirationChanged?.Invoke(inspiration);
        }

        public void UseInspiration(string abilityId, float amount)
        {
            if (inspiration >= amount)
            {
                inspiration -= amount;
                AddAbilityExperience(abilityId, amount * 100);
                OnInspirationChanged?.Invoke(inspiration);
            }
        }

        public int GetAbilityLevel(string abilityId)
        {
            return abilities.TryGetValue(abilityId, out MindAbility ability) ? ability.level : 0;
        }

        public void Cleanup()
        {
            foreach (var ability in abilities.Values)
            {
                ability.OnLevelUp -= OnAbilityLevelUp;
            }
            abilities.Clear();
            OnMentalStrengthChanged = null;
            OnAbilityLevelUp = null;
            OnInspirationChanged = null;
        }
    }
} 