using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs.Commands
{
    // Outside World Commands
    public class SelectSignifierCommand : ICommand
    {
        private OutsideWorldController controller;
        private Vector2 direction;

        public SelectSignifierCommand(OutsideWorldController controller, Vector2 direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.SelectSignifier(direction);
        }
    }

    public class SwitchToInsideWorldCommand : ICommand
    {
        // We will need a reference to GameManager or a state manager to change GameState
        public void Execute()
        {
            GameManager.Instance.ChangeState(BecomeSisyphus.Core.GameState.ExploringMind);
            Debug.Log("Switching to Inside World (ExploringMind State)");
            // TODO: Also disable/enable relevant cameras/UI here or in a state manager
        }
    }
} 