using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class ThoughtBoatSailingController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 100f;

        private MindOceanSystem mindOceanSystem;
        private ExplorationSystem explorationSystem;
        private VesselSystem vesselSystem;
        
        private Vector2 currentVelocity;
        private float currentRotation;

        public enum InteractionType
        {
            Island,
            Salvage,
            Lighthouse,
            Harbor,
            Vessel,
            NavigationMap,
            Telescope
        }

        private void Awake()
        {
            mindOceanSystem = GameManager.Instance.GetSystem<MindOceanSystem>();
            explorationSystem = GameManager.Instance.GetSystem<ExplorationSystem>();
            vesselSystem = GameManager.Instance.GetSystem<VesselSystem>();
        }

        // Movement functionality
        public void Move(Vector2 direction)
        {
            // Normalize direction to ensure consistent movement speed
            direction = Vector2.ClampMagnitude(direction, 1f);
            
            // Update velocity with smooth damping
            currentVelocity = Vector2.Lerp(currentVelocity, direction * moveSpeed, Time.deltaTime * 5f);
            
            // Apply movement
            transform.position += new Vector3(currentVelocity.x, 0, currentVelocity.y) * Time.deltaTime;

            // Handle rotation
            if (direction != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
                currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
                transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            }

            Debug.Log($"Boat moving: Direction={direction}, Velocity={currentVelocity}, Rotation={currentRotation}");
        }

        public void Stop()
        {
            currentVelocity = Vector2.zero;
        }

        // Interaction functionality
        public void OpenInteraction(InteractionType type, string interactionId = null)
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
                case InteractionType.Vessel:
                    OpenVesselUI();
                    break;
                case InteractionType.NavigationMap:
                    OpenNavigationMap();
                    break;
                case InteractionType.Telescope:
                    OpenTelescope();
                    break;
            }
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

        private void OpenLighthouseInteraction(string lighthouseId)
        {
            Debug.Log($"Opening lighthouse interaction for: {lighthouseId}");
            // TODO: 实现灯塔交互界面
        }

        private void OpenHarborInteraction(string harborId)
        {
            Debug.Log($"Opening harbor interaction for: {harborId}");
            // TODO: 实现港口交互界面
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
    }
}