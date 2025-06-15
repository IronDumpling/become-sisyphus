using UnityEngine;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Systems;

namespace BecomeSisyphus
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public GameState CurrentState { get; private set; }
        
        private Dictionary<System.Type, ISystem> systems = new Dictionary<System.Type, ISystem>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSystems();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSystems()
        {
            // 在这里初始化所有系统
            RegisterSystem(new SisyphusManager());
            RegisterSystem(new ConfusionSystem());
            RegisterSystem(new MemorySystem());
            RegisterSystem(new ExplorationSystem());
            RegisterSystem(new LogbookSystem());
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
