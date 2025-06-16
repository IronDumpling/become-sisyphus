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
        [SerializeField] private bool createSailingUIController = true;
        [SerializeField] private bool createVesselUIController = true;
        [SerializeField] private bool createTelescopeUIController = true;

        private InputActionMap currentActionMap;
        private Dictionary<string, ICommand> commandMap = new Dictionary<string, ICommand>();

        // Controllers
        private OutsideWorldController outsideWorldController;
        private ThoughtBoatController thoughtBoatController;
        private SailingUIController sailingUIController;
        private VesselUIController vesselUIController;
        private TelescopeUIController telescopeUIController;

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

            if (createSailingUIController)
            {
                var controllerObj = new GameObject("SailingUIController");
                controllerObj.transform.SetParent(transform);
                sailingUIController = controllerObj.AddComponent<SailingUIController>();
            }

            if (createVesselUIController)
            {
                var controllerObj = new GameObject("VesselUIController");
                controllerObj.transform.SetParent(transform);
                vesselUIController = controllerObj.AddComponent<VesselUIController>();
            }

            if (createTelescopeUIController)
            {
                var controllerObj = new GameObject("TelescopeUIController");
                controllerObj.transform.SetParent(transform);
                telescopeUIController = controllerObj.AddComponent<TelescopeUIController>();
            }
        }

        private void ValidateControllers()
        {
            if (outsideWorldController == null && createOutsideWorldController) Debug.LogWarning("OutsideWorldController not created!");
            if (thoughtBoatController == null && createThoughtBoatController) Debug.LogWarning("ThoughtBoatController not created!");
            if (sailingUIController == null && createSailingUIController) Debug.LogWarning("SailingUIController not created!");
            if (vesselUIController == null && createVesselUIController) Debug.LogWarning("VesselUIController not created!");
            if (telescopeUIController == null && createTelescopeUIController) Debug.LogWarning("TelescopeUIController not created!");
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
            RegisterCommand("OpenVesselUI", new OpenVesselUICommand(sailingUIController));
        }

        private void RegisterSailingCommands()
        {
            if (sailingUIController == null) return;

            RegisterCommand("OpenIslandInteraction", new OpenIslandInteractionCommand(sailingUIController, ""));
            RegisterCommand("OpenSalvageInteraction", new OpenSalvageInteractionCommand(sailingUIController, ""));
            RegisterCommand("OpenVesselUI", new OpenVesselUICommand(sailingUIController));
            RegisterCommand("OpenNavigationMap", new OpenNavigationMapCommand(sailingUIController));
            RegisterCommand("OpenTelescope", new OpenTelescopeCommand(sailingUIController));
            RegisterCommand("SwitchToOutsideWorld", new SwitchToOutsideWorldCommand());
        }

        private void RegisterVesselCommands()
        {
            if (vesselUIController == null) return;

            RegisterCommand("SwitchMode", new SwitchVesselModeCommand(vesselUIController, VesselUIController.VesselUIMode.Grid));
            RegisterCommand("SelectGrid", new SelectGridCommand(vesselUIController, Vector2Int.zero));
            RegisterCommand("SelectCargo", new SelectCargoCommand(vesselUIController));
            RegisterCommand("MoveCargo", new MoveCargoCommand(vesselUIController, Vector2Int.zero));
            RegisterCommand("RotateCargo", new RotateCargoCommand(vesselUIController));
            RegisterCommand("ExitCargoSelection", new ExitCargoSelectionCommand(vesselUIController));
            RegisterCommand("SelectConfusion", new SelectConfusionCommand(vesselUIController, 0));
            RegisterCommand("SelectAbility", new SelectAbilityCommand(vesselUIController, 0));
            RegisterCommand("MoveMap", new MoveMapCommand(vesselUIController, Vector2.zero));
            RegisterCommand("CloseVesselUI", new CloseVesselUICommand(vesselUIController));
        }

        private void RegisterTelescopeCommands()
        {
            if (telescopeUIController == null) return;

            RegisterCommand("SwitchMode", new SwitchTelescopeModeCommand(telescopeUIController, TelescopeUIController.TelescopeMode.View));
            RegisterCommand("CloseTelescope", new CloseTelescopeCommand(telescopeUIController));
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