using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class ThoughtVesselController : MonoBehaviour
    {
        public enum VesselUIMode
        {
            Grid,           // 思维格整理模式
            Confusions,     // 困惑列表模式
            MindAbilities,  // 精神能力模式
            Navigation      // 航海图模式
        }

        private VesselUIMode currentMode = VesselUIMode.Grid;
        private VesselSystem vesselSystem;
        private bool isInCargoSelectionMode = false;

        void Awake()
        {
            vesselSystem = GameManager.Instance.GetSystem<VesselSystem>();
        }

        public void OpenVesselUI(VesselUIMode mode = VesselUIMode.Grid)
        {
            currentMode = mode;
            Debug.Log($"Opening Vessel UI in {mode} mode");
            // TODO: 实现UI显示逻辑
        }

        public void SwitchMode(VesselUIMode mode)
        {
            currentMode = mode;
            Debug.Log($"Switching to {mode} mode");
            // TODO: 实现模式切换逻辑
        }

        public void SelectGrid(Vector2Int direction)
        {
            if (currentMode != VesselUIMode.Grid) return;
            Debug.Log($"Selecting grid at: {direction}");
            // TODO: 实现格子选择逻辑
        }

        public void SelectCargo()
        {
            if (currentMode != VesselUIMode.Grid) return;
            isInCargoSelectionMode = true;
            Debug.Log("Selecting cargo");
            // TODO: 实现货物选择逻辑
        }

        public void MoveCargo(Vector2Int direction)
        {
            if (!isInCargoSelectionMode) return;
            Debug.Log($"Moving selected cargo: {direction}");
            // TODO: 实现货物移动逻辑
        }

        public void RotateCargo()
        {
            if (!isInCargoSelectionMode) return;
            Debug.Log("Rotating selected cargo");
            // TODO: 实现货物旋转逻辑
        }

        public void ExitCargoSelection()
        {
            if (!isInCargoSelectionMode) return;
            isInCargoSelectionMode = false;
            Debug.Log("Exiting cargo selection");
            // TODO: 实现退出选择逻辑
        }

        public void SelectConfusion(int direction)
        {
            if (currentMode != VesselUIMode.Confusions) return;
            Debug.Log($"Selecting confusion: {direction}");
            // TODO: 实现困惑选择逻辑
        }

        public void SelectAbility(int direction)
        {
            if (currentMode != VesselUIMode.MindAbilities) return;
            Debug.Log($"Selecting ability: {direction}");
            // TODO: 实现能力选择逻辑
        }

        public void MoveMap(Vector2 direction)
        {
            if (currentMode != VesselUIMode.Navigation) return;
            Debug.Log($"Moving map: {direction}");
            // TODO: 实现地图移动逻辑
        }

        public void CloseVesselUI()
        {
            Debug.Log("Closing Vessel UI");
            GameManager.Instance.ChangeState(GameState.Sailing);
            // TODO: 实现UI关闭逻辑
        }
    }
} 