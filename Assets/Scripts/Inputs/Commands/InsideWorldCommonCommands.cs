using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Commands
{
    /// <summary>
    /// Unified command to enter outside world from any inside world state
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
                
                Debug.Log($"EnterOutsideWorldFromInsideCommand: Current state: {currentState?.StateName}, Path: {statePath}");
                
                if (statePath != null && statePath.Contains("InsideWorld"))
                {
                    if (currentState is HarbourInteractionState harbourState)
                    {
                        Debug.Log("EnterOutsideWorldFromInsideCommand: Exiting from harbour interaction");
                        harbourState.EnterOutsideWorld();
                    }
                    else if (currentState is SailingState sailingState)
                    {
                        Debug.Log("EnterOutsideWorldFromInsideCommand: Exiting from sailing state");
                        sailingState.EnterOutsideWorld();
                    }
                    else if (currentState is LighthouseInteractionState lighthouseState)
                    {
                        Debug.Log("EnterOutsideWorldFromInsideCommand: Exiting from lighthouse interaction");
                        stateManager.SwitchToLastOutsideWorldState();
                    }
                    else if (currentState is SalvageInteractionState salvageState)
                    {
                        Debug.Log("EnterOutsideWorldFromInsideCommand: Exiting from salvage interaction");
                        stateManager.SwitchToLastOutsideWorldState();
                    }
                    else
                    {
                        Debug.Log("EnterOutsideWorldFromInsideCommand: Using fallback to last outside world state");
                        // Fallback for other inside world states
                        stateManager.SwitchToLastOutsideWorldState();
                    }
                }
                else
                {
                    Debug.LogWarning($"EnterOutsideWorldFromInsideCommand: Cannot execute from current state: {currentState?.StateName}");
                }
            }
            else
            {
                Debug.LogError("EnterOutsideWorldFromInsideCommand: GameStateManager.Instance is null!");
            }
        }
    }
    
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
} 