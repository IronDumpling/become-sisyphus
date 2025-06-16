using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Inputs.Commands
{
    // Thought Vessel Interface Commands
    public class CloseVesselUICommand : ICommand
    {
        private VesselUIController controller;

        public CloseVesselUICommand(VesselUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.CloseVesselUI();
            // TODO: Determine the state to return to (e.g., InsideWorld/Sailing)
            Debug.Log("Thought Vessel UI: Closing");
        }
    }

    public class SelectVesselGridCommand : ICommand
    {
        private VesselUIController controller;
        private Vector2Int direction;

        public SelectVesselGridCommand(VesselUIController controller, Vector2Int direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.SelectGrid(direction);
            Debug.Log($"Thought Vessel UI: Selecting grid at {direction}");
        }
    }

    public class SelectCargoInVesselCommand : ICommand
    {
        private VesselUIController controller;
        // Potentially pass the selected cargo if known, or let controller determine

        public SelectCargoInVesselCommand(VesselUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            // Assuming controller handles finding the cargo at the selected grid
            // For now, let's just assume it tries to select *some* cargo.
            controller.SelectCargo(null); // TODO: Pass actual cargo
            Debug.Log("Thought Vessel UI: Selecting cargo");
        }
    }

    public class MoveCargoInVesselCommand : ICommand
    {
        private VesselUIController controller;
        private Vector2Int direction;

        public MoveCargoInVesselCommand(VesselUIController controller, Vector2Int direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.MoveCargo(direction);
            Debug.Log($"Thought Vessel UI: Moving cargo {direction}");
        }
    }

    public class RotateCargoInVesselCommand : ICommand
    {
        private VesselUIController controller;

        public RotateCargoInVesselCommand(VesselUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.RotateCargo();
            Debug.Log("Thought Vessel UI: Rotating cargo");
        }
    }

    public class ExitCargoSelectionCommand : ICommand
    {
        private VesselUIController controller;

        public ExitCargoSelectionCommand(VesselUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.ExitCargoSelection();
            Debug.Log("Thought Vessel UI: Exiting cargo selection");
        }
    }
} 