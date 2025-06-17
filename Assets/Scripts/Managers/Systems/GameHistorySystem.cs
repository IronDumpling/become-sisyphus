using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Managers.Systems
{
    public class GameHistorySystem : ISystem
    {
        private List<GameEvent> eventHistory = new List<GameEvent>();
        private Dictionary<string, DateTime> discoveryTimestamps = new Dictionary<string, DateTime>();

        public event Action<GameEvent> OnEventRecorded;

        public void Initialize()
        {
            // 订阅其他系统的事件
            var MindOceanSystem = GameManager.Instance.GetSystem<MindOceanSystem>();
            var vesselSystem = GameManager.Instance.GetSystem<VesselSystem>();

            // 订阅MindOceanSystem事件
            MindOceanSystem.OnRegionEntered += OnRegionEntered;
            MindOceanSystem.OnMarkerDiscovered += OnMarkerDiscovered;
            MindOceanSystem.OnHazardEncountered += OnHazardEncountered;

            // 订阅VesselSystem事件
            vesselSystem.OnCargoCombined += OnCargoCombined;
            vesselSystem.OnLoadRatioChanged += OnLoadRatioChanged;
        }

        public void Update() { }

        private void RecordEvent(string content, GameEventType type, string relatedEntityId = null, Vector3? location = null)
        {
            var entry = new GameEvent
            {
                id = Guid.NewGuid().ToString(),
                timestamp = DateTime.Now,
                content = content,
                type = type,
                relatedEntityId = relatedEntityId,
                location = location ?? Vector3.zero
            };

            eventHistory.Add(entry);
            OnEventRecorded?.Invoke(entry);
        }

        private void OnRegionEntered(MindSeaRegion region)
        {
            if (!region.isDiscovered)
            {
                RecordEvent(
                    $"发现新海域：{region.name}", 
                    GameEventType.Navigation,
                    region.id,
                    region.center
                );
                discoveryTimestamps[region.id] = DateTime.Now;
            }
        }

        private void OnMarkerDiscovered(MapMarker marker)
        {
            string markerType = GetMarkerTypeDescription(marker.type);
            RecordEvent(
                $"发现{markerType}：{marker.description}", 
                GameEventType.Discovery,
                marker.id,
                marker.position
            );
            discoveryTimestamps[marker.id] = DateTime.Now;
        }

        private void OnHazardEncountered(Hazard hazard)
        {
            string hazardType = GetHazardTypeDescription(hazard.type);
            RecordEvent(
                $"遭遇{hazardType}", 
                GameEventType.Exploration,
                hazard.id,
                hazard.position
            );
        }

        private void OnCargoCombined(VesselCargo cargo1, VesselCargo cargo2)
        {
            RecordEvent(
                $"合成：{cargo1.name} + {cargo2.name}", 
                GameEventType.Interaction,
                cargo1.id
            );
        }

        private void OnLoadRatioChanged(float ratio)
        {
            if (ratio >= 0.75f) // 思维负荷警告阈值
            {
                RecordEvent(
                    $"思维负荷过重：{(ratio * 100):F0}%", 
                    GameEventType.System
                );
            }
        }

        private string GetMarkerTypeDescription(MarkerType type)
        {
            switch (type)
            {
                case MarkerType.Lighthouse: return "灯塔";
                case MarkerType.Harbor: return "港口";
                case MarkerType.Island: return "能指小岛";
                case MarkerType.Cloud: return "记忆云朵";
                case MarkerType.Salvage: return "打捞点";
                case MarkerType.Memory: return "记忆";
                case MarkerType.Solution: return "解答";
                case MarkerType.Treasure: return "宝藏";
                default: return "未知标记";
            }
        }

        private string GetHazardTypeDescription(HazardType type)
        {
            switch (type)
            {
                case HazardType.Storm: return "风暴";
                case HazardType.Whirlpool: return "漩涡";
                case HazardType.SeaMonster: return "海怪";
                default: return "未知危险";
            }
        }

        public List<GameEvent> GetHistory(GameEventType? type = null)
        {
            if (type.HasValue)
            {
                return eventHistory.FindAll(entry => entry.type == type.Value);
            }
            return new List<GameEvent>(eventHistory);
        }

        public DateTime? GetDiscoveryTime(string entityId)
        {
            return discoveryTimestamps.TryGetValue(entityId, out DateTime time) ? time : null;
        }

        public void Cleanup()
        {
            eventHistory.Clear();
            discoveryTimestamps.Clear();
            OnEventRecorded = null;

            // 取消订阅事件
            if (GameManager.Instance != null)
            {
                var MindOceanSystem = GameManager.Instance.GetSystem<MindOceanSystem>();
                var vesselSystem = GameManager.Instance.GetSystem<VesselSystem>();

                if (MindOceanSystem != null)
                {
                    MindOceanSystem.OnRegionEntered -= OnRegionEntered;
                    MindOceanSystem.OnMarkerDiscovered -= OnMarkerDiscovered;
                    MindOceanSystem.OnHazardEncountered -= OnHazardEncountered;
                }

                if (vesselSystem != null)
                {
                    vesselSystem.OnCargoCombined -= OnCargoCombined;
                    vesselSystem.OnLoadRatioChanged -= OnLoadRatioChanged;
                }
            }
        }
    }
} 