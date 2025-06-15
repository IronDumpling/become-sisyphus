using UnityEngine;
using System;
using System.Collections.Generic;

namespace BecomeSisyphus.Core.Data
{
    [Serializable]
    public class Confusion
    {
        public string id;
        public string title;
        public string description;
        public ConfusionType type;
        public float difficulty;
        public bool isKeyConfusion;
        public bool isSolved;
        public DateTime discoveredTime;
        public DateTime? solvedTime;
        public int size;
        public List<string> requiredSignifierIds;
        public List<string> solutionFragmentIds;
        public float timeLimit; // 临时困惑的时间限制，-1表示无限制

        public event Action<Confusion> OnSolved;

        public void Solve()
        {
            if (!isSolved)
            {
                isSolved = true;
                solvedTime = DateTime.Now;
                OnSolved?.Invoke(this);
            }
        }
    }

    public enum ConfusionType
    {
        Philosophical,
        Memory,
        Environmental,
        Emotional,
        Temporary
    }
} 