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
    }

    public enum MarkerType
    {
        Signifier,
        Memory,
        Solution,
        Treasure
    }
} 