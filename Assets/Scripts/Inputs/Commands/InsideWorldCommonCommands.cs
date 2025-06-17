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
} 