using UnityEngine;

namespace BecomeSisyphus.Core
{
    public interface ISystem
    {
        void Initialize();
        void Update();
        void Cleanup();
    }
} 