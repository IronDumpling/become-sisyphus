using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs.Commands
{
    public class OpenInteractionCommand : ICommand
    {
        private readonly InteractionController controller;
        private readonly InteractionController.InteractionType type;
        private readonly string interactionId;

        public OpenInteractionCommand(InteractionController controller, InteractionController.InteractionType type, string interactionId)
        {
            this.controller = controller;
            this.type = type;
            this.interactionId = interactionId;
        }

        public void Execute()
        {
            controller.OpenInteraction(type, interactionId);
        }
    }

    public class CloseInteractionCommand : ICommand
    {
        private readonly InteractionController controller;

        public CloseInteractionCommand(InteractionController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.CloseCurrentInteraction();
        }
    }
} 