using UnityEngine;
using System.Collections.Generic;
using System;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Core.GameStateSystem
{
    /// <summary>
    /// 游戏状态管理器
    /// </summary>
    public class GameStateManager : ISystem
    {
        public static GameStateManager Instance { get; private set; }
        
        private Dictionary<string, IGameState> rootStates = new Dictionary<string, IGameState>();
        private IGameState currentRootState;
        
        public IGameState CurrentRootState => currentRootState;
        public IGameState CurrentActiveState => currentRootState?.GetActiveLeafState();
        
        // 事件
        public event Action<IGameState> OnStateEntered;
        public event Action<IGameState> OnStateExited;
        public event Action<IGameState, IGameState> OnStateTransition;

        public void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
                SetupGameStates();
                
                // 设置初始状态为MainMenu
                SwitchToRootState("MainMenu");
                
                Debug.Log("GameStateManager: Initialized successfully");
            }
            else
            {
                Debug.LogWarning("GameStateManager: Trying to initialize when instance already exists");
            }
        }

        public void Update()
        {
            currentRootState?.OnUpdate();
        }

        public void Cleanup()
        {
            currentRootState?.OnExit(null);
            rootStates.Clear();
            Instance = null;
        }

        /// <summary>
        /// 设置游戏状态架构
        /// </summary>
        private void SetupGameStates()
        {
            // 创建根状态
            var mainMenuState = new MainMenuState();
            var insideGameState = new InsideGameState();
            
            // 添加根状态
            AddRootState(mainMenuState);
            AddRootState(insideGameState);
            
            // 设置InsideGame的子状态
            var outsideWorldState = new OutsideWorldState();
            var insideWorldState = new InsideWorldState();
            
            insideGameState.AddSubState(outsideWorldState);
            insideGameState.AddSubState(insideWorldState);
            
            // 设置OutsideWorld的子状态
            outsideWorldState.AddSubState(new MountainFootState());
            outsideWorldState.AddSubState(new ClimbingState());
            outsideWorldState.AddSubState(new PerceptionState());
            outsideWorldState.AddSubState(new MountainTopState());
            
            // 设置InsideWorld的子状态
            insideWorldState.AddSubState(new SailingState());
            insideWorldState.AddSubState(new InteractionState());
            insideWorldState.AddSubState(new ThoughtBoatCabinState());
            insideWorldState.AddSubState(new TelescopeState());
            
            // 设置Interaction的子状态
            var interactionState = insideWorldState.SubStates["Interaction"];
            interactionState.AddSubState(new HarbourInteractionState());
            interactionState.AddSubState(new LighthouseInteractionState());
            interactionState.AddSubState(new SalvageInteractionState());
            interactionState.AddSubState(new IslandInteractionState());
            
            // 设置ThoughtBoatCabin的子状态
            var cabinState = insideWorldState.SubStates["ThoughtBoatCabin"];
            cabinState.AddSubState(new ThoughtVesselState());
            cabinState.AddSubState(new NavigationMapState());
            cabinState.AddSubState(new MindAbilityState());
            
            Debug.Log("GameStateManager: Game state hierarchy setup completed");
        }

        /// <summary>
        /// 添加根状态
        /// </summary>
        public void AddRootState(IGameState state)
        {
            rootStates[state.StateName] = state;
            Debug.Log($"GameStateManager: Added root state {state.StateName}");
        }

        /// <summary>
        /// 切换到根状态
        /// </summary>
        public void SwitchToRootState(string stateName)
        {
            if (!rootStates.ContainsKey(stateName))
            {
                Debug.LogError($"Root state {stateName} not found. Available states: {string.Join(", ", rootStates.Keys)}");
                return;
            }

            var previousState = currentRootState;
            var newState = rootStates[stateName];

            if (previousState == newState)
            {
                Debug.LogWarning($"Already in root state {stateName}");
                return;
            }

            // 退出当前状态
            if (currentRootState != null)
            {
                currentRootState.OnExit(newState);
                OnStateExited?.Invoke(currentRootState);
            }

            // 进入新状态
            currentRootState = newState;
            currentRootState.OnEnter(previousState);
            OnStateEntered?.Invoke(currentRootState);
            
            // 触发状态转换事件
            OnStateTransition?.Invoke(previousState, currentRootState);
            
            Debug.Log($"GameStateManager: Switched from {previousState?.StateName ?? "null"} to {stateName}");
        }

        /// <summary>
        /// 切换到指定的完整状态路径
        /// </summary>
        public void SwitchToState(string statePath)
        {
            var pathParts = statePath.Split('/');
            if (pathParts.Length == 0)
            {
                Debug.LogError("Invalid state path: " + statePath);
                return;
            }

            // 切换到根状态
            var rootStateName = pathParts[0];
            if (currentRootState?.StateName != rootStateName)
            {
                SwitchToRootState(rootStateName);
            }

            // 逐级切换到子状态
            var currentState = currentRootState;
            for (int i = 1; i < pathParts.Length; i++)
            {
                var subStateName = pathParts[i];
                currentState.SwitchToSubState(subStateName);
                currentState = currentState.CurrentSubState;
            }
        }

        /// <summary>
        /// 获取当前状态路径
        /// </summary>
        public string GetCurrentStatePath()
        {
            return CurrentActiveState?.GetFullStatePath() ?? "None";
        }

        /// <summary>
        /// 检查是否在指定状态中
        /// </summary>
        public bool IsInState(string stateName)
        {
            return currentRootState?.IsInState(stateName) ?? false;
        }

        /// <summary>
        /// 状态变化时的回调（由BaseGameState调用）
        /// </summary>
        public void OnStateChanged(IGameState newState)
        {
            Debug.Log($"GameStateManager: State changed to {newState.GetFullStatePath()}");
            
            // 这里可以添加全局状态变化处理逻辑
            // 比如通知InputManager、CameraSystem等
            NotifySystemsOfStateChange(newState);
        }

        /// <summary>
        /// 通知其他系统状态变化
        /// </summary>
        private void NotifySystemsOfStateChange(IGameState newState)
        {
            // 通知InputManager切换Action Map
            if (BecomeSisyphus.Inputs.InputManager.Instance != null)
            {
                var actionMapName = DetermineActionMapForState(newState);
                if (!string.IsNullOrEmpty(actionMapName))
                {
                    BecomeSisyphus.Inputs.InputManager.Instance.SwitchActionMap(actionMapName);
                }
            }

            // 通知CameraSystem切换相机
            if (GameManager.Instance != null)
            {
                var cameraSystem = GameManager.Instance.GetSystem<BecomeSisyphus.Managers.Systems.CameraSystem>();
                if (cameraSystem != null)
                {
                    HandleCameraTransition(newState, cameraSystem);
                }
            }
        }

        /// <summary>
        /// 根据状态确定对应的Action Map
        /// </summary>
        private string DetermineActionMapForState(IGameState state)
        {
            var statePath = state.GetFullStatePath();
            
            if (statePath.Contains("MainMenu"))
                return "MainTitle";
            else if (statePath.Contains("OutsideWorld"))
                return "OutsideWorld";
            else if (statePath.Contains("Sailing"))
                return "InsideWorld";
            else if (statePath.Contains("Interaction"))
                return "BoatInteraction";
            else if (statePath.Contains("ThoughtVessel"))
                return "ThoughtVessel";
            else if (statePath.Contains("Telescope"))
                return "Telescope";
            else if (statePath.Contains("InsideWorld"))
                return "InsideWorld";
                
            return null;
        }

        /// <summary>
        /// 处理相机切换
        /// </summary>
        private void HandleCameraTransition(IGameState state, BecomeSisyphus.Managers.Systems.CameraSystem cameraSystem)
        {
            var statePath = state.GetFullStatePath();
            
            if (statePath.Contains("OutsideWorld"))
            {
                cameraSystem.SwitchToOutsideWorld();
            }
            else if (statePath.Contains("InsideWorld"))
            {
                cameraSystem.SwitchToInsideWorld();
            }
        }
    }
} 