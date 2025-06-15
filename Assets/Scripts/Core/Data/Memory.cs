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
        public List<string> relatedClueIds;
        public bool isRevealed;
    }

    public enum MemoryType
    {
        Personal,
        Mythological,
        Philosophical,
        Environmental
    }
} 