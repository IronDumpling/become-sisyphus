using UnityEngine;
using System;
using System.Collections.Generic;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class Signifier
    {
        public string id;
        public string name;
        public string description;
        public SignifierType type;
        public float value;
        public bool isDiscovered;
        public DateTime discoveredTime;

        public event Action<Signifier> OnDiscovered;

        public void Discover()
        {
            if (!isDiscovered)
            {
                isDiscovered = true;
                discoveredTime = DateTime.Now;
                OnDiscovered?.Invoke(this);
            }
        }
    }

    public enum SignifierType
    {
        Memory,
        Insight,
        Emotion,
        Environment
    }
} 