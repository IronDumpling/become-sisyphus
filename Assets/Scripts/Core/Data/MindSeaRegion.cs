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
        public Vector3 center;
        public float radius;
        public List<string> connectedRegionIds;
        public List<MapMarker> markers;
    }
} 