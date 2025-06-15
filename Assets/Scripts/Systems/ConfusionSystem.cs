using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Systems
{
    public class ConfusionSystem : MonoBehaviour, ISystem
    {
        [SerializeField] private float confusionGenerationInterval = 30f;
        [SerializeField] private float temporaryConfusionDuration = 60f;

        private List<Confusion> activeConfusions = new List<Confusion>();
        private Dictionary<string, Confusion> confusionTemplates = new Dictionary<string, Confusion>();
        private SisyphusMindSystem mindSystem;
        private ThoughtVesselSystem vesselSystem;
        private float lastConfusionGenerationTime;

        public event Action<Confusion> OnConfusionGenerated;
        public event Action<Confusion> OnConfusionSolved;
        public event Action<Confusion> OnConfusionExpired;

        public void Initialize()
        {
            mindSystem = GameManager.Instance.GetSystem<SisyphusMindSystem>();
            vesselSystem = GameManager.Instance.GetSystem<ThoughtVesselSystem>();
            lastConfusionGenerationTime = Time.time;
            LoadConfusionTemplates();
        }

        private void LoadConfusionTemplates()
        {
            // TODO: 从配置文件或ScriptableObject加载困惑模板
        }

        public void Update()
        {
            if (Time.time - lastConfusionGenerationTime >= confusionGenerationInterval)
            {
                TryGenerateConfusion();
                lastConfusionGenerationTime = Time.time;
            }

            UpdateTemporaryConfusions();
        }

        private void TryGenerateConfusion()
        {
            // TODO: 实现困惑生成逻辑
            // 基于游戏进度、当前状态等条件生成新的困惑
        }

        private void UpdateTemporaryConfusions()
        {
            for (int i = activeConfusions.Count - 1; i >= 0; i--)
            {
                var confusion = activeConfusions[i];
                if (confusion.type == ConfusionType.Temporary && 
                    Time.time - confusion.timeLimit >= temporaryConfusionDuration)
                {
                    ExpireConfusion(confusion);
                }
            }
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
                
                // 奖励精神力
                mindSystem.ConsumeMentalStrength(-50f); // 恢复50点精神力
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