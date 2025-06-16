using UnityEngine;

namespace BecomeSisyphus.Core.Interfaces
{
    public interface ICameraSystem : ISystem
    {
        void SwitchToOutsideWorld();
        void SwitchToInsideWorld();
    }
} 