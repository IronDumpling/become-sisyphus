using UnityEngine;

using System.Linq;
using System.Collections.Generic;

using BecomeSisyphus.Core;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Managers.Systems;
using BecomeSisyphus.Managers.Behaviours;

namespace BecomeSisyphus
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private Dictionary<System.Type, ISystem> systems = new Dictionary<System.Type, ISystem>();
        
        [SerializeField] private GameConfiguration config;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                CreateSystemBehaviours();
                InitializeSystems();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void CreateSystemBehaviours()
        {
            CreateSystemBehaviour<SisyphusManagerBehaviour>("SisyphusManagerBehaviour");
            CreateSystemBehaviour<SisyphusMindSystemBehaviour>("SisyphusMindSystemBehaviour");
            CreateSystemBehaviour<ConfusionSystemBehaviour>("ConfusionSystemBehaviour");
            CreateSystemBehaviour<ThoughtVesselSystemBehaviour>("ThoughtVesselSystemBehaviour");
            CreateSystemBehaviour<ThoughtBoatSystemBehaviour>("ThoughtBoatSystemBehaviour");
            CreateSystemBehaviour<MindOceanSystemBehaviour>("MindOceanSystemBehaviour");
            CreateSystemBehaviour<MemorySystemBehaviour>("MemorySystemBehaviour");
            CreateSystemBehaviour<ExplorationSystemBehaviour>("ExplorationSystemBehaviour");
            CreateSystemBehaviour<SignifierSystemBehaviour>("SignifierSystemBehaviour");
            CreateSystemBehaviour<TimeSystemBehaviour>("TimeSystemBehaviour");
            
            // Create UI Manager
            CreateUIManager();
        }

        private void CreateSystemBehaviour<T>(string name) where T : MonoBehaviour
        {
            var go = new GameObject(name);
            go.transform.parent = this.transform;
            go.AddComponent<T>();
        }

        private void CreateUIManager()
        {
            // Check if UIManager already exists in scene
            if (BecomeSisyphus.UI.UIManager.Instance != null)
            {
                Debug.Log("GameManager: UIManager already exists in scene");
                return;
            }

            // Create UIManager GameObject as a root object (no parent)
            var uiManagerGO = new GameObject("UIManager");
            
            // Add Canvas
            var canvas = uiManagerGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Ensure UI is on top
            
            // Add CanvasScaler
            var canvasScaler = uiManagerGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
            
            // Add GraphicRaycaster
            uiManagerGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Add UIManager component
            uiManagerGO.AddComponent<BecomeSisyphus.UI.UIManager>();
            
            // Add UIManagerSetup for automatic configuration
            uiManagerGO.AddComponent<BecomeSisyphus.UI.UIManagerSetup>();

            // Since it's a root object, DontDestroyOnLoad will work correctly
            DontDestroyOnLoad(uiManagerGO);
            
            Debug.Log("GameManager: UIManager created and configured as root GameObject");
        }

        private void InitializeSystems()
        {
            if (config == null)
            {
                Debug.LogError("GameManager: GameConfiguration is not set in inspector!");
                return;
            }

            // 基础系统先初始化
            RegisterSystem(new GameStateManager());
            RegisterSystem(new TimeSystem(config.timeScale, config.dayLength));
            RegisterSystem(new CameraSystem());
            
            // 核心游戏系统
            RegisterSystem(new SisyphusManager(
                config.managerMentalStrength, 
                config.managerMaxBrainCapacity, 
                config.managerMentalStrengthRegenRate
            ));
            RegisterSystem(new SisyphusMindSystem(
                config.maxMentalStrength, 
                config.mentalStrengthRegenRate, 
                config.mentalStrengthRegenDelay
            ));
            
            // 游戏机制系统
            RegisterSystem(new ConfusionSystem(
                config.confusionGenerationInterval, 
                config.temporaryConfusionDuration
            ));
            RegisterSystem(new ThoughtVesselSystem(
                config.initialRows, 
                config.initialColumns, 
                config.loadRatioThreshold, 
                config.mentalStrengthConsumptionRate
            ));
            
            // 辅助系统
            RegisterSystem(new MemorySystem());
            RegisterSystem(new ExplorationSystem());
            RegisterSystem(new MindOceanSystem());
            RegisterSystem(new SignifierSystem());
            RegisterSystem(new ThoughtBoatSystem());

            Debug.Log("GameManager: All systems initialized successfully");
        }

        private void RegisterSystem(ISystem system)
        {
            Debug.Log($"GameManager: Registering system: {system.GetType().Name}");
            systems[system.GetType()] = system;
            Debug.Log($"GameManager: Initializing system: {system.GetType().Name}");
            system.Initialize();
            Debug.Log($"GameManager: System {system.GetType().Name} registered and initialized successfully");
        }

        public T GetSystem<T>() where T : ISystem
        {
            Debug.Log($"GameManager: Attempting to get system: {typeof(T).Name}");
            
            if (systems.TryGetValue(typeof(T), out ISystem system))
            {
                Debug.Log($"GameManager: Found system: {typeof(T).Name}");
                return (T)system;
            }
            
            // Debug.LogError($"GameManager: System not found: {typeof(T).Name}");
            // Debug.LogError($"GameManager: Available systems: {string.Join(", ", systems.Keys.Select(k => k.Name))}");
            return default;
        }
        
        // Debug method to list all registered systems
        [ContextMenu("List All Systems")]
        public void ListAllSystems()
        {
            Debug.Log($"GameManager: Total registered systems: {systems.Count}");
            foreach (var kvp in systems)
            {
                Debug.Log($"  - {kvp.Key.Name}: {kvp.Value}");
            }
        }

        private void Update()
        {
            foreach (var system in systems.Values)
            {
                system.Update();
            }
        }

        private void OnDestroy()
        {
            foreach (var system in systems.Values)
            {
                system.Cleanup();
            }
        }
    }
}
