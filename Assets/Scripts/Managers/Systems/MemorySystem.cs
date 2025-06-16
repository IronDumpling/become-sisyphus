using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Managers.Systems
{
    public class MemorySystem : ISystem
    {
        private Dictionary<string, Memory> discoveredMemories = new Dictionary<string, Memory>();
        private SisyphusManager sisyphusManager;

        public event Action<Memory> OnMemoryDiscovered;
        public event Action<Memory> OnMemoryUpdated;

        public void Initialize()
        {
            sisyphusManager = GameManager.Instance.GetSystem<SisyphusManager>();
        }

        public void Update()
        {
            // 检查新的记忆发现
            CheckForNewMemories();
        }

        private void CheckForNewMemories()
        {
            // TODO: 实现记忆发现逻辑
            // 基于游戏进度、当前状态等条件发现新的记忆
        }

        public void DiscoverMemory(Memory memory)
        {
            if (!discoveredMemories.ContainsKey(memory.id))
            {
                discoveredMemories.Add(memory.id, memory);
                OnMemoryDiscovered?.Invoke(memory);
            }
        }

        public void UpdateMemory(Memory memory)
        {
            if (discoveredMemories.ContainsKey(memory.id))
            {
                discoveredMemories[memory.id] = memory;
                OnMemoryUpdated?.Invoke(memory);
            }
        }

        public Memory GetMemory(string id)
        {
            return discoveredMemories.TryGetValue(id, out Memory memory) ? memory : null;
        }

        public void Cleanup()
        {
            discoveredMemories.Clear();
            OnMemoryDiscovered = null;
            OnMemoryUpdated = null;
        }
    } 
}