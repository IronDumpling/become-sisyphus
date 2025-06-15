using UnityEngine;
using System;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class VesselCargo
    {
        public string id;
        public string name;
        public string description;
        public CargoType type;
        public int width;
        public int height;
        public float weight;
        public bool isRotated;
        public Vector2Int position;

        public event Action<VesselCargo> OnPositionChanged;
        public event Action<VesselCargo> OnRotated;

        public void SetPosition(Vector2Int newPosition)
        {
            position = newPosition;
            OnPositionChanged?.Invoke(this);
        }

        public void Rotate()
        {
            isRotated = !isRotated;
            int temp = width;
            width = height;
            height = temp;
            OnRotated?.Invoke(this);
        }
    }

    public enum CargoType
    {
        Memory,
        Insight,
        Emotion,
        Confusion
    }
} 