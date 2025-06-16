using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs.Commands
{
    public class SwitchTelescopeModeCommand : ICommand
    {
        private readonly TelescopeUIController controller;
        private readonly TelescopeUIController.TelescopeMode mode;

        public SwitchTelescopeModeCommand(TelescopeUIController controller, TelescopeUIController.TelescopeMode mode)
        {
            this.controller = controller;
            this.mode = mode;
        }

        public void Execute()
        {
            controller.SwitchMode(mode);
        }
    }

    public class CloseTelescopeCommand : ICommand
    {
        private readonly TelescopeUIController controller;

        public CloseTelescopeCommand(TelescopeUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.CloseTelescopeUI();
        }
    }
} 