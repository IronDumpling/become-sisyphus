using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class ThoughtBoatSailingController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float accelerationTime = 0.2f;
        [SerializeField] private float decelerationTime = 0.1f;

        // References
        private Rigidbody2D rb;
        private SisyphusMindSystem mindSystem;
        private ThoughtBoatSystem boatSystem;
        private MindOceanSystem mindOceanSystem;
        private ExplorationSystem explorationSystem;
        private VesselSystem vesselSystem;
        
        private Vector2 currentVelocity;
        private float currentRotation;
        private Vector2 moveDirection;
        private bool isMoving;

        [Header("Mental Cost")]
        [SerializeField] private float movementMentalCost = 0.5f;
        [SerializeField] private float mentalCostInterval = 1f;
        private float lastMentalCostTime;

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
            // Setup Rigidbody2D for physics-based movement
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;
                rb.linearDamping = 0.5f;
                rb.angularDamping = 0.5f;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                Debug.Log("ThoughtBoatSailingController: Added Rigidbody2D component");
            }
        }

        private void Start()
        {
            // Get system references in Start() to ensure GameManager is fully initialized
            if (GameManager.Instance != null)
            {
                mindSystem = GameManager.Instance.GetSystem<SisyphusMindSystem>();
                boatSystem = GameManager.Instance.GetSystem<ThoughtBoatSystem>();
            mindOceanSystem = GameManager.Instance.GetSystem<MindOceanSystem>();
            explorationSystem = GameManager.Instance.GetSystem<ExplorationSystem>();
            vesselSystem = GameManager.Instance.GetSystem<VesselSystem>();
                
                Debug.Log($"ThoughtBoatSailingController: Got system references - " +
                         $"MindSystem={mindSystem != null}, " +
                         $"BoatSystem={boatSystem != null}, " +
                         $"MindOceanSystem={mindOceanSystem != null}, " +
                         $"ExplorationSystem={explorationSystem != null}, " +
                         $"VesselSystem={vesselSystem != null}");
            }
            else
            {
                Debug.LogError("ThoughtBoatSailingController: GameManager.Instance is null in Start");
                return;
            }

            // Register to ThoughtBoatSystem
            if (boatSystem != null)
            {
                boatSystem.RegisterActiveBoat(this);
                Debug.Log("ThoughtBoatSailingController: Registered to ThoughtBoatSystem");
            }
            else
            {
                Debug.LogWarning("ThoughtBoatSailingController: boatSystem is null, cannot register");
                
                // Add more debugging information
                if (GameManager.Instance != null)
                {
                    Debug.LogWarning("ThoughtBoatSailingController: GameManager exists but ThoughtBoatSystem not found. Check if ThoughtBoatSystem is properly registered in GameManager.InitializeSystems()");
                }
            }
        }

        private void FixedUpdate()
        {
            if (isMoving && moveDirection != Vector2.zero)
            {
                // Apply mental cost at intervals while moving
                if (Time.time - lastMentalCostTime >= mentalCostInterval)
                {
                    ApplyMentalCost();
                    lastMentalCostTime = Time.time;
                }

                // Calculate target velocity
                Vector2 targetVelocity = moveDirection * moveSpeed;
                
                // Smoothly accelerate to target velocity
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 
                    Time.fixedDeltaTime / accelerationTime);

                // Handle rotation
                float targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                float newRotation = Mathf.LerpAngle(rb.rotation, targetRotation, 
                    Time.fixedDeltaTime * rotationSpeed);
                rb.MoveRotation(newRotation);
            }
            else
            {
                // Apply deceleration when not moving (inertia effect)
                if (rb.linearVelocity.magnitude > 0.01f)
                {
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 
                        Time.fixedDeltaTime / decelerationTime);
                }
                else
                {
                    // Stop completely when velocity is very small
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }

        // Called by input system
        public void Move(Vector2 direction)
        {
            Vector2 previousDirection = moveDirection;
            bool wasMoving = isMoving;
            
            moveDirection = Vector2.ClampMagnitude(direction, 1f);
            isMoving = moveDirection != Vector2.zero;
            
            // Log state changes
            if (wasMoving != isMoving)
            {
                if (isMoving)
            {
                    Debug.Log($"ThoughtBoatSailingController: Started moving - Direction={moveDirection}");
                }
                else
                {
                    Debug.Log("ThoughtBoatSailingController: Stopped input - Boat will decelerate with inertia");
                }
            }
            else if (isMoving && Vector2.Distance(previousDirection, moveDirection) > 0.1f)
            {
                Debug.Log($"ThoughtBoatSailingController: Direction changed - New direction={moveDirection}");
            }
        }

        public void Stop()
        {
            moveDirection = Vector2.zero;
            isMoving = false;
            Debug.Log("ThoughtBoatSailingController: Stop called");
        }

        private void ApplyMentalCost()
        {
            if (mindSystem != null)
            {
                mindSystem.ConsumeMentalStrength(movementMentalCost, Time.time);
                Debug.Log($"ThoughtBoatSailingController: Applied mental cost: {movementMentalCost}");
            }
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
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                stateManager.SwitchToState("InsideGame/InsideWorld/ThoughtBoatCabin/ThoughtVessel");
            }
            else
            {
                Debug.LogError("GameStateManager not found! Cannot open vessel UI properly.");
            }
        }

        public void OpenNavigationMap()
        {
            Debug.Log("Opening navigation map");
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                stateManager.SwitchToState("InsideGame/InsideWorld/ThoughtBoatCabin/NavigationMap");
            }
            // TODO: 实现航海图界面
        }

        public void OpenTelescope()
        {
            Debug.Log("Opening telescope");
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                stateManager.SwitchToState("InsideGame/InsideWorld/Telescope");
            }
            else
            {
                Debug.LogError("GameStateManager not found! Cannot open telescope properly.");
            }
        }

        public void SwitchToOutsideWorld()
        {
            Debug.Log("Switching to outside world");
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                stateManager.SwitchToState("InsideGame/OutsideWorld/Climbing");
            }
            else
            {
                Debug.LogError("GameStateManager not found! Cannot switch to outside world properly.");
            }
        }
    }
}