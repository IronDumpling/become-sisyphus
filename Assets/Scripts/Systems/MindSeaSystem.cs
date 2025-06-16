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
        private Dictionary<string, MindSeaRegion> regions = new Dictionary<string, MindSeaRegion>();
        private Dictionary<string, MapMarker> markers = new Dictionary<string, MapMarker>();
        private Vector3 currentPosition;
        private MindSeaRegion currentRegion;
        private SisyphusMindSystem mindSystem;

        public event Action<Vector3> OnPositionChanged;
        public event Action<MindSeaRegion> OnRegionEntered;
        public event Action<MapMarker> OnMarkerDiscovered;
        public event Action<Hazard> OnHazardEncountered;

        public void Initialize()
        {
            mindSystem = GameManager.Instance.GetSystem<SisyphusMindSystem>();
            LoadInitialRegions();
        }

        public void Update()
        {
            UpdateCurrentRegion();
            CheckHazards();
            UpdateMarkers();
        }

        public void Navigate(Vector3 direction)
        {
            Vector3 newPosition = currentPosition + direction;
            if (IsValidPosition(newPosition))
            {
                currentPosition = newPosition;
                OnPositionChanged?.Invoke(currentPosition);
            }
        }

        public void DiscoverRegion(string regionId)
        {
            if (regions.TryGetValue(regionId, out MindSeaRegion region) && !region.isDiscovered)
            {
                region.isDiscovered = true;
                UpdateRegionMarkers(region);
            }
        }

        public void ExploreRegion(string regionId)
        {
            if (regions.TryGetValue(regionId, out MindSeaRegion region) && !region.isExplored)
            {
                region.isExplored = true;
                RevealRegionSecrets(region);
            }
        }

        public void AddMarker(MapMarker marker)
        {
            if (!markers.ContainsKey(marker.id))
            {
                markers.Add(marker.id, marker);
                if (marker.isDiscovered)
                {
                    OnMarkerDiscovered?.Invoke(marker);
                }
            }
        }

        public void DiscoverMarker(string markerId)
        {
            if (markers.TryGetValue(markerId, out MapMarker marker) && !marker.isDiscovered)
            {
                marker.isDiscovered = true;
                OnMarkerDiscovered?.Invoke(marker);
            }
        }

        private void LoadInitialRegions()
        {
            // TODO: 从配置或存档加载初始区域
        }

        private void UpdateCurrentRegion()
        {
            MindSeaRegion newRegion = FindRegionAtPosition(currentPosition);
            if (newRegion != null && newRegion != currentRegion)
            {
                currentRegion = newRegion;
                OnRegionEntered?.Invoke(currentRegion);
            }
        }

        private void CheckHazards()
        {
            if (currentRegion != null && currentRegion.type == RegionType.Dangerous)
            {
                foreach (var hazard in currentRegion.hazards)
                {
                    if (IsWithinHazardRange(hazard))
                    {
                        OnHazardEncountered?.Invoke(hazard);
                        ApplyHazardEffect(hazard);
                    }
                }
            }
        }

        private void ApplyHazardEffect(Hazard hazard)
        {
            if (mindSystem != null)
            {
                float mentalDrain = hazard.intensity * Time.deltaTime;
                mindSystem.ConsumeMentalStrength(mentalDrain, Time.deltaTime);
            }
        }

        private void UpdateMarkers()
        {
            foreach (var marker in markers.Values)
            {
                if (!marker.isDiscovered && IsWithinMarkerRange(marker))
                {
                    DiscoverMarker(marker.id);
                }
            }
        }

        private bool IsValidPosition(Vector3 position)
        {
            // TODO: 实现位置有效性检查
            return true;
        }

        private bool IsWithinHazardRange(Hazard hazard)
        {
            return Vector3.Distance(currentPosition, hazard.position) <= hazard.radius;
        }

        private bool IsWithinMarkerRange(MapMarker marker)
        {
            return Vector3.Distance(currentPosition, marker.position) <= marker.radius;
        }

        private MindSeaRegion FindRegionAtPosition(Vector3 position)
        {
            foreach (var region in regions.Values)
            {
                if (Vector3.Distance(position, region.center) <= region.radius)
                {
                    return region;
                }
            }
            return null;
        }

        private void UpdateRegionMarkers(MindSeaRegion region)
        {
            foreach (var marker in region.markers)
            {
                if (!markers.ContainsKey(marker.id))
                {
                    AddMarker(marker);
                }
            }
        }

        private void RevealRegionSecrets(MindSeaRegion region)
        {
            // TODO: 实现区域秘密揭示逻辑
        }

        public void Cleanup()
        {
            regions.Clear();
            markers.Clear();
            OnPositionChanged = null;
            OnRegionEntered = null;
            OnMarkerDiscovered = null;
            OnHazardEncountered = null;
        }
    }
}