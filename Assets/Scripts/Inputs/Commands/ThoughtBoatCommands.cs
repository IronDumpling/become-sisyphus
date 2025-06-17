using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;
using BecomeSisyphus.Core.GameStateSystem;

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
        private Vector2 direction;

        public MoveBoatCommand(ThoughtBoatSailingController controller, Vector2 direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            // Validate current state before executing
            var stateManager = BecomeSisyphus.Core.GameStateSystem.GameStateManager.Instance;
            if (stateManager != null)
            {
                var currentState = stateManager.CurrentActiveState;
                var statePath = currentState?.GetFullStatePath();
                
                // Only allow boat movement in sailing states
                if (statePath != null && statePath.Contains("Sailing"))
                {
                    controller.Move(direction);
                    
                    // Also notify the sailing state
                    if (currentState is SailingState sailingState)
                    {
                        sailingState.MoveBoat(direction);
                    }
                }
                else
                {
                    Debug.LogWarning($"MoveBoatCommand: Cannot move boat in current state: {currentState?.StateName}");
                }
            }
            else
            {
                // Fallback if no state manager
                controller.Move(direction);
            }
        }

        /// <summary>
        /// Update the movement direction for dynamic input
        /// </summary>
        public void UpdateDirection(Vector2 newDirection)
        {
            direction = newDirection;
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