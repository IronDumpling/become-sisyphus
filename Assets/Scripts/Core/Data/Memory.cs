using UnityEngine;
using System;
using System.Collections.Generic;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class Memory
    {
        public string id;
        public string title;
        public string description;
        public MemoryType type;
        public float emotionalValue;
        public Vector3 cloudPosition;    // 在天空中的位置
        public bool isDiscovered;
        public bool isRecalled;
        public List<string> relatedSignifierIds;
        public List<string> unlockedByConfusionIds;

        public event Action<Memory> OnDiscovered;
        public event Action<Memory> OnRecalled;

        public void Discover()
        {
            if (!isDiscovered)
            {
                isDiscovered = true;
                OnDiscovered?.Invoke(this);
            }
        }

        public void Recall()
        {
            if (isDiscovered && !isRecalled)
            {
                isRecalled = true;
                OnRecalled?.Invoke(this);
            }
        }
    }

    public enum MemoryType
    {
        Personal,
        Mythological,
        Philosophical,
        Environmental
    }
} 