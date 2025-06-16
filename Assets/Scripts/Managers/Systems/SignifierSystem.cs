using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Managers.Systems
{
    public class SignifierSystem : ISystem
    {
        private Dictionary<string, Signifier> discoveredSignifiers = new Dictionary<string, Signifier>();
        private SisyphusMindSystem mindSystem;
        private ThoughtVesselSystem vesselSystem;

        public event Action<Signifier> OnSignifierDiscovered;
        public event Action<Signifier> OnSignifierUpdated;

        public void Initialize()
        {
            mindSystem = GameManager.Instance.GetSystem<SisyphusMindSystem>();
            vesselSystem = GameManager.Instance.GetSystem<ThoughtVesselSystem>();
        }

        public void Update()
        {
            // 检查是否需要更新能指状态
        }

        public void DiscoverSignifier(Signifier signifier)
        {
            if (!discoveredSignifiers.ContainsKey(signifier.id))
            {
                discoveredSignifiers.Add(signifier.id, signifier);
                OnSignifierDiscovered?.Invoke(signifier);
            }
        }

        public void UpdateSignifier(Signifier signifier)
        {
            if (discoveredSignifiers.ContainsKey(signifier.id))
            {
                discoveredSignifiers[signifier.id] = signifier;
                OnSignifierUpdated?.Invoke(signifier);
            }
        }

        public bool TryPerceiveSignifier(Signifier signifier)
        {
            if (mindSystem.GetAbilityLevel("Perception") >= 1)
            {
                DiscoverSignifier(signifier);
                return true;
            }
            return false;
        }

        public bool TryRecallSignifier(Signifier signifier)
        {
            if (mindSystem.GetAbilityLevel("Memory") >= 1)
            {
                DiscoverSignifier(signifier);
                return true;
            }
            return false;
        }

        public Signifier GetSignifier(string id)
        {
            return discoveredSignifiers.TryGetValue(id, out Signifier signifier) ? signifier : null;
        }

        public List<Signifier> GetAvailableSignifiers(SignifierType type)
        {
            List<Signifier> result = new List<Signifier>();
            foreach (var signifier in discoveredSignifiers.Values)
            {
                if (signifier.type == type)
                {
                    result.Add(signifier);
                }
            }
            return result;
        }

        public void Cleanup()
        {
            discoveredSignifiers.Clear();
            OnSignifierDiscovered = null;
            OnSignifierUpdated = null;
        }
    }
} 