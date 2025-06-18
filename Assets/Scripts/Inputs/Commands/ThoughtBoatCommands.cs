using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;
using BecomeSisyphus.Core.GameStateSystem;

namespace BecomeSisyphus.Inputs.Commands
{
    /// <summary>
    /// Command for non-location-based interactions (Vessel UI, Navigation Map, Telescope)
    /// Location-based interactions now use InteractWithNearbyPointCommand
    /// </summary>
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

    /// <summary>
    /// Unified command to close any current interaction and start sailing
    /// Handles harbour, lighthouse, salvage, and other interaction closures
    /// </summary>
    public class CloseInteractionCommand : ICommand
    {
        public void Execute()
        {
            Debug.Log("CloseInteractionCommand: Executing start sailing (close interaction and begin sailing)");
            
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                var currentState = stateManager.CurrentActiveState;
                var statePath = currentState?.GetFullStatePath();
                
                Debug.Log($"CloseInteractionCommand: Current state: {currentState?.StateName}, Path: {statePath}");
                
                // Handle different interaction states
                if (currentState is HarbourInteractionState harbourState)
                {
                    Debug.Log("CloseInteractionCommand: Closing harbour interaction and starting sailing");
                    harbourState.StartSailing();
                }
                else if (currentState is LighthouseInteractionState lighthouseState)
                {
                    Debug.Log("CloseInteractionCommand: Closing lighthouse interaction and starting sailing");
                    // Close lighthouse interaction and switch to sailing
                    var insideWorldState = stateManager.CurrentRootState?.SubStates["InsideWorld"];
                    insideWorldState?.SwitchToSubState("Sailing");
                }
                else if (currentState is SalvageInteractionState salvageState)
                {
                    Debug.Log("CloseInteractionCommand: Closing salvage interaction and starting sailing");
                    // Close salvage interaction and switch to sailing
                    var insideWorldState = stateManager.CurrentRootState?.SubStates["InsideWorld"];
                    insideWorldState?.SwitchToSubState("Sailing");
                }
                else if (statePath != null && statePath.Contains("InsideWorld"))
                {
                    Debug.Log("CloseInteractionCommand: Switching to sailing from inside world state");
                    // Generic fallback for any inside world state
                    var insideWorldState = stateManager.CurrentRootState?.SubStates["InsideWorld"];
                    insideWorldState?.SwitchToSubState("Sailing");
                }
                else
                {
                    Debug.LogWarning($"CloseInteractionCommand: Cannot execute from current state: {currentState?.StateName}");
                }
            }
            else
            {
                Debug.LogError("CloseInteractionCommand: GameStateManager.Instance is null!");
            }
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

    /// <summary>
    /// Command to interact with nearby interaction point
    /// </summary>
    public class InteractWithNearbyPointCommand : ICommand
    {
        private readonly ThoughtBoatSailingController controller;

        public InteractWithNearbyPointCommand(ThoughtBoatSailingController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            
            var stateManager = BecomeSisyphus.Core.GameStateSystem.GameStateManager.Instance;
            if (stateManager != null)
            {
                var currentState = stateManager.CurrentActiveState;
                var statePath = currentState?.GetFullStatePath();
                
                Debug.Log($"[InteractWithNearbyPointCommand] Current State: {currentState?.StateName}, Path: {statePath}");
                
                // Only allow interaction in sailing states
                if (statePath != null && statePath.Contains("Sailing"))
                {
                    controller.TryInteractWithNearbyPoint();
                }
                else
                {
                    Debug.LogWarning($"[InteractWithNearbyPointCommand] ❌ Cannot interact in current state: {currentState?.StateName}");
                }
            }
            else
            {
                Debug.LogError("[InteractWithNearbyPointCommand] ❌ GameStateManager.Instance is null!");
            }
        }
    }
} 