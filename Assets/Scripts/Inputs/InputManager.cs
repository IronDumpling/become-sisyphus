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

        [Header("Controller Settings")]
        [SerializeField] private bool createOutsideWorldController = true;
        [SerializeField] private bool createThoughtBoatSailingController = true;
        [SerializeField] private bool createThoughtBoatInteractionController = true;
        [SerializeField] private bool createThoughtVesselController = true;
        [SerializeField] private bool createTelescopeController = true;

        private InputActionMap currentActionMap;
        private Dictionary<string, ICommand> commandMap = new Dictionary<string, ICommand>();

        // Controllers
        private OutsideWorldController outsideWorldController;
        private ThoughtBoatSailingController thoughtBoatSailingController;
        private ThoughtBoatInteractionController thoughtBoatInteractionController;
        private ThoughtVesselController thoughtVesselController;
        private TelescopeController telescopeController;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                CreateControllers();
                InitializeInputActions();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void CreateControllers()
        {
            if (createOutsideWorldController)
            {
                var controllerObj = new GameObject("OutsideWorldController");
                controllerObj.transform.SetParent(transform);
                outsideWorldController = controllerObj.AddComponent<OutsideWorldController>();
            }

            if (createThoughtBoatSailingController)
            {
                var controllerObj = new GameObject("ThoughtBoatSailingController");
                controllerObj.transform.SetParent(transform);
                thoughtBoatSailingController = controllerObj.AddComponent<ThoughtBoatSailingController>();
            }

            if (createThoughtBoatInteractionController)
            {
                var controllerObj = new GameObject("ThoughtBoatInteractionController");
                controllerObj.transform.SetParent(transform);
                thoughtBoatInteractionController = controllerObj.AddComponent<ThoughtBoatInteractionController>();
            }

            if (createThoughtVesselController)
            {
                var controllerObj = new GameObject("ThoughtVesselController");
                controllerObj.transform.SetParent(transform);
                thoughtVesselController = controllerObj.AddComponent<ThoughtVesselController>();
            }

            if (createTelescopeController)
            {
                var controllerObj = new GameObject("TelescopeController");
                controllerObj.transform.SetParent(transform);
                telescopeController = controllerObj.AddComponent<TelescopeController>();
            }
        }

        private void ValidateControllers()
        {
            if (outsideWorldController == null && createOutsideWorldController) Debug.LogWarning("OutsideWorldController not created!");
            if (thoughtBoatSailingController == null && createThoughtBoatSailingController) Debug.LogWarning("ThoughtBoatSailingController not created!");
            if (thoughtBoatInteractionController == null && createThoughtBoatInteractionController) Debug.LogWarning("ThoughtBoatInteractionController not created!");
            if (thoughtVesselController == null && createThoughtVesselController) Debug.LogWarning("ThoughtVesselController not created!");
            if (telescopeController == null && createTelescopeController) Debug.LogWarning("TelescopeController not created!");
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
                case "OutsideWorld":
                    RegisterOutsideWorldCommands();
                    break;
                case "InsideWorld":
                    RegisterInsideWorldCommands();
                    break;
                case "BoatSailing":
                    RegisterBoatSailingCommands();
                    RegisterInsideWorldCommands();
                    break;
                case "BoatInteraction":
                    RegisterBoatInteractionCommands();
                    RegisterInsideWorldCommands();
                    break;
                case "ThoughtVessel":
                    RegisterThoughtVesselCommands();
                    RegisterInsideWorldCommands();
                    break;
                case "Telescope":
                    RegisterTelescopeCommands();
                    RegisterInsideWorldCommands();
                    break;
            }
        }

        private void RegisterOutsideWorldCommands()
        {
            if (outsideWorldController == null) 
            {
                Debug.LogError("InputManager: outsideWorldController is null in RegisterOutsideWorldCommands!");
                return;
            }

            Debug.Log("InputManager: Registering OutsideWorld commands...");

            RegisterCommand("SelectSignifier", new SelectSignifierCommand(outsideWorldController, Vector2.zero));
            RegisterCommand("SwitchToInsideWorld", new SwitchToInsideWorldCommand());
            RegisterCommand("UsePerceptionSkill", new UsePerceptionSkillCommand(outsideWorldController));

            Debug.Log("InputManager: Finished registering OutsideWorld commands");
        }

        private void RegisterInsideWorldCommands()
        {
            if (thoughtBoatSailingController == null) return;

            RegisterCommand("SwitchToOutsideWorld", new SwitchToOutsideWorldCommand());
            RegisterCommand("UsePerceptionSkill", new UsePerceptionSkillCommand(outsideWorldController));
        }

        private void RegisterBoatSailingCommands()
        {
            if (thoughtBoatSailingController == null) 
            {
                Debug.LogError("InputManager: thoughtBoatSailingController is null in RegisterBoatSailingCommands!");
                return;
            }

            Debug.Log("InputManager: Registering BoatSailing commands...");

            RegisterCommand("MoveBoat", new MoveBoatCommand(thoughtBoatSailingController, Vector2.zero));
            RegisterCommand("StopBoat", new StopBoatCommand(thoughtBoatSailingController));

            // Use unified OpenInteractionCommand with specific interaction types
            RegisterCommand("OpenIslandInteraction", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.Island, ""));
            RegisterCommand("OpenSalvageInteraction", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.Salvage, ""));
            RegisterCommand("OpenLighthouseInteraction", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.Lighthouse, ""));
            RegisterCommand("OpenHarborInteraction", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.Harbor, ""));
            RegisterCommand("OpenVesselUI", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.Vessel));
            RegisterCommand("OpenNavigationMap", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.NavigationMap));
            RegisterCommand("OpenTelescope", new OpenInteractionCommand(thoughtBoatSailingController, ThoughtBoatSailingController.InteractionType.Telescope));
            // RegisterCommand("SwitchToOutsideWorld", new SwitchToOutsideWorldCommand());

            Debug.Log("InputManager: Finished registering BoatSailing commands");
        }

        private void RegisterBoatInteractionCommands()
        {
            if (thoughtBoatInteractionController == null) return;

            RegisterCommand("CloseInteraction", new CloseInteractionCommand(thoughtBoatInteractionController));
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
            Debug.Log($"InputManager: Action performed: {context.action.name}");
            ExecuteCommand(context.action.name);
        }
        
        private void OnActionCanceled(InputAction.CallbackContext context)
        {
            Debug.Log($"InputManager: Action canceled: {context.action.name}");
            // Handle action canceled if needed
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