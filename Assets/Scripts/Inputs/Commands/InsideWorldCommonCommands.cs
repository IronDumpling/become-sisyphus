using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Inputs.Controllers;
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
} 