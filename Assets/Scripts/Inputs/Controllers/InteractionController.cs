using UnityEngine;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class InteractionController : MonoBehaviour
    {
        private MindOceanSystem MindOceanSystem;
        private ExplorationSystem explorationSystem;

        public enum InteractionType
        {
            Island,
            Salvage,
            Lighthouse,
            Harbor
        }

        private void Awake()
        {
            MindOceanSystem = GameManager.Instance.GetSystem<MindOceanSystem>();
            explorationSystem = GameManager.Instance.GetSystem<ExplorationSystem>();
        }

        public void OpenInteraction(InteractionType type, string interactionId)
        {
            switch (type)
            {
                case InteractionType.Island:
                    OpenIslandInteraction(interactionId);
                    break;
                case InteractionType.Salvage:
                    OpenSalvageInteraction(interactionId);
                    break;
                case InteractionType.Lighthouse:
                    OpenLighthouseInteraction(interactionId);
                    break;
                case InteractionType.Harbor:
                    OpenHarborInteraction(interactionId);
                    break;
            }
        }

        private void OpenIslandInteraction(string islandId)
        {
            // Implementation
        }

        private void OpenSalvageInteraction(string salvageId)
        {
            // Implementation
        }

        private void OpenLighthouseInteraction(string lighthouseId)
        {
            // Implementation
        }

        private void OpenHarborInteraction(string harborId)
        {
            // Implementation
        }

        public void CloseCurrentInteraction()
        {
            // Implementation
        }
    }
} 