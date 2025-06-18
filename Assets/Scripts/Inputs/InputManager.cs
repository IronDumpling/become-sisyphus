using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Core;
using BecomeSisyphus.Inputs.Commands;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Inputs
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [SerializeField] private InputActionAsset inputActions;

        private InputActionMap currentActionMap;
        private Dictionary<string, ICommand> commandMap = new Dictionary<string, ICommand>();

        // Store move command for dynamic updates
        private MoveBoatCommand moveBoatCommand;
        private MoveBoatCommand sailingMoveBoatCommand;

        // Controllers
        private ThoughtBoatSailingController thoughtBoatSailingController;
        private ThoughtVesselController thoughtVesselController;
        private TelescopeController telescopeController;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeInputActions();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // New method: Find and set active controllers when needed
        public void SetActiveControllers()
        {
            // Find controllers in current scene
            thoughtBoatSailingController = FindAnyObjectByType<ThoughtBoatSailingController>();
            thoughtVesselController = FindAnyObjectByType<ThoughtVesselController>();
            telescopeController = FindAnyObjectByType<TelescopeController>();

            Debug.Log($"InputManager: SetActiveControllers - Found controllers: " +
                     $"ThoughtBoatSailing={thoughtBoatSailingController != null}, " +
                     $"ThoughtVessel={thoughtVesselController != null}, " +
                     $"Telescope={telescopeController != null}");
        }

        private void InitializeInputActions()
        {
            if (inputActions == null)
            {
                Debug.LogError("Input Action Asset not assigned to InputManager!");
                return;
            }

            Debug.Log($"InputManager: Initializing Input Actions. Found {inputActions.actionMaps.Count} action maps");

            foreach (var actionMap in inputActions.actionMaps)
            {
                Debug.Log($"InputManager: Found action map: {actionMap.name} with {actionMap.actions.Count} actions");
                foreach (var action in actionMap.actions)
                {
                    Debug.Log($"    Action: {action.name}");
                    foreach (var binding in action.bindings)
                    {
                        Debug.Log($"      Binding: {binding.path}");
                    }
                }
                actionMap.Enable();
                actionMap.Disable();
            }

            // Set default action map based on initial game state
            SwitchActionMap("OutsideWorld");
        }

        public void SwitchActionMap(string actionMapName)
        {
            Debug.Log($"InputManager: Attempting to switch to action map: {actionMapName}");
            
            // Update controller references when switching action maps
            SetActiveControllers();
            
            if (currentActionMap != null)
            {
                Debug.Log($"InputManager: Disabling current action map: {currentActionMap.name}");
                UnsubscribeFromActions();
                currentActionMap.Disable();
                UnregisterAllCommands();
            }

            currentActionMap = inputActions.FindActionMap(actionMapName);
            if (currentActionMap != null)
            {
                Debug.Log($"InputManager: Found action map: {actionMapName}, enabling...");
                currentActionMap.Enable();
                
                // Log all actions in this action map
                Debug.Log($"InputManager: Action map {actionMapName} has {currentActionMap.actions.Count} actions:");
                foreach (var action in currentActionMap.actions)
                {
                    Debug.Log($"  - Action: {action.name}, Enabled: {action.enabled}");
                }
                
                RegisterCommandsForActionMap(actionMapName);
                SubscribeToActions();
                Debug.Log($"InputManager: Successfully switched to action map: {actionMapName}");
            }
            else
            {
                Debug.LogError($"InputManager: Action map not found: {actionMapName}");
                Debug.LogError($"InputManager: Available action maps: {string.Join(", ", inputActions.actionMaps.Select(am => am.name))}");
            }
        }

        private void RegisterCommandsForActionMap(string actionMapName)
        {
            switch (actionMapName)
            {
                case "MainTitle":
                    // No commands needed for MainTitle as it auto-transitions
                    break;
                case "OutsideWorld":
                    RegisterOutsideWorldCommands();
                    break;
                case "InsideWorld":
                    RegisterInsideWorldCommands();
                    break;
                case "BoatSailing":
                    RegisterBoatSailingCommands();
                    break;
                case "BoatInteraction":
                    RegisterBoatInteractionCommands();
                    break;
                case "ThoughtVessel":
                    RegisterThoughtVesselCommands();
                    break;
                case "Telescope":
                    RegisterTelescopeCommands();
                    break;
            }
        }

        private void RegisterOutsideWorldCommands()
        {
            Debug.Log("InputManager: Registering OutsideWorld commands...");

            // State-specific commands based on current state
            RegisterCommand("StartClimbing", new StartClimbingCommand());
            RegisterCommand("EnterInsideWorld", new EnterInsideWorldCommand());

            Debug.Log("InputManager: Finished registering OutsideWorld commands");
        }

        private void RegisterInsideWorldCommands()
        {
            if (thoughtBoatSailingController == null) 
            {
                Debug.LogError("InputManager: thoughtBoatSailingController is null in RegisterInsideWorldCommands!");
                return;
            }

            Debug.Log("InputManager: Registering InsideWorld commands...");

            // State-specific commands
            RegisterCommand("StartSailing", new CloseInteractionCommand());
            RegisterCommand("EnterOutsideWorld", new EnterOutsideWorldFromInsideCommand());
            
            // Store move command for dynamic updates (using original MoveBoatCommand)
            moveBoatCommand = new MoveBoatCommand(thoughtBoatSailingController, Vector2.zero);
            RegisterCommand("MoveBoat", moveBoatCommand);

            Debug.Log("InputManager: Finished registering InsideWorld commands");
        }

        private void RegisterBoatSailingCommands()
        {
            if (thoughtBoatSailingController == null) 
            {
                Debug.LogError("InputManager: thoughtBoatSailingController is null in RegisterBoatSailingCommands!");
                return;
            }

            Debug.Log("InputManager: Registering BoatSailing commands...");

            // Store move command for dynamic updates (separate instance for sailing)
            sailingMoveBoatCommand = new MoveBoatCommand(thoughtBoatSailingController, Vector2.zero);
            RegisterCommand("MoveBoat", sailingMoveBoatCommand);
            RegisterCommand("EnterOutsideWorld", new EnterOutsideWorldFromInsideCommand());

            // Unified interaction command for nearby points
            RegisterCommand("InteractWithNearbyPoint", new InteractWithNearbyPointCommand(thoughtBoatSailingController));
            
            // Keep vessel and navigation commands as they're not location-based
            RegisterCommand("OpenVesselUI", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.Vessel));
            RegisterCommand("OpenNavigationMap", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.NavigationMap));
            RegisterCommand("OpenTelescope", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.Telescope));

            Debug.Log("InputManager: Finished registering BoatSailing commands");
        }

        private void RegisterBoatInteractionCommands()
        {

        }

        private void RegisterThoughtVesselCommands()
        {
            if (thoughtVesselController == null) return;

            RegisterCommand("SwitchMode", new SwitchVesselModeCommand(thoughtVesselController, ThoughtVesselController.VesselUIMode.Grid));
            RegisterCommand("SelectGrid", new SelectGridCommand(thoughtVesselController, Vector2Int.zero));
            RegisterCommand("SelectCargo", new SelectCargoCommand(thoughtVesselController));
            RegisterCommand("MoveCargo", new MoveCargoCommand(thoughtVesselController, Vector2Int.zero));
            RegisterCommand("RotateCargo", new RotateCargoCommand(thoughtVesselController));
            RegisterCommand("ExitCargoSelection", new ExitCargoSelectionCommand(thoughtVesselController));
            RegisterCommand("SelectConfusion", new SelectConfusionCommand(thoughtVesselController, 0));
            RegisterCommand("SelectAbility", new SelectAbilityCommand(thoughtVesselController, 0));
            RegisterCommand("MoveMap", new MoveMapCommand(thoughtVesselController, Vector2.zero));
            RegisterCommand("CloseVesselUI", new CloseVesselUICommand(thoughtVesselController));
        }

        private void RegisterTelescopeCommands()
        {
            if (telescopeController == null) return;

            RegisterCommand("SwitchMode", new SwitchTelescopeModeCommand(telescopeController, TelescopeController.TelescopeMode.View));
            RegisterCommand("CloseTelescope", new CloseTelescopeCommand(telescopeController));
        }

        public void RegisterCommand(string actionName, ICommand command)
        {
            if (commandMap.ContainsKey(actionName))
            {
                Debug.LogWarning($"Command already registered for action: {actionName}");
                return;
            }

            commandMap[actionName] = command;
            Debug.Log($"Registered command for action: {actionName}");
        }

        private void UnregisterAllCommands()
        {
            commandMap.Clear();
        }

        public void UnregisterCommand(string actionName)
        {
            if (commandMap.ContainsKey(actionName))
            {
                commandMap.Remove(actionName);
                Debug.Log($"Unregistered command for action: {actionName}");
            }
        }

        public void ExecuteCommand(string actionName)
        {
            Debug.Log($"InputManager: Attempting to execute command: {actionName}");
            
            if (commandMap.TryGetValue(actionName, out ICommand command))
            {
                Debug.Log($"InputManager: Found command for {actionName}, executing...");
                command.Execute();
                Debug.Log($"InputManager: Command {actionName} executed successfully");
            }
            else
            {
                Debug.LogWarning($"InputManager: No command registered for action: {actionName}");
                Debug.LogWarning($"InputManager: Available commands: {string.Join(", ", commandMap.Keys)}");
            }
        }

        private void SubscribeToActions()
        {
            if (currentActionMap == null) return;
            
            Debug.Log($"InputManager: Subscribing to actions in {currentActionMap.name}");
            
            foreach (var action in currentActionMap.actions)
            {
                Debug.Log($"InputManager: Subscribing to action: {action.name}");
                action.performed += OnActionPerformed;
                action.canceled += OnActionCanceled;
                action.started += OnActionStarted;
            }
        }
        
        private void UnsubscribeFromActions()
        {
            if (currentActionMap == null) return;
            
            Debug.Log($"InputManager: Unsubscribing from actions in {currentActionMap.name}");
            
            foreach (var action in currentActionMap.actions)
            {
                action.performed -= OnActionPerformed;
                action.canceled -= OnActionCanceled;
                action.started -= OnActionStarted;
            }
        }
        
        private void OnActionPerformed(InputAction.CallbackContext context)
        {
            Debug.Log($"InputManager: Action performed: {context.action.name} in ActionMap: {currentActionMap?.name}");
            
            // Handle Vector2 actions differently
            if (context.action.name == "MoveBoat")
            {
                Vector2 direction = context.ReadValue<Vector2>();
                
                // Update the appropriate command instance based on current action map
                if (currentActionMap?.name == "InsideWorld" && moveBoatCommand != null)
                {
                    moveBoatCommand.UpdateDirection(direction);
                    moveBoatCommand.Execute();
                }
                else if (currentActionMap?.name == "BoatSailing" && sailingMoveBoatCommand != null)
                {
                    sailingMoveBoatCommand.UpdateDirection(direction);
                    sailingMoveBoatCommand.Execute();
                }
                else
                {
                    Debug.LogWarning($"InputManager: No MoveBoat command available for action map: {currentActionMap?.name}");
                }
            }
            else
            {
                Debug.Log($"InputManager: Executing command for action: {context.action.name}");
                ExecuteCommand(context.action.name);
            }
        }
        
        private void OnActionCanceled(InputAction.CallbackContext context)
        {
            Debug.Log($"InputManager: Action canceled: {context.action.name}");
            
            // Handle MoveBoat action cancellation (when player releases WASD)
            if (context.action.name == "MoveBoat")
            {
                // Send zero direction to stop the boat
                if (currentActionMap?.name == "InsideWorld" && moveBoatCommand != null)
                {
                    moveBoatCommand.UpdateDirection(Vector2.zero);
                    moveBoatCommand.Execute();
                }
                else if (currentActionMap?.name == "BoatSailing" && sailingMoveBoatCommand != null)
                {
                    sailingMoveBoatCommand.UpdateDirection(Vector2.zero);
                    sailingMoveBoatCommand.Execute();
                }
                
                Debug.Log("InputManager: MoveBoat action canceled, stopping boat");
            }
        }
        
        private void OnActionStarted(InputAction.CallbackContext context)
        {
            Debug.Log($"InputManager: Action started: {context.action.name}");
            // Handle action started if needed
        }

        private void OnDestroy()
        {
            if (currentActionMap != null)
            {
                UnsubscribeFromActions();
                currentActionMap.Disable();
            }
            commandMap.Clear();
        }
    }
} 