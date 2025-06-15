using UnityEngine;
using System;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class MindAbility
    {
        public string id;
        public string name;
        public string description;
        public int level;
        public float experience;
        public float experienceToNextLevel;
        public bool isUnlocked;

        public event Action<MindAbility> OnLevelUp;
        public event Action<MindAbility> OnUnlocked;

        public void AddExperience(float amount)
        {
            if (!isUnlocked) return;

            experience += amount;
            while (experience >= experienceToNextLevel)
            {
                LevelUp();
            }
        }

        public void Unlock()
        {
            if (!isUnlocked)
            {
                isUnlocked = true;
                OnUnlocked?.Invoke(this);
            }
        }

        private void LevelUp()
        {
            level++;
            experience -= experienceToNextLevel;
            experienceToNextLevel *= 1.5f; // 每级所需经验值增加50%
            OnLevelUp?.Invoke(this);
        }
    }
}

public enum MindAbilityType
{
    Perception,  // 感知
    Memory,      // 回忆
    Tenacity,    // 顽强
    Rationality, // 理智
    Dimension    // 维度
} 