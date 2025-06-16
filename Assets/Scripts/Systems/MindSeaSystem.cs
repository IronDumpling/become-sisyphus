using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Systems
{
    public class MindSeaSystem : ISystem
    {
        private float baseMentalStrengthConsumption = 1f;
        private float stormMentalStrengthConsumption = 3f;
        private float vortexMentalStrengthConsumption = 2f;

        private SisyphusMindSystem mindSystem;
        private Vector3 currentPosition;
        private Dictionary<string, MindSeaRegion> discoveredRegions = new Dictionary<string, MindSeaRegion>();
        private List<MapMarker> activeMarkers = new List<MapMarker>();

        public event Action<Vector3> OnPositionChanged;
        public event Action<MindSeaRegion> OnRegionDiscovered;
        public event Action<MapMarker> OnMarkerAdded;
        public event Action<MapMarker> OnMarkerRemoved;
        public event Action<Signifier> OnSignifierDiscovered;

        public void Initialize()
        {
            currentPosition = Vector3.zero;
        }

        public void Update() { }

        public void Update(float deltaTime, float time)
        {
            UpdateMentalStrengthConsumption(deltaTime, time);
        }

        private void HandleExplorationInput()
        {
            // TODO: Implement exploration input handling
        }

        private void UpdateMentalStrengthConsumption(float deltaTime, float time)
        {
            float consumption = baseMentalStrengthConsumption;
            if (IsInStorm())
            {
                consumption += stormMentalStrengthConsumption;
            }
            if (IsInVortex())
            {
                consumption += vortexMentalStrengthConsumption;
            }
            if (mindSystem != null)
                mindSystem.ConsumeMentalStrength(consumption * deltaTime, time);
        }

        public void SetMindSystem(SisyphusMindSystem system)
        {
            mindSystem = system;
        }

        public void MoveToPosition(Vector3 newPosition)
        {
            currentPosition = newPosition;
            OnPositionChanged?.Invoke(currentPosition);
        }

        public void DiscoverRegion(MindSeaRegion region)
        {
            if (!discoveredRegions.ContainsKey(region.id))
            {
                discoveredRegions.Add(region.id, region);
                OnRegionDiscovered?.Invoke(region);
            }
        }

        public void AddMarker(MapMarker marker)
        {
            activeMarkers.Add(marker);
            OnMarkerAdded?.Invoke(marker);
        }

        public void RemoveMarker(MapMarker marker)
        {
            if (activeMarkers.Contains(marker))
            {
                activeMarkers.Remove(marker);
                OnMarkerRemoved?.Invoke(marker);
            }
        }

        public void DiscoverSignifier(Signifier signifier)
        {
            OnSignifierDiscovered?.Invoke(signifier);
        }

        private bool IsInStorm()
        {
            // TODO: 实现风暴区域检测
            return false;
        }

        private bool IsInVortex()
        {
            // TODO: 实现漩涡区域检测
            return false;
        }

        public void Cleanup()
        {
            discoveredRegions.Clear();
            activeMarkers.Clear();
            OnPositionChanged = null;
            OnRegionDiscovered = null;
            OnMarkerAdded = null;
            OnMarkerRemoved = null;
            OnSignifierDiscovered = null;
        }
    }
}