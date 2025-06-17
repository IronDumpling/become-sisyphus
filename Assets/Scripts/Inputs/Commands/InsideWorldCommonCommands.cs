using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Commands
{
    public class SwitchToOutsideWorldCommand : ICommand
    {
        public void Execute()
        {
            Debug.Log("SwitchToOutsideWorldCommand: Starting execution...");
            
            if (GameManager.Instance == null)
            {
                Debug.LogError("SwitchToOutsideWorldCommand: GameManager.Instance is null!");
                return;
            }
            
            Debug.Log("SwitchToOutsideWorldCommand: Changing game state to Climbing...");
            GameManager.Instance.ChangeState(GameState.Climbing);
            
            Debug.Log("SwitchToOutsideWorldCommand: Getting CameraSystem...");
            var cameraSystem = GameManager.Instance.GetSystem<CameraSystem>();
            
            if (cameraSystem == null)
            {
                Debug.LogError("SwitchToOutsideWorldCommand: CameraSystem is null!");
                return;
            }
            
            Debug.Log("SwitchToOutsideWorldCommand: Calling SwitchToOutsideWorld...");
            cameraSystem.SwitchToOutsideWorld();
            
            // Switch back to outside world input action map
            Debug.Log("SwitchToOutsideWorldCommand: Switching input action map to OutsideWorld...");
            if (BecomeSisyphus.Inputs.InputManager.Instance != null)
            {
                BecomeSisyphus.Inputs.InputManager.Instance.SwitchActionMap("OutsideWorld");
            }
            else
            {
                Debug.LogError("SwitchToOutsideWorldCommand: InputManager.Instance is null!");
            }
            
            Debug.Log("SwitchToOutsideWorldCommand: Execution completed - Switched to Outside World (Climbing State)");
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
            GameManager.Instance.ChangeState(GameState.Climbing); // Auto switch to outside world
            Debug.Log("Using Perception Skill and switching to Outside World");
        }
    }
} 