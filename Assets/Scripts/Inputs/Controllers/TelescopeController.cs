using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class TelescopeController : MonoBehaviour
    {
        public enum TelescopeMode
        {
            View,    // 查看模式
            Memory   // 回忆模式
        }

        private TelescopeMode currentMode = TelescopeMode.View;

        public void OpenTelescopeUI(TelescopeMode mode = TelescopeMode.View)
        {
            currentMode = mode;
            Debug.Log($"Opening Telescope UI in {mode} mode");
            // TODO: 实现望远镜UI显示逻辑
        }

        public void SwitchMode(TelescopeMode mode)
        {
            currentMode = mode;
            Debug.Log($"Switching to {mode} mode");
            // TODO: 实现模式切换逻辑
        }

        public void CloseTelescopeUI()
        {
            Debug.Log("Closing Telescope UI");
            // Use new state system to return to sailing
            var stateManager = BecomeSisyphus.Core.GameStateSystem.GameStateManager.Instance;
            if (stateManager != null)
            {
                stateManager.SwitchToState("InsideGame/InsideWorld/Sailing");
            }
            else
            {
                Debug.LogError("GameStateManager not found! Cannot close telescope UI properly.");
            }
            // TODO: 实现UI关闭逻辑
        }
    }
} 