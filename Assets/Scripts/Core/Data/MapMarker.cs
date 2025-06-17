using System;
using UnityEngine;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class MapMarker
    {
        public string id;
        public MarkerType type;
        public Vector3 position;
        public string description;
        public bool isDiscovered;
        public bool isActive;
        public float radius;
        public string linkedEntityId; // 关联的实体ID（记忆、能指等）
    }

    public enum MarkerType
    {
        Lighthouse,  // 灯塔
        Harbor,      // 港口
        Island,      // 小岛（已发现的能指）
        Cloud,       // 云（已发现的记忆）
        Salvage,     // 打捞点
        Hazard,       // 危险
        Memory,      // 记忆
        Solution,    // 解答
        Treasure     // 宝藏
    }
} 