using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs.Commands
{
    public class OpenInteractionCommand : ICommand
    {
        private readonly ThoughtBoatSailingController controller;
        private readonly ThoughtBoatSailingController.InteractionType type;
        private readonly string interactionId;

        public OpenInteractionCommand(ThoughtBoatSailingController controller, ThoughtBoatSailingController.InteractionType type, string interactionId = null)
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
        private readonly ThoughtBoatInteractionController controller;

        public CloseInteractionCommand(ThoughtBoatInteractionController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.CloseCurrentInteraction();
        }
    }

    public class MoveBoatCommand : ICommand
    {
        private readonly ThoughtBoatSailingController controller;
        private readonly Vector2 direction;

        public MoveBoatCommand(ThoughtBoatSailingController controller, Vector2 direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.Move(direction);
        }
    }

    public class StopBoatCommand : ICommand
    {
        private readonly ThoughtBoatSailingController controller;

        public StopBoatCommand(ThoughtBoatSailingController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.Stop();
        }
    }
} 