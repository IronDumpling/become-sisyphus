using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Commands
{
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