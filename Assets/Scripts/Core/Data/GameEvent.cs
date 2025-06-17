using System;
using UnityEngine;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class GameEvent
    {
        public string id;
        public DateTime timestamp;
        public string content;
        public GameEventType type;
        public string relatedEntityId;  // 关联的实体ID（如果有）
        public Vector3 location;        // 事件发生的位置（如果有）
    }

    public enum GameEventType
    {
        System,         // 系统事件
        Discovery,      // 发现事件（能指、记忆等）
        Confusion,      // 困惑相关
        Navigation,     // 航行相关
        Exploration,    // 探索相关
        Interaction,    // 交互相关
        Achievement     // 成就相关
    }
} 