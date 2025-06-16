using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs.Commands
{
    public class OpenIslandInteractionCommand : ICommand
    {
        private readonly SailingController controller;
        private readonly string islandId;

        public OpenIslandInteractionCommand(SailingController controller, string islandId)
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
        private readonly SailingController controller;
        private readonly string salvageId;

        public OpenSalvageInteractionCommand(SailingController controller, string salvageId)
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
        private readonly SailingController controller;

        public OpenNavigationMapCommand(SailingController controller)
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
        private readonly SailingController controller;

        public OpenTelescopeCommand(SailingController controller)
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
        private readonly SailingController controller;

        public OpenVesselUICommand(SailingController controller)
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