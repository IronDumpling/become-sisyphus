using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
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
        [SerializeField] private bool createThoughtBoatController = true;
        [SerializeField] private bool createSailingController = true;
        [SerializeField] private bool createThoughtVesselController = true;
        [SerializeField] private bool createTelescopeController = true;
        [SerializeField] private bool createInteractionController = true;

        private InputActionMap currentActionMap;
        private Dictionary<string, ICommand> commandMap = new Dictionary<string, ICommand>();

        // Controllers
        private OutsideWorldController outsideWorldController;
        private ThoughtBoatController thoughtBoatController;
        private SailingController SailingController;
        private ThoughtVesselController ThoughtVesselController;
        private TelescopeController TelescopeController;
        private InteractionController InteractionController;

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

            if (createThoughtBoatController)
            {
                var controllerObj = new GameObject("ThoughtBoatController");
                controllerObj.transform.SetParent(transform);
                thoughtBoatController = controllerObj.AddComponent<ThoughtBoatController>();
            }

            if (createSailingController)
            {
                var controllerObj = new GameObject("SailingController");
                controllerObj.transform.SetParent(transform);
                SailingController = controllerObj.AddComponent<SailingController>();
            }

            if (createThoughtVesselController)
            {
                var controllerObj = new GameObject("ThoughtVesselController");
                controllerObj.transform.SetParent(transform);
                ThoughtVesselController = controllerObj.AddComponent<ThoughtVesselController>();
            }

            if (createTelescopeController)
            {
                var controllerObj = new GameObject("TelescopeController");
                controllerObj.transform.SetParent(transform);
                TelescopeController = controllerObj.AddComponent<TelescopeController>();
            }

            if (createInteractionController)
            {
                var controllerObj = new GameObject("InteractionController");
                controllerObj.transform.SetParent(transform);
                InteractionController = controllerObj.AddComponent<InteractionController>();
            }
        }

        private void ValidateControllers()
        {
            if (outsideWorldController == null && createOutsideWorldController) Debug.LogWarning("OutsideWorldController not created!");
            if (thoughtBoatController == null && createThoughtBoatController) Debug.LogWarning("ThoughtBoatController not created!");
            if (SailingController == null && createSailingController) Debug.LogWarning("SailingController not created!");
            if (ThoughtVesselController == null && createThoughtVesselController) Debug.LogWarning("ThoughtVesselController not created!");
            if (TelescopeController == null && createTelescopeController) Debug.LogWarning("TelescopeController not created!");
            if (InteractionController == null && createInteractionController) Debug.LogWarning("InteractionController not created!");
        }

        private void InitializeInputActions()
        {
            if (inputActions == null)
            {
                Debug.LogError("Input Action Asset not assigned to InputManager!");
                return;
            }

            foreach (var actionMap in inputActions.actionMaps)
            {
                actionMap.Enable();
                actionMap.Disable();
            }

            // Set default action map based on initial game state
            SwitchActionMap("OutsideWorld");
        }

        public void SwitchActionMap(string actionMapName)
        {
            if (currentActionMap != null)
            {
                currentActionMap.Disable();
                UnregisterAllCommands();
            }

            currentActionMap = inputActions.FindActionMap(actionMapName);
            if (currentActionMap != null)
            {
                currentActionMap.Enable();
                RegisterCommandsForActionMap(actionMapName);
                Debug.Log($"Switched to action map: {actionMapName}");
            }
            else
            {
                Debug.LogError($"Action map not found: {actionMapName}");
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
                case "Sailing":
                    RegisterSailingCommands();
                    break;
                case "Vessel":
                    RegisterVesselCommands();
                    break;
                case "Telescope":
                    RegisterTelescopeCommands();
                    break;
                case "Interaction":
                    RegisterInteractionCommands();
                    break;
            }
        }

        private void RegisterOutsideWorldCommands()
        {
            if (outsideWorldController == null) return;

            RegisterCommand("SelectSignifier", new SelectSignifierCommand(outsideWorldController, Vector2.zero));
            RegisterCommand("SwitchToInsideWorld", new SwitchToInsideWorldCommand());
            RegisterCommand("UsePerceptionSkill", new UsePerceptionSkillCommand(outsideWorldController));
        }

        private void RegisterInsideWorldCommands()
        {
            if (thoughtBoatController == null) return;

            RegisterCommand("MoveBoat", new MoveThoughtBoatCommand(thoughtBoatController, Vector2.zero));
            RegisterCommand("SwitchToOutsideWorld", new SwitchToOutsideWorldCommand());
            RegisterCommand("UsePerceptionSkill", new UsePerceptionSkillCommand(outsideWorldController));
            RegisterCommand("OpenVesselUI", new OpenVesselUICommand(SailingController));
        }

        private void RegisterSailingCommands()
        {
            if (SailingController == null) return;

            RegisterCommand("OpenIslandInteraction", new OpenIslandInteractionCommand(SailingController, ""));
            RegisterCommand("OpenSalvageInteraction", new OpenSalvageInteractionCommand(SailingController, ""));
            RegisterCommand("OpenVesselUI", new OpenVesselUICommand(SailingController));
            RegisterCommand("OpenNavigationMap", new OpenNavigationMapCommand(SailingController));
            RegisterCommand("OpenTelescope", new OpenTelescopeCommand(SailingController));
            RegisterCommand("SwitchToOutsideWorld", new SwitchToOutsideWorldCommand());
        }

        private void RegisterVesselCommands()
        {
            if (ThoughtVesselController == null) return;

            RegisterCommand("SwitchMode", new SwitchVesselModeCommand(ThoughtVesselController, ThoughtVesselController.VesselUIMode.Grid));
            RegisterCommand("SelectGrid", new SelectGridCommand(ThoughtVesselController, Vector2Int.zero));
            RegisterCommand("SelectCargo", new SelectCargoCommand(ThoughtVesselController));
            RegisterCommand("MoveCargo", new MoveCargoCommand(ThoughtVesselController, Vector2Int.zero));
            RegisterCommand("RotateCargo", new RotateCargoCommand(ThoughtVesselController));
            RegisterCommand("ExitCargoSelection", new ExitCargoSelectionCommand(ThoughtVesselController));
            RegisterCommand("SelectConfusion", new SelectConfusionCommand(ThoughtVesselController, 0));
            RegisterCommand("SelectAbility", new SelectAbilityCommand(ThoughtVesselController, 0));
            RegisterCommand("MoveMap", new MoveMapCommand(ThoughtVesselController, Vector2.zero));
            RegisterCommand("CloseVesselUI", new CloseVesselUICommand(ThoughtVesselController));
        }

        private void RegisterTelescopeCommands()
        {
            if (TelescopeController == null) return;

            RegisterCommand("SwitchMode", new SwitchTelescopeModeCommand(TelescopeController, TelescopeController.TelescopeMode.View));
            RegisterCommand("CloseTelescope", new CloseTelescopeCommand(TelescopeController));
        }

        private void RegisterInteractionCommands()
        {
            if (InteractionController == null) return;

            RegisterCommand("OpenInteraction", new OpenInteractionCommand(InteractionController, InteractionController.InteractionType.Island, ""));
            RegisterCommand("CloseInteraction", new CloseInteractionCommand(InteractionController));
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
            if (commandMap.TryGetValue(actionName, out ICommand command))
            {
                command.Execute();
            }
            else
            {
                Debug.LogWarning($"No command registered for action: {actionName}");
            }
        }

        private void OnDestroy()
        {
            if (currentActionMap != null)
            {
                currentActionMap.Disable();
            }
            commandMap.Clear();
        }
    }
} 