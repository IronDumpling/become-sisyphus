using UnityEngine;

namespace BecomeSisyphus.Core.Interfaces
{
    public interface ISystem
    {
        void Initialize();
        void Update();
        void Cleanup();
    }
} 