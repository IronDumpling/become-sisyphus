using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Interfaces;
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
            
            if (GameManager.Instance == null)
            {
                Debug.LogError("SwitchToInsideWorldCommand: GameManager.Instance is null!");
                return;
            }
            
            Debug.Log("SwitchToInsideWorldCommand: Changing game state to Sailing...");
            GameManager.Instance.ChangeState(GameState.Sailing);
            
            Debug.Log("SwitchToInsideWorldCommand: Getting CameraSystem...");
            var cameraSystem = GameManager.Instance.GetSystem<CameraSystem>();
            
            if (cameraSystem == null)
            {
                Debug.LogError("SwitchToInsideWorldCommand: CameraSystem is null!");
                return;
            }
            
            Debug.Log("SwitchToInsideWorldCommand: Calling SwitchToInsideWorld...");
            cameraSystem.SwitchToInsideWorld();
            
            // Switch to inside world input action map
            Debug.Log("SwitchToInsideWorldCommand: Switching input action map to InsideWorld...");
            if (BecomeSisyphus.Inputs.InputManager.Instance != null)
            {
                BecomeSisyphus.Inputs.InputManager.Instance.SwitchActionMap("InsideWorld");
            }
            else
            {
                Debug.LogError("SwitchToInsideWorldCommand: InputManager.Instance is null!");
            }
            
            Debug.Log("SwitchToInsideWorldCommand: Execution completed - Switched to Inside World (Sailing State)");
        }
    }
} 