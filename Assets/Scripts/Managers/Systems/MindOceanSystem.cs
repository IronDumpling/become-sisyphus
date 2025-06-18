using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Managers.Systems
{
    public class MindOceanSystem : ISystem
    {
        private Dictionary<string, MindSeaRegion> regions = new Dictionary<string, MindSeaRegion>();
        private Dictionary<string, MapMarker> markers = new Dictionary<string, MapMarker>();
        private Dictionary<string, InteractionPoint> interactionPoints = new Dictionary<string, InteractionPoint>();
        private Vector3 currentPosition;
        private MindSeaRegion currentRegion;
        private InteractionPoint nearbyInteractionPoint;
        private SisyphusMindSystem mindSystem;

        public event Action<Vector3> OnPositionChanged;
        public event Action<MindSeaRegion> OnRegionEntered;
        public event Action<MapMarker> OnMarkerDiscovered;
        public event Action<Hazard> OnHazardEncountered;
        public event Action<InteractionPoint> OnInteractionPointEntered;
        public event Action<InteractionPoint> OnInteractionPointExited;
        public event Action<InteractionPoint> OnInteractionPointDiscovered;

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

        public void SetPosition(Vector3 position)
        {
            if (IsValidPosition(position))
            {
                Vector3 oldPosition = currentPosition;
                currentPosition = position;
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

        // Interaction Point Management
        public void AddInteractionPoint(InteractionPoint point)
        {
            if (!interactionPoints.ContainsKey(point.id))
            {
                interactionPoints.Add(point.id, point);
                Debug.Log($"[MindOceanSystem] ✅ Added interaction point {point.id} of type {point.type} at {point.position}. Total points: {interactionPoints.Count}");
            }
            else
            {
                Debug.LogWarning($"[MindOceanSystem] ⚠️ Interaction point {point.id} already exists!");
            }
        }

        public void RemoveInteractionPoint(string pointId)
        {
            if (interactionPoints.ContainsKey(pointId))
            {
                interactionPoints.Remove(pointId);
                Debug.Log($"MindOceanSystem: Removed interaction point {pointId}");
            }
        }

        public InteractionPoint GetInteractionPoint(string pointId)
        {
            interactionPoints.TryGetValue(pointId, out InteractionPoint point);
            return point;
        }

        public InteractionPoint GetNearbyInteractionPoint()
        {
            return nearbyInteractionPoint;
        }

        public List<InteractionPoint> GetAllInteractionPoints()
        {
            return new List<InteractionPoint>(interactionPoints.Values);
        }

        public List<InteractionPoint> GetDiscoveredInteractionPoints()
        {
            var discovered = new List<InteractionPoint>();
            foreach (var point in interactionPoints.Values)
            {
                if (point.isDiscovered)
                {
                    discovered.Add(point);
                }
            }
            return discovered;
        }

        // Removed UpdateInteractionPoints() - now using Trigger system
        
        /// <summary>
        /// Called by trigger system when boat enters interaction range
        /// </summary>
        public void OnInteractionPointTriggered(InteractionPoint point)
        {
            if (point != null && point.isActive)
            {
                // Discover point if not yet discovered
                if (!point.isDiscovered)
                {
                    point.isDiscovered = true;
                    OnInteractionPointDiscovered?.Invoke(point);
                    Debug.Log($"[MindOceanSystem] 🔍 Discovered interaction point {point.id} ({point.type})");
                }

                // Set as nearby point
                if (nearbyInteractionPoint != point)
                {
                    // Exit previous interaction point
                    if (nearbyInteractionPoint != null)
                    {
                        OnInteractionPointExited?.Invoke(nearbyInteractionPoint);
                        Debug.Log($"[MindOceanSystem] ⬅️ Exited interaction range of {nearbyInteractionPoint.id} ({nearbyInteractionPoint.type})");
                    }

                    // Enter new interaction point
                    nearbyInteractionPoint = point;
                    OnInteractionPointEntered?.Invoke(nearbyInteractionPoint);
                    Debug.Log($"[MindOceanSystem] ➡️ Entered interaction range of {nearbyInteractionPoint.id} ({nearbyInteractionPoint.type})");
                }
            }
        }
        
        /// <summary>
        /// Called by trigger system when boat exits interaction range
        /// </summary>
        public void HandleInteractionPointExit(InteractionPoint point)
        {
            if (point != null && nearbyInteractionPoint == point)
            {
                OnInteractionPointExited?.Invoke(nearbyInteractionPoint);
                Debug.Log($"[MindOceanSystem] ⬅️ Exited interaction range of {nearbyInteractionPoint.id} ({nearbyInteractionPoint.type})");
                nearbyInteractionPoint = null;
            }
        }

        private void LoadInitialInteractionPoints()
        {
            // Create some initial interaction points for testing
            // In a real implementation, this would load from configuration or save data
            
            // Harbor (current position based on the screenshot)
            var harborPoint = new InteractionPoint("harbor_01", InteractionPointType.Harbor, new Vector3(0, 0, 0))
            {
                title = "港口",
                description = "在这里你可以安全地休息，恢复精神力量。",
                data = new HarborData
                {
                    restEffectiveness = 1.2f,
                    restDuration = 5f,
                    canRepairVessel = true,
                    availableServices = new List<string> { "休息", "船只维修", "补给" }
                }
            };

            // Lighthouse
            var lighthousePoint = new InteractionPoint("lighthouse_01", InteractionPointType.Lighthouse, new Vector3(10, 5, 0))
            {
                title = "灯塔",
                description = "古老的灯塔为迷失在思想海洋中的船只指引方向。",
                data = new LighthouseData
                {
                    illuminationRadius = 15f,
                    providesNavigation = true,
                    lightkeeperMessage = "欢迎来到思想的灯塔，这里能为你的心灵航程提供指引。"
                }
            };

            // Island
            var islandPoint = new InteractionPoint("island_01", InteractionPointType.Island, new Vector3(-8, 3, 0))
            {
                title = "记忆之岛",
                description = "一座充满回忆的小岛，或许能找到一些有用的东西。",
                data = new IslandData
                {
                    availableResources = new List<string> { "记忆碎片", "精神之果", "智慧结晶" },
                    hasSecrets = true,
                    explorationReward = "解锁新的记忆"
                }
            };

            // Salvage point
            var salvagePoint = new InteractionPoint("salvage_01", InteractionPointType.Salvage, new Vector3(5, -8, 0))
            {
                title = "沉船遗迹",
                description = "一处沉船的遗迹，或许能打捞到有价值的物品。",
                data = new SalvageData
                {
                    availableItems = new List<string> { "古老的航海图", "神秘的指南针", "失落的日记" },
                    salvageDifficulty = 0.8f,
                    isExhausted = false
                }
            };

            AddInteractionPoint(harborPoint);
            AddInteractionPoint(lighthousePoint);
            AddInteractionPoint(islandPoint);
            AddInteractionPoint(salvagePoint);

            Debug.Log($"MindOceanSystem: Loaded {interactionPoints.Count} initial interaction points");
        }

        public void Cleanup()
        {
            regions.Clear();
            markers.Clear();
            interactionPoints.Clear();
            nearbyInteractionPoint = null;
            OnPositionChanged = null;
            OnRegionEntered = null;
            OnMarkerDiscovered = null;
            OnHazardEncountered = null;
            OnInteractionPointEntered = null;
            OnInteractionPointExited = null;
            OnInteractionPointDiscovered = null;
        }
    }
}