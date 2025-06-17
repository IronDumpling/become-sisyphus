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
            GameManager.Instance.ChangeState(GameState.Climbing);
            GameManager.Instance.GetSystem<CameraSystem>().SwitchToOutsideWorld();
            Debug.Log("Switching to Outside World (Climbing State)");
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