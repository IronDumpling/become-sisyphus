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
        [SerializeField] private bool createLogbookUIController = true;
        [SerializeField] private bool createVesselUIController = true;

        private InputActionMap currentActionMap;
        private Dictionary<string, ICommand> commandMap = new Dictionary<string, ICommand>();

        // Controllers
        private OutsideWorldController outsideWorldController;
        private ThoughtBoatController thoughtBoatController;
        private LogbookUIController logbookUIController;
        private VesselUIController vesselUIController;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeControllers();
                InitializeInputActions();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeControllers()
        {
            // Create controllers as child objects if needed
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

            if (createLogbookUIController)
            {
                var controllerObj = new GameObject("LogbookUIController");
                controllerObj.transform.SetParent(transform);
                logbookUIController = controllerObj.AddComponent<LogbookUIController>();
            }

            if (createVesselUIController)
            {
                var controllerObj = new GameObject("VesselUIController");
                controllerObj.transform.SetParent(transform);
                vesselUIController = controllerObj.AddComponent<VesselUIController>();
            }

            // Log warnings for any missing controllers
            if (outsideWorldController == null) Debug.LogWarning("OutsideWorldController not created!");
            if (thoughtBoatController == null) Debug.LogWarning("ThoughtBoatController not created!");
            if (logbookUIController == null) Debug.LogWarning("LogbookUIController not created!");
            if (vesselUIController == null) Debug.LogWarning("VesselUIController not created!");
        }

        private void InitializeInputActions()
        {
            if (inputActions == null)
            {
                Debug.LogError("Input Action Asset not assigned to InputManager!");
                return;
            }

            // Enable all action maps but set them to disabled initially
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
                case "Logbook":
                    RegisterLogbookCommands();
                    break;
                case "ThoughtVessel":
                    RegisterThoughtVesselCommands();
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

            RegisterCommand("MoveThoughtBoat", new MoveThoughtBoatCommand(thoughtBoatController, Vector2.zero));
            RegisterCommand("SwitchToOutsideWorld", new SwitchToOutsideWorldCommand());
            RegisterCommand("OpenLogbookMap", new OpenLogbookMapCommand(logbookUIController));
            RegisterCommand("OpenVesselUI", new OpenVesselUICommand(vesselUIController));
            RegisterCommand("OpenLogbookUI", new OpenLogbookUICommand(logbookUIController));
        }

        private void RegisterLogbookCommands()
        {
            if (logbookUIController == null) return;

            RegisterCommand("FlipPage", new FlipLogbookPageCommand(logbookUIController, 0));
            RegisterCommand("MoveMap", new MoveLogbookMapCommand(logbookUIController, Vector2.zero));
            RegisterCommand("SelectLogEntry", new SelectLogEntryCommand(logbookUIController, Vector2.zero));
            RegisterCommand("CloseLogbook", new CloseLogbookCommand(logbookUIController));
        }

        private void RegisterThoughtVesselCommands()
        {
            if (vesselUIController == null) return;

            RegisterCommand("SelectGrid", new SelectVesselGridCommand(vesselUIController, Vector2Int.zero));
            RegisterCommand("SelectCargo", new SelectCargoInVesselCommand(vesselUIController));
            RegisterCommand("MoveCargo", new MoveCargoInVesselCommand(vesselUIController, Vector2Int.zero));
            RegisterCommand("RotateCargo", new RotateCargoInVesselCommand(vesselUIController));
            RegisterCommand("ExitCargoSelection", new ExitCargoSelectionCommand(vesselUIController));
            RegisterCommand("CloseVesselUI", new CloseVesselUICommand(vesselUIController));
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