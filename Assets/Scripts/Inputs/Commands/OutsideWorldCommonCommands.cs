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
            GameManager.Instance.ChangeState(GameState.Sailing);
            GameManager.Instance.GetSystem<CameraSystem>().SwitchToInsideWorld();
            Debug.Log("Switching to Inside World (Sailing State)");
        }
    }
} 