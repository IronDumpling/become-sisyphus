using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs.Commands
{
    public class SwitchTelescopeModeCommand : ICommand
    {
        private readonly TelescopeController controller;
        private readonly TelescopeController.TelescopeMode mode;

        public SwitchTelescopeModeCommand(TelescopeController controller, TelescopeController.TelescopeMode mode)
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
        private readonly TelescopeController controller;

        public CloseTelescopeCommand(TelescopeController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.CloseTelescopeUI();
        }
    }
} 