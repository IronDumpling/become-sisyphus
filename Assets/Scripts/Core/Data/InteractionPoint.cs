using System;
using System.Collections.Generic;
using UnityEngine;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class InteractionPoint
    {
        public string id;
        public InteractionPointType type;
        public Vector3 position;
        public float interactionRadius;
        public string title;
        public string description;
        public bool isDiscovered;
        public bool isActive;
        public InteractionPointData data;

        public InteractionPoint(string id, InteractionPointType type, Vector3 position)
        {
            this.id = id;
            this.type = type;
            this.position = position;
            this.interactionRadius = 2f; // Default interaction radius
            this.isDiscovered = false;
            this.isActive = true;
        }
    }

    public enum InteractionPointType
    {
        Island,         // 小岛
        Lighthouse,     // 灯塔
        Harbor,         // 港口
        Salvage,        // 打捞点
        AbandonedVessel,// 废弃船只
        MysteriousRuins,// 神秘遗迹
        MemoryFragment, // 记忆碎片
        TreasureSpot    // 宝藏点
    }

    [Serializable]
    public abstract class InteractionPointData
    {
        public abstract InteractionPointType GetPointType();
    }

    [Serializable]
    public class HarborData : InteractionPointData
    {
        public float restEffectiveness = 1.2f;
        public float restDuration = 5f;
        public bool canRepairVessel = true;
        public List<string> availableServices = new List<string>();

        public override InteractionPointType GetPointType() => InteractionPointType.Harbor;
    }

    [Serializable]
    public class LighthouseData : InteractionPointData
    {
        public float illuminationRadius = 10f;
        public bool providesNavigation = true;
        public string lightkeeperMessage;

        public override InteractionPointType GetPointType() => InteractionPointType.Lighthouse;
    }

    [Serializable]
    public class IslandData : InteractionPointData
    {
        public List<string> availableResources = new List<string>();
        public bool hasSecrets = false;
        public string explorationReward;

        public override InteractionPointType GetPointType() => InteractionPointType.Island;
    }

    [Serializable]
    public class SalvageData : InteractionPointData
    {
        public List<string> availableItems = new List<string>();
        public float salvageDifficulty = 1f;
        public bool isExhausted = false;

        public override InteractionPointType GetPointType() => InteractionPointType.Salvage;
    }

    [Serializable]
    public class AbandonedVesselData : InteractionPointData
    {
        public string vesselType;
        public List<string> salvageableItems = new List<string>();
        public string vesselStory;

        public override InteractionPointType GetPointType() => InteractionPointType.AbandonedVessel;
    }

    [Serializable]
    public class MysteriousRuinsData : InteractionPointData
    {
        public string ruinType;
        public bool requiresSpecialKey = false;
        public string secretReward;

        public override InteractionPointType GetPointType() => InteractionPointType.MysteriousRuins;
    }

    [Serializable]
    public class MemoryFragmentData : InteractionPointData
    {
        public string memoryContent;
        // public MemoryType memoryType;
        public float mentalStrengthReward = 10f;

        public override InteractionPointType GetPointType() => InteractionPointType.MemoryFragment;
    }

    [Serializable]
    public class TreasureSpotData : InteractionPointData
    {
        public List<string> treasureItems = new List<string>();
        public bool requiresSpecialTool = false;
        public string hint;

        public override InteractionPointType GetPointType() => InteractionPointType.TreasureSpot;
    }
} 