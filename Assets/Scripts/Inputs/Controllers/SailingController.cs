using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class SailingController : MonoBehaviour
    {
        private MindOceanSystem MindOceanSystem;
        private VesselSystem vesselSystem;

        void Awake()
        {
            MindOceanSystem = GameManager.Instance.GetSystem<MindOceanSystem>();
            vesselSystem = GameManager.Instance.GetSystem<VesselSystem>();
        }

        public void OpenIslandInteraction(string islandId)
        {
            Debug.Log($"Opening island interaction for: {islandId}");
            // TODO: 实现小岛交互界面
        }

        public void OpenSalvageInteraction(string salvageId)
        {
            Debug.Log($"Opening salvage interaction for: {salvageId}");
            // TODO: 实现打捞点交互界面
        }

        public void OpenVesselUI()
        {
            Debug.Log("Opening vessel UI");
            GameManager.Instance.ChangeState(GameState.Vessel);
        }

        public void OpenNavigationMap()
        {
            Debug.Log("Opening navigation map");
            // TODO: 实现航海图界面
        }

        public void OpenTelescope()
        {
            Debug.Log("Opening telescope");
            GameManager.Instance.ChangeState(GameState.Telescope);
        }

        public void SwitchToOutsideWorld()
        {
            Debug.Log("Switching to outside world");
            GameManager.Instance.ChangeState(GameState.Climbing);
        }

        public void CloseSailingUI()
        {
            Debug.Log("Closing sailing UI");
            // TODO: 实现关闭界面逻辑
        }
    }
} 