using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs.Commands
{
    public class MoveThoughtBoatCommand : ICommand
    {
        private readonly ThoughtBoatController controller;
        private readonly Vector2 direction;

        public MoveThoughtBoatCommand(ThoughtBoatController controller, Vector2 direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            // TODO: 实现思维船移动逻辑
        }
    }

    public class SwitchToOutsideWorldCommand : ICommand
    {
        public void Execute()
        {
            GameManager.Instance.ChangeState(GameState.Climbing);
            Debug.Log("Switching to Outside World (Climbing State)");
            // TODO: Also disable/enable relevant cameras/UI here or in a state manager
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