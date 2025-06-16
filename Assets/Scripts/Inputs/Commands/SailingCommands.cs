using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs.Commands
{
    public class OpenIslandInteractionCommand : ICommand
    {
        private readonly SailingUIController controller;
        private readonly string islandId;

        public OpenIslandInteractionCommand(SailingUIController controller, string islandId)
        {
            this.controller = controller;
            this.islandId = islandId;
        }

        public void Execute()
        {
            controller.OpenIslandInteraction(islandId);
        }
    }

    public class OpenSalvageInteractionCommand : ICommand
    {
        private readonly SailingUIController controller;
        private readonly string salvageId;

        public OpenSalvageInteractionCommand(SailingUIController controller, string salvageId)
        {
            this.controller = controller;
            this.salvageId = salvageId;
        }

        public void Execute()
        {
            controller.OpenSalvageInteraction(salvageId);
        }
    }

    public class OpenNavigationMapCommand : ICommand
    {
        private readonly SailingUIController controller;

        public OpenNavigationMapCommand(SailingUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.OpenNavigationMap();
        }
    }

    public class OpenTelescopeCommand : ICommand
    {
        private readonly SailingUIController controller;

        public OpenTelescopeCommand(SailingUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.OpenTelescope();
        }
    }

    public class OpenVesselUICommand : ICommand
    {
        private readonly SailingUIController controller;

        public OpenVesselUICommand(SailingUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.OpenVesselUI();
            Debug.Log("Opening Vessel UI");
        }
    }
} 