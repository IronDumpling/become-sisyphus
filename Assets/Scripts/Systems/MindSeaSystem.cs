using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Systems
{
    public class MindSeaSystem : MonoBehaviour, ISystem
    {
        [SerializeField] private float baseMentalStrengthConsumption = 1f;
        [SerializeField] private float stormMentalStrengthConsumption = 3f;
        [SerializeField] private float vortexMentalStrengthConsumption = 2f;

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
            mindSystem = GameManager.Instance.GetSystem<SisyphusMindSystem>();
            currentPosition = Vector3.zero;
        }

        public void Update()
        {
            if (GameManager.Instance.CurrentState == GameState.ExploringMind)
            {
                HandleExplorationInput();
                UpdateMentalStrengthConsumption();
            }
        }

        private void HandleExplorationInput()
        {
            // TODO: 实现探索输入处理
            // 处理玩家输入，移动思维小船
            // 检查是否发现新的区域、标记或能指
        }

        private void UpdateMentalStrengthConsumption()
        {
            float consumption = baseMentalStrengthConsumption;
            
            // 检查当前位置是否在危险区域
            if (IsInStorm())
            {
                consumption += stormMentalStrengthConsumption;
            }
            if (IsInVortex())
            {
                consumption += vortexMentalStrengthConsumption;
            }

            mindSystem.ConsumeMentalStrength(consumption * Time.deltaTime);
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