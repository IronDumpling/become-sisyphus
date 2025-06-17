using UnityEngine;

using System.Linq;
using System.Collections.Generic;

using BecomeSisyphus.Core;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Managers.Systems;

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
            CreateSystemBehaviour<MindOceanSystemBehaviour>("MindOceanSystemBehaviour");
            CreateSystemBehaviour<MemorySystemBehaviour>("MemorySystemBehaviour");
            CreateSystemBehaviour<ExplorationSystemBehaviour>("ExplorationSystemBehaviour");
            CreateSystemBehaviour<SignifierSystemBehaviour>("SignifierSystemBehaviour");
            CreateSystemBehaviour<TimeSystemBehaviour>("TimeSystemBehaviour");
        }

        private void CreateSystemBehaviour<T>(string name) where T : MonoBehaviour
        {
            var go = new GameObject(name);
            go.transform.parent = this.transform;
            go.AddComponent<T>();
        }

        private void InitializeSystems()
        {
            if (config == null)
            {
                Debug.LogError("GameManager: GameConfiguration is not set in inspector!");
                return;
            }

            // 基础系统先初始化
            RegisterSystem(new TimeSystem(config.timeScale, config.dayLength));
            RegisterSystem(new CameraSystem());
            RegisterSystem(new BecomeSisyphus.Core.GameStateSystem.GameStateManager());
            
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
