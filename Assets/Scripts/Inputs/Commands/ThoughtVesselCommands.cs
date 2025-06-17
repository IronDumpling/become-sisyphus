using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs.Commands
{
    public class SwitchVesselModeCommand : ICommand
    {
        private readonly ThoughtVesselController controller;
        private readonly ThoughtVesselController.VesselUIMode mode;

        public SwitchVesselModeCommand(ThoughtVesselController controller, ThoughtVesselController.VesselUIMode mode)
        {
            this.controller = controller;
            this.mode = mode;
        }

        public void Execute()
        {
            controller.SwitchMode(mode);
            Debug.Log($"Vessel UI: Switching to {mode} mode");
        }
    }

    public class SelectGridCommand : ICommand
    {
        private readonly ThoughtVesselController controller;
        private readonly Vector2Int direction;

        public SelectGridCommand(ThoughtVesselController controller, Vector2Int direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.SelectGrid(direction);
            Debug.Log($"Vessel UI: Selecting grid at {direction}");
        }
    }

    public class SelectCargoCommand : ICommand
    {
        private readonly ThoughtVesselController controller;

        public SelectCargoCommand(ThoughtVesselController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.SelectCargo();
            Debug.Log("Vessel UI: Selecting cargo");
        }
    }

    public class MoveCargoCommand : ICommand
    {
        private readonly ThoughtVesselController controller;
        private readonly Vector2Int direction;

        public MoveCargoCommand(ThoughtVesselController controller, Vector2Int direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.MoveCargo(direction);
            Debug.Log($"Vessel UI: Moving cargo {direction}");
        }
    }

    public class RotateCargoCommand : ICommand
    {
        private readonly ThoughtVesselController controller;

        public RotateCargoCommand(ThoughtVesselController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.RotateCargo();
            Debug.Log("Vessel UI: Rotating cargo");
        }
    }

    public class ExitCargoSelectionCommand : ICommand
    {
        private readonly ThoughtVesselController controller;

        public ExitCargoSelectionCommand(ThoughtVesselController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.ExitCargoSelection();
            Debug.Log("Vessel UI: Exiting cargo selection");
        }
    }

    public class SelectConfusionCommand : ICommand
    {
        private readonly ThoughtVesselController controller;
        private readonly int direction;

        public SelectConfusionCommand(ThoughtVesselController controller, int direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.SelectConfusion(direction);
            Debug.Log($"Vessel UI: Selecting confusion {direction}");
        }
    }

    public class SelectAbilityCommand : ICommand
    {
        private readonly ThoughtVesselController controller;
        private readonly int direction;

        public SelectAbilityCommand(ThoughtVesselController controller, int direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.SelectAbility(direction);
            Debug.Log($"Vessel UI: Selecting ability {direction}");
        }
    }

    public class MoveMapCommand : ICommand
    {
        private readonly ThoughtVesselController controller;
        private readonly Vector2 direction;

        public MoveMapCommand(ThoughtVesselController controller, Vector2 direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.MoveMap(direction);
            Debug.Log($"Vessel UI: Moving map {direction}");
        }
    }

    public class CloseVesselUICommand : ICommand
    {
        private readonly ThoughtVesselController controller;

        public CloseVesselUICommand(ThoughtVesselController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.CloseVesselUI();
            Debug.Log("Vessel UI: Closing");
        }
    }
} 