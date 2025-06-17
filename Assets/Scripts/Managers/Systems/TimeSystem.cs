using UnityEngine;
using System;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Core.GameStateSystem;

namespace BecomeSisyphus.Managers.Systems
{
    public class TimeSystem : ISystem
    {
        [SerializeField] private float timeScale = 1f;
        [SerializeField] private float dayLength = 24f; // 游戏内一天的长度（分钟）

        private float gameTime; // 游戏内时间（分钟）
        private float realTime; // 现实时间（秒）
        private int currentDay;
        private Season currentSeason;

        public event Action<float> OnGameTimeChanged;
        public event Action<int> OnDayChanged;
        public event Action<Season> OnSeasonChanged;

        public float GameTime => gameTime;
        public int CurrentDay => currentDay;
        public Season CurrentSeason => currentSeason;

        public TimeSystem(float timeScale, float dayLength)
        {
            this.timeScale = timeScale;
            this.dayLength = dayLength;
        }

        public void Initialize()
        {
            gameTime = 0f;
            realTime = 0f;
            currentDay = 1;
            currentSeason = Season.Spring;
        }

        public void Update()
        {
            // Check if we're in states where time should progress
            var stateManager = GameStateManager.Instance;
            bool shouldProgressTime = false;
            
            if (stateManager != null)
            {
                var currentStatePath = stateManager.GetCurrentStatePath();
                // Time progresses during climbing, sailing, and mind exploration
                shouldProgressTime = currentStatePath.Contains("Climbing") ||
                                   currentStatePath.Contains("Sailing") ||
                                   currentStatePath.Contains("ExploringMind") ||
                                   currentStatePath.Contains("Harbour") ||  // Resting at harbour
                                   currentStatePath.Contains("Lighthouse") ||  // Resting at lighthouse
                                   currentStatePath.Contains("Salvage");  // Salvaging action
            }
            
            if (shouldProgressTime)
            {
                realTime += Time.deltaTime * timeScale;
                gameTime += Time.deltaTime * timeScale / 60f; // 转换为分钟

                // 检查是否需要更新日期
                if (gameTime >= dayLength)
                {
                    gameTime -= dayLength;
                    currentDay++;
                    OnDayChanged?.Invoke(currentDay);

                    // 检查是否需要更新季节
                    UpdateSeason();
                }

                OnGameTimeChanged?.Invoke(gameTime);
            }
        }

        private void UpdateSeason()
        {
            Season newSeason = (Season)((currentDay - 1) / 30 % 4); // 每30天一个季节
            if (newSeason != currentSeason)
            {
                currentSeason = newSeason;
                OnSeasonChanged?.Invoke(currentSeason);
            }
        }

        public void Cleanup()
        {
            OnGameTimeChanged = null;
            OnDayChanged = null;
            OnSeasonChanged = null;
        }
    }

    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }
} 