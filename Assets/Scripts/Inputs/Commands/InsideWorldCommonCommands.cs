using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Inputs.Controllers;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Commands
{
    public class SwitchToOutsideWorldCommand : ICommand
    {
        public void Execute()
        {
            Debug.Log("SwitchToOutsideWorldCommand: Starting execution...");
            
            // 使用新的状态管理系统
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                Debug.Log("SwitchToOutsideWorldCommand: Switching to OutsideWorld/Climbing state...");
                stateManager.SwitchToState("InsideGame/OutsideWorld/Climbing");
                Debug.Log("SwitchToOutsideWorldCommand: Execution completed - Switched to Outside World (Climbing State)");
            }
            else
            {
                Debug.LogError("SwitchToOutsideWorldCommand: GameStateManager.Instance is null!");
            }
        }
    }

    public class UsePerceptionSkillCommand : ICommand
    {
        private readonly OutsideWorldController outsideController;

        public UsePerceptionSkillCommand(OutsideWorldController controller)
        {
            this.outsideController = controller;
        }

        public void Execute()
        {
            outsideController.UsePerceptionSkill();
            
            // Switch to outside world using new state system
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                stateManager.SwitchToState("InsideGame/OutsideWorld/Climbing");
            }
            else
            {
                Debug.LogError("GameStateManager not found! Cannot switch to outside world.");
            }
            
            Debug.Log("Using Perception Skill and switching to Outside World");
        }
    }

    /// <summary>
    /// 开始航行命令 (Resting -> Sailing)
    /// </summary>
    public class StartSailingCommand : ICommand
    {
        public void Execute()
        {
            Debug.Log("StartSailingCommand: Executing start sailing");
            
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                var currentState = stateManager.CurrentActiveState;
                if (currentState is HarbourInteractionState harbourState)
                {
                    harbourState.StartSailing();
                }
                else if (currentState is LighthouseInteractionState lighthouseState)
                {
                    // If we add StartSailing to lighthouse state as well
                    Debug.Log("StartSailingCommand: Starting sailing from lighthouse");
                    var insideWorldState = stateManager.CurrentRootState?.SubStates["InsideWorld"];
                    insideWorldState?.SwitchToSubState("Sailing");
                }
                else
                {
                    Debug.LogWarning($"StartSailingCommand: Cannot execute from current state: {currentState?.StateName}");
                }
            }
        }
    }

    /// <summary>
    /// 进入外部世界命令 (从任何InsideWorld状态)
    /// </summary>
    public class EnterOutsideWorldFromInsideCommand : ICommand
    {
        public void Execute()
        {
            Debug.Log("EnterOutsideWorldFromInsideCommand: Executing enter outside world");
            
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                var currentState = stateManager.CurrentActiveState;
                var statePath = currentState?.GetFullStatePath();
                
                if (statePath != null && statePath.Contains("InsideWorld"))
                {
                    if (currentState is HarbourInteractionState harbourState)
                    {
                        harbourState.EnterOutsideWorld();
                    }
                    else if (currentState is SailingState sailingState)
                    {
                        sailingState.EnterOutsideWorld();
                    }
                    else
                    {
                        // Fallback for other inside world states
                        stateManager.SwitchToLastOutsideWorldState();
                    }
                }
                else
                {
                    Debug.LogWarning($"EnterOutsideWorldFromInsideCommand: Cannot execute from current state: {currentState?.StateName}");
                }
            }
        }
    }
} 