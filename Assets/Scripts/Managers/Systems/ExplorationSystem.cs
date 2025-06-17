using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Managers.Systems
{
    public class ExplorationSystem : ISystem
    {
        private SisyphusManager sisyphusManager;
        private Vector3 currentPosition;
        // private float explorationSpeed = 5f;
        private float mentalStrengthConsumptionRate = 2f;

        public event Action<Vector3> OnPositionChanged;
        public event Action<Clue> OnClueDiscovered;
        public event Action OnExplorationFailed;

        public void Initialize()
        {
            sisyphusManager = GameManager.Instance.GetSystem<SisyphusManager>();
            currentPosition = Vector3.zero;
        }

        public void Update()
        {
            if (GameManager.Instance.CurrentState == GameState.ExploringMind)
            {
                HandleExplorationInput();
                ConsumeMentalStrength();
            }
        }

        private void HandleExplorationInput()
        {
            // TODO: 实现探索输入处理
            // 处理玩家输入，移动思维小船
            // 检查是否发现新的线索
        }

        private void ConsumeMentalStrength()
        {
            sisyphusManager.ConsumeMentalStrength(mentalStrengthConsumptionRate * Time.deltaTime);
        }

        public void MoveToPosition(Vector3 newPosition)
        {
            if (sisyphusManager.MentalStrength > 0)
            {
                currentPosition = newPosition;
                OnPositionChanged?.Invoke(currentPosition);
            }
            else
            {
                OnExplorationFailed?.Invoke();
            }
        }

        public void DiscoverClue(Clue clue)
        {
            OnClueDiscovered?.Invoke(clue);
        }

        public void Cleanup()
        {
            OnPositionChanged = null;
            OnClueDiscovered = null;
            OnExplorationFailed = null;
        }
    }
} 