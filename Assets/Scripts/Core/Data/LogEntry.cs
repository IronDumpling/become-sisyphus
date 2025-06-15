using System;
using System.Collections.Generic;
using UnityEngine;

namespace BecomeSisyphus.Core.Data
{
    
    [Serializable]
    public class LogEntry
    {
        public DateTime timestamp;
        public string content;
        public LogType type;
    }

    public enum LogType
    {
        System,
        Signifier,
        Confusion,
        Map,
        Exploration
    }
}