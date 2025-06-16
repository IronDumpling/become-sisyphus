using UnityEngine;
using System;
using System.Collections.Generic;

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
        public List<string> combinableWithIds; // 可以与哪些载荷合成
        public string resultCargoId;           // 合成后的结果ID

        public event Action<VesselCargo> OnPositionChanged;
        public event Action<VesselCargo> OnRotated;
        public event Action<VesselCargo, VesselCargo> OnCombined;

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

        public bool CanCombineWith(VesselCargo other)
        {
            return combinableWithIds != null && combinableWithIds.Contains(other.id);
        }

        public void CombineWith(VesselCargo other)
        {
            if (CanCombineWith(other))
            {
                OnCombined?.Invoke(this, other);
            }
        }
    }

    public enum CargoType
    {
        Confusion,  // 困惑
        Signifier,  // 能指
        Solution,   // 解答
        Fragment    // 解答碎片
    }
} 