using System;
using System.Collections.Generic;
using UnityEngine;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class MindSeaRegion
    {
        public string id;
        public string name;
        public string description;
        public Vector3 center;
        public float radius;
        public RegionType type;
        public List<string> connectedRegionIds;
        public List<MapMarker> markers;
        public List<Hazard> hazards;
        public bool isDiscovered;
        public bool isExplored;
    }

    public enum RegionType
    {
        Safe,       // 安全区域，有灯塔或港口
        Dangerous,  // 危险区域，有风暴、漩涡或海怪
        Neutral     // 普通区域，可能有小岛或打捞点
    }

    [Serializable]
    public class Hazard
    {
        public string id;
        public HazardType type;
        public Vector3 position;
        public float intensity;
        public float radius;
    }

    public enum HazardType
    {
        Storm,      // 风暴
        Whirlpool,  // 漩涡
        SeaMonster  // 海怪
    }
} 