using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Inputs.Commands
{
    // Inside World Commands (General & Sailing Interface)
    public class MoveThoughtBoatCommand : ICommand
    {
        private ThoughtBoatController controller;
        private Vector2 direction;

        public MoveThoughtBoatCommand(ThoughtBoatController controller, Vector2 direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.Move(direction);
        }
    }

    public class SwitchToOutsideWorldCommand : ICommand
    {
        public void Execute()
        {
            GameManager.Instance.ChangeState(BecomeSisyphus.Core.GameState.Climbing);
            Debug.Log("Switching to Outside World (Climbing State)");
            // TODO: Also disable/enable relevant cameras/UI here or in a state manager
        }
    }

    public class OpenLogbookMapCommand : ICommand
    {
        private LogbookUIController controller;

        public OpenLogbookMapCommand(LogbookUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.OpenLogbook("Map"); // Open directly to map page
            Debug.Log("Opening Logbook to Map Page");
        }
    }

    public class UsePerceptionSkillCommand : ICommand
    {
        private OutsideWorldController outsideController;

        public UsePerceptionSkillCommand(OutsideWorldController controller)
        {
            this.outsideController = controller;
        }

        public void Execute()
        {
            outsideController.UsePerceptionSkill();
            GameManager.Instance.ChangeState(BecomeSisyphus.Core.GameState.Climbing); // Auto switch to outside world
            Debug.Log("Using Perception Skill and switching to Outside World");
        }
    }

    public class OpenVesselUICommand : ICommand
    {
        private VesselUIController controller;

        public OpenVesselUICommand(VesselUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.OpenVesselUI();
            Debug.Log("Opening Thought Vessel UI");
        }
    }

    public class OpenLogbookUICommand : ICommand
    {
        private LogbookUIController controller;

        public OpenLogbookUICommand(LogbookUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.OpenLogbook(); // Open to last closed page
            Debug.Log("Opening Logbook UI");
        }
    }
} 