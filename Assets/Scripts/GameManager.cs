using UnityEngine;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Managers.Systems;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public GameState CurrentState { get; private set; }
        
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
            // Use config to initialize each system with parameters
            RegisterSystem(new SisyphusManager(config.managerMentalStrength, config.managerMaxBrainCapacity, config.managerMentalStrengthRegenRate));
            RegisterSystem(new SisyphusMindSystem(config.maxMentalStrength, config.mentalStrengthRegenRate, config.mentalStrengthRegenDelay));
            RegisterSystem(new ConfusionSystem(config.confusionGenerationInterval, config.temporaryConfusionDuration));
            RegisterSystem(new ThoughtVesselSystem(config.initialRows, config.initialColumns, config.loadRatioThreshold, config.mentalStrengthConsumptionRate));
            RegisterSystem(new MemorySystem());
            RegisterSystem(new ExplorationSystem());
            RegisterSystem(new MindOceanSystem());
            RegisterSystem(new SignifierSystem());
            RegisterSystem(new TimeSystem(config.timeScale, config.dayLength));
        }

        private void RegisterSystem(ISystem system)
        {
            systems[system.GetType()] = system;
            system.Initialize();
        }

        public T GetSystem<T>() where T : ISystem
        {
            if (systems.TryGetValue(typeof(T), out ISystem system))
            {
                return (T)system;
            }
            return default;
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            // 在这里处理状态切换逻辑
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
