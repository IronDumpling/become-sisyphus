using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Systems
{
    public class SisyphusMindSystem : MonoBehaviour, ISystem
    {
        [SerializeField] private float maxMentalStrength = 100f;
        [SerializeField] private float mentalStrengthRegenRate = 1f;
        [SerializeField] private float mentalStrengthRegenDelay = 5f;

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

        public void Initialize()
        {
            currentMentalStrength = maxMentalStrength;
            lastActionTime = Time.time;
            InitializeAbilities();
        }

        private void InitializeAbilities()
        {
            // Example: initialize with some default abilities
            abilities["Perception"] = new MindAbility { id = "Perception", name = "Perception", description = "感知力", level = 1, experience = 0, experienceToNextLevel = 100, isUnlocked = true };
            abilities["Memory"] = new MindAbility { id = "Memory", name = "Memory", description = "记忆力", level = 1, experience = 0, experienceToNextLevel = 100, isUnlocked = true };
            abilities["Insight"] = new MindAbility { id = "Insight", name = "Insight", description = "洞察力", level = 1, experience = 0, experienceToNextLevel = 100, isUnlocked = true };
            foreach (var ability in abilities.Values)
            {
                ability.OnLevelUp += OnAbilityLevelUp;
            }
        }

        public void Update()
        {
            if (Time.time - lastActionTime > mentalStrengthRegenDelay)
            {
                RegenerateMentalStrength();
            }
        }

        public void ConsumeMentalStrength(float amount)
        {
            currentMentalStrength = Mathf.Max(0, currentMentalStrength - amount);
            lastActionTime = Time.time;
            OnMentalStrengthChanged?.Invoke(currentMentalStrength);
        }

        private void RegenerateMentalStrength()
        {
            if (currentMentalStrength < maxMentalStrength)
            {
                currentMentalStrength = Mathf.Min(maxMentalStrength, 
                    currentMentalStrength + mentalStrengthRegenRate * Time.deltaTime);
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
                AddAbilityExperience(abilityId, amount * 100); // 灵感转换为大量经验
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