using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Systems
{
    public class ConfusionSystem : ISystem
    {
        private float confusionGenerationInterval = 30f;
        private float temporaryConfusionDuration = 60f;

        private List<Confusion> activeConfusions = new List<Confusion>();
        private Dictionary<string, Confusion> confusionTemplates = new Dictionary<string, Confusion>();
        private SisyphusMindSystem mindSystem;
        private ThoughtVesselSystem vesselSystem;
        private float lastConfusionGenerationTime;

        public event Action<Confusion> OnConfusionGenerated;
        public event Action<Confusion> OnConfusionSolved;
        public event Action<Confusion> OnConfusionExpired;

        public ConfusionSystem(float confusionGenerationInterval, float temporaryConfusionDuration)
        {
            this.confusionGenerationInterval = confusionGenerationInterval;
            this.temporaryConfusionDuration = temporaryConfusionDuration;
        }

        public void Initialize()
        {
            lastConfusionGenerationTime = 0f;
            // mindSystem and vesselSystem should be set externally or via setters
            LoadConfusionTemplates();
        }

        private void LoadConfusionTemplates()
        {
            // TODO: 从配置文件或ScriptableObject加载困惑模板
        }

        public void Update() { }

        public void Update(float deltaTime, float time)
        {
            if (time - lastConfusionGenerationTime >= confusionGenerationInterval)
            {
                TryGenerateConfusion();
                lastConfusionGenerationTime = time;
            }

            UpdateTemporaryConfusions(time);
        }

        private void TryGenerateConfusion()
        {
            // TODO: 实现困惑生成逻辑
        }

        private void UpdateTemporaryConfusions(float time)
        {
            for (int i = activeConfusions.Count - 1; i >= 0; i--)
            {
                var confusion = activeConfusions[i];
                if (confusion.type == ConfusionType.Temporary && 
                    time - confusion.timeLimit >= temporaryConfusionDuration)
                {
                    ExpireConfusion(confusion);
                }
            }
        }

        public void SetMindSystem(SisyphusMindSystem system)
        {
            mindSystem = system;
        }
        public void SetVesselSystem(ThoughtVesselSystem system)
        {
            vesselSystem = system;
        }

        public void GenerateConfusion(Confusion confusion)
        {
            activeConfusions.Add(confusion);
            OnConfusionGenerated?.Invoke(confusion);
        }

        public void SolveConfusion(Confusion confusion)
        {
            if (activeConfusions.Contains(confusion))
            {
                confusion.Solve();
                activeConfusions.Remove(confusion);
                OnConfusionSolved?.Invoke(confusion);
                if (mindSystem != null)
                {
                    mindSystem.ConsumeMentalStrength(-50f, 0f);
                }
            }
        }

        private void ExpireConfusion(Confusion confusion)
        {
            if (activeConfusions.Contains(confusion))
            {
                activeConfusions.Remove(confusion);
                OnConfusionExpired?.Invoke(confusion);
            }
        }

        public List<Confusion> GetActiveConfusions()
        {
            return new List<Confusion>(activeConfusions);
        }

        public void Cleanup()
        {
            activeConfusions.Clear();
            confusionTemplates.Clear();
            OnConfusionGenerated = null;
            OnConfusionSolved = null;
            OnConfusionExpired = null;
        }
    }
} 