using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Inputs.Controllers;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Commands
{
    public class SelectSignifierCommand : ICommand
    {
        private readonly OutsideWorldController controller;
        private readonly Vector2 position;

        public SelectSignifierCommand(OutsideWorldController controller, Vector2 position)
        {
            this.controller = controller;
            this.position = position;
        }

        public void Execute()
        {
            // TODO: 实现选择标识物逻辑
        }
    }

    public class SwitchToInsideWorldCommand : ICommand
    {
        public void Execute()
        {
            Debug.Log("SwitchToInsideWorldCommand: Starting execution...");
            
            // 使用新的状态管理系统
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                Debug.Log("SwitchToInsideWorldCommand: Switching to InsideWorld/Sailing state...");
                stateManager.SwitchToState("InsideGame/InsideWorld/Sailing");
                Debug.Log("SwitchToInsideWorldCommand: Execution completed - Switched to Inside World (Sailing State)");
            }
            else
            {
                Debug.LogError("SwitchToInsideWorldCommand: GameStateManager.Instance is null!");
            }
        }
    }

    /// <summary>
    /// 开始爬山命令 (MountainFoot -> Climbing)
    /// </summary>
    public class StartClimbingCommand : ICommand
    {
        public void Execute()
        {
            Debug.Log("StartClimbingCommand: Executing start climbing");
            
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                var currentState = stateManager.CurrentActiveState;
                if (currentState is MountainFootState mountainFootState)
                {
                    mountainFootState.StartClimbing();
                }
                else
                {
                    Debug.LogWarning($"StartClimbingCommand: Cannot execute from current state: {currentState?.StateName}");
                }
            }
        }
    }

    /// <summary>
    /// 进入内部世界命令 (Climbing -> InsideWorld)
    /// </summary>
    public class EnterInsideWorldCommand : ICommand
    {
        public void Execute()
        {
            Debug.Log("EnterInsideWorldCommand: Executing enter inside world");
            
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                var currentState = stateManager.CurrentActiveState;
                if (currentState is ClimbingState climbingState)
                {
                    climbingState.EnterInsideWorld();
                }
                else
                {
                    Debug.LogWarning($"EnterInsideWorldCommand: Cannot execute from current state: {currentState?.StateName}");
                }
            }
        }
    }
} 