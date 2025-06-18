using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace BecomeSisyphus.Core.GameStateSystem
{
    /// <summary>
    /// 游戏状态的基础抽象类
    /// </summary>
    public abstract class BaseGameState : IGameState
    {
        public string StateName { get; protected set; }
        public IGameState ParentState { get; set; }
        public Dictionary<string, IGameState> SubStates { get; private set; } = new Dictionary<string, IGameState>();
        public IGameState CurrentSubState { get; private set; }
        
        protected bool isActive = false;
        protected bool isPaused = false;

        public BaseGameState(string stateName)
        {
            StateName = stateName;
        }

        public virtual void OnEnter(IGameState previousState)
        {
            isActive = true;
            isPaused = false;
            Debug.Log($"GameState: Entering {GetFullStatePath()}");
            
            // 通知GameStateManager状态变化
            GameStateManager.Instance?.OnStateChanged(this);
        }

        public virtual void OnExit(IGameState nextState)
        {
            // 如果有活跃的子状态，先退出子状态
            if (CurrentSubState != null)
            {
                CurrentSubState.OnExit(nextState);
                CurrentSubState = null;
            }
            
            isActive = false;
            isPaused = false;
            Debug.Log($"GameState: Exiting {GetFullStatePath()}");
        }

        public virtual void OnUpdate()
        {
            if (!isActive || isPaused) return;
            
            // 如果有活跃的子状态，更新子状态
            CurrentSubState?.OnUpdate();
        }

        public virtual void OnPause()
        {
            isPaused = true;
            Debug.Log($"GameState: Pausing {GetFullStatePath()}");
        }

        public virtual void OnResume()
        {
            isPaused = false;
            Debug.Log($"GameState: Resuming {GetFullStatePath()}");
        }

        public virtual void AddSubState(IGameState subState)
        {
            if (subState == null)
            {
                Debug.LogError($"Cannot add null substate to {StateName}");
                return;
            }
            
            subState.ParentState = this;
            SubStates[subState.StateName] = subState;
            Debug.Log($"GameState: Added substate {subState.StateName} to {StateName}");
        }

        public virtual void SwitchToSubState(string stateName)
        {
            if (!SubStates.ContainsKey(stateName))
            {
                Debug.LogError($"Substate {stateName} not found in {StateName}. Available substates: {string.Join(", ", SubStates.Keys)}");
                return;
            }

            if (!CanTransitionTo(stateName))
            {
                Debug.LogWarning($"Cannot transition from {CurrentSubState?.StateName ?? "null"} to {stateName} in {StateName}");
                return;
            }

            var previousSubState = CurrentSubState;
            var newSubState = SubStates[stateName];

            // 退出当前子状态
            if (CurrentSubState != null)
            {
                CurrentSubState.OnExit(newSubState);
            }
            else
            {
                // 如果没有子状态，暂停当前状态
                OnPause();
            }

            // 进入新的子状态
            CurrentSubState = newSubState;
            CurrentSubState.OnEnter(previousSubState);
        }

        public virtual bool CanTransitionTo(string stateName)
        {
            // 默认允许所有转换，子类可以重写此方法添加限制
            return SubStates.ContainsKey(stateName);
        }

        /// <summary>
        /// 退出当前子状态，返回父状态
        /// </summary>
        public virtual void ExitCurrentSubState()
        {
            if (CurrentSubState != null)
            {
                CurrentSubState.OnExit(this);
                CurrentSubState = null;
                OnResume();
            }
        }

        /// <summary>
        /// 获取完整的状态路径（用于调试）
        /// </summary>
        public string GetFullStatePath()
        {
            var path = StateName;
            var parent = ParentState;
            while (parent != null)
            {
                path = parent.StateName + "/" + path;
                parent = parent.ParentState;
            }
            return path;
        }

        /// <summary>
        /// 检查是否是指定状态或其子状态
        /// </summary>
        public bool IsInState(string stateName)
        {
            if (StateName == stateName) return true;
            return CurrentSubState?.IsInState(stateName) ?? false;
        }

        /// <summary>
        /// 获取当前活跃的叶子状态（最深层的活跃状态）
        /// </summary>
        public IGameState GetActiveLeafState()
        {
            return CurrentSubState?.GetActiveLeafState() ?? this;
        }
    }
} 