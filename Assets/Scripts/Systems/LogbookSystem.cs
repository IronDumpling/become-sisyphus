using UnityEngine;
using System;
using System.Collections.Generic;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Systems
{
    public class LogbookSystem : MonoBehaviour, ISystem
    {
        private List<LogEntry> logEntries = new List<LogEntry>();
        private Dictionary<string, Signifier> signifierRecords = new Dictionary<string, Signifier>();
        private Dictionary<string, Confusion> confusionRecords = new Dictionary<string, Confusion>();
        private Dictionary<string, MindSeaRegion> mapRecords = new Dictionary<string, MindSeaRegion>();

        public event Action<LogEntry> OnNewLogEntry;
        public event Action<Signifier> OnSignifierRecorded;
        public event Action<Confusion> OnConfusionRecorded;
        public event Action<MindSeaRegion> OnMapRecorded;

        public void Initialize()
        {
            // 初始化日志系统
        }

        public void Update()
        {
            // 检查是否需要记录新的日志
        }

        public void AddLogEntry(string content, BecomeSisyphus.Core.Data.LogType type)
        {
            LogEntry entry = new LogEntry
            {
                timestamp = DateTime.Now,
                content = content,
                type = type
            };

            logEntries.Add(entry);
            OnNewLogEntry?.Invoke(entry);
        }

        public void RecordSignifier(Signifier signifier)
        {
            if (!signifierRecords.ContainsKey(signifier.id))
            {
                signifierRecords.Add(signifier.id, signifier);
                OnSignifierRecorded?.Invoke(signifier);
                AddLogEntry($"发现新的能指：{signifier.name}", BecomeSisyphus.Core.Data.LogType.Signifier);
            }
        }

        public void RecordConfusion(Confusion confusion)
        {
            if (!confusionRecords.ContainsKey(confusion.id))
            {
                confusionRecords.Add(confusion.id, confusion);
                OnConfusionRecorded?.Invoke(confusion);
                AddLogEntry($"记录新的困惑：{confusion.title}", BecomeSisyphus.Core.Data.LogType.Confusion);
            }
        }

        public void RecordMap(MindSeaRegion region)
        {
            if (!mapRecords.ContainsKey(region.id))
            {
                mapRecords.Add(region.id, region);
                OnMapRecorded?.Invoke(region);
                AddLogEntry($"发现新的海域：{region.name}", BecomeSisyphus.Core.Data.LogType.Map);
            }
        }

        public List<LogEntry> GetLogEntries(BecomeSisyphus.Core.Data.LogType? type = null)
        {
            if (type.HasValue)
            {
                return logEntries.FindAll(entry => entry.type == type.Value);
            }
            return new List<LogEntry>(logEntries);
        }

        public Signifier GetSignifierRecord(string id)
        {
            return signifierRecords.TryGetValue(id, out Signifier signifier) ? signifier : null;
        }

        public Confusion GetConfusionRecord(string id)
        {
            return confusionRecords.TryGetValue(id, out Confusion confusion) ? confusion : null;
        }

        public MindSeaRegion GetMapRecord(string id)
        {
            return mapRecords.TryGetValue(id, out MindSeaRegion region) ? region : null;
        }

        public void Cleanup()
        {
            logEntries.Clear();
            signifierRecords.Clear();
            confusionRecords.Clear();
            mapRecords.Clear();
            OnNewLogEntry = null;
            OnSignifierRecorded = null;
            OnConfusionRecorded = null;
            OnMapRecorded = null;
        }
    }
} 