using UnityEngine;
using System;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class Clue
    {
        public string id;
        public string description;
        public ClueSource source;
        public Vector3 position; // 经纬度+深度
        public bool isDiscovered;
    }

    public enum ClueSource
    {
        Internal,
        External
    }
} 