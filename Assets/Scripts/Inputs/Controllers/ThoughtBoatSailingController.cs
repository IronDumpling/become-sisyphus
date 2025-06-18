using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Managers.Systems;
using DG.Tweening;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class ThoughtBoatSailingController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float accelerationTime = 0.2f;
        [SerializeField] private float decelerationTime = 0.4f;

        // References
        private SisyphusMindSystem mindSystem;
        private ThoughtBoatSystem boatSystem;
        private MindOceanSystem mindOceanSystem;
        private ExplorationSystem explorationSystem;
        private VesselSystem vesselSystem;
        
        // Movement state
        private Vector2 currentVelocity;
        private Vector2 targetVelocity;
        private Vector2 moveDirection;
        private bool isMoving;
        
        // DOTween references
        private Tween movementTween;
        private Tween velocityTween;

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
            // Initialize movement state
            currentVelocity = Vector2.zero;
            targetVelocity = Vector2.zero;
            moveDirection = Vector2.zero;
            isMoving = false;
            
            Debug.Log("ThoughtBoatSailingController: Initialized DOTween-based movement system");
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
                
                // Subscribe to MindOceanSystem events
                if (mindOceanSystem != null)
                {
                    mindOceanSystem.OnInteractionPointEntered += OnInteractionPointEntered;
                    mindOceanSystem.OnInteractionPointExited += OnInteractionPointExited;
                    mindOceanSystem.OnInteractionPointDiscovered += OnInteractionPointDiscovered;
                    Debug.Log("ThoughtBoatSailingController: Subscribed to MindOceanSystem events");
                }
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

        private void Update()
        {
            // Apply mental cost at intervals while moving
            if (isMoving && Time.time - lastMentalCostTime >= mentalCostInterval)
            {
                ApplyMentalCost();
                lastMentalCostTime = Time.time;
            }

            // Apply current velocity to position
            if (currentVelocity.magnitude > 0.01f)
            {
                Vector3 deltaPosition = new Vector3(currentVelocity.x, currentVelocity.y, 0) * Time.deltaTime;
                transform.position += deltaPosition;
                
                // Update position in MindOceanSystem
                UpdatePositionInMindOcean();
            }
        }

        private void UpdatePositionInMindOcean()
        {
            if (mindOceanSystem != null)
            {
                Vector3 currentPosition = new Vector3(transform.position.x, transform.position.y, 0);
                mindOceanSystem.SetPosition(currentPosition);
            }
        }

        // Called by input system
        public void Move(Vector2 direction)
        {
            Vector2 previousDirection = moveDirection;
            bool wasMoving = isMoving;
            
            moveDirection = Vector2.ClampMagnitude(direction, 1f);
            bool shouldMove = moveDirection != Vector2.zero;
            
            if (shouldMove != isMoving || (shouldMove && Vector2.Distance(previousDirection, moveDirection) > 0.1f))
            {
                if (shouldMove)
                {
                    StartMovement();
                    Debug.Log($"ThoughtBoatSailingController: Started/Changed movement - Direction={moveDirection}");
                }
                else
                {
                    StopMovement();
                    Debug.Log("ThoughtBoatSailingController: Stopped input - Boat will decelerate smoothly");
                }
            }
            
            isMoving = shouldMove;
        }

        private void StartMovement()
        {
            // Calculate target velocity
            targetVelocity = moveDirection * moveSpeed;
            
            // Kill existing velocity tween
            velocityTween?.Kill();
            
            // Smoothly accelerate to target velocity
            velocityTween = DOTween.To(
                () => currentVelocity,
                velocity => currentVelocity = velocity,
                targetVelocity,
                accelerationTime
            ).SetEase(Ease.OutQuart);
        }

        private void StopMovement()
        {
            // Kill existing velocity tween
            velocityTween?.Kill();
            
            // Smoothly decelerate to zero (inertia effect)
            velocityTween = DOTween.To(
                () => currentVelocity,
                velocity => currentVelocity = velocity,
                Vector2.zero,
                decelerationTime
            ).SetEase(Ease.OutQuart);
        }

        public void Stop()
        {
            moveDirection = Vector2.zero;
            isMoving = false;
            StopMovement();
            Debug.Log("ThoughtBoatSailingController: Stop called - boat will decelerate smoothly");
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
            
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                // Switch to harbor interaction state
                stateManager.SwitchToState("InsideGame/InsideWorld/Interaction/Harbour");
                
                // Get interaction point data and pass to UI
                if (mindOceanSystem != null)
                {
                    var interactionPoint = mindOceanSystem.GetInteractionPoint(harborId);
                    if (interactionPoint != null)
                    {
                        // Find and configure the harbor window
                        var harborWindow = FindObjectOfType<BecomeSisyphus.UI.Components.HarbourInteractionWindow>();
                        if (harborWindow != null)
                        {
                            harborWindow.SetInteractionPoint(interactionPoint);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("GameStateManager not found! Cannot open harbor interaction properly.");
            }
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

        // MindOceanSystem event handlers
        private void OnInteractionPointEntered(InteractionPoint point)
        {
            Debug.Log($"ThoughtBoatSailingController: Entered interaction range of {point.title} ({point.type})");
            ShowInteractionHint(point);
        }

        private void OnInteractionPointExited(InteractionPoint point)
        {
            Debug.Log($"ThoughtBoatSailingController: Exited interaction range of {point.title} ({point.type})");
            HideInteractionHint();
        }

        private void OnInteractionPointDiscovered(InteractionPoint point)
        {
            Debug.Log($"ThoughtBoatSailingController: Discovered new interaction point: {point.title} ({point.type})");
        }

        private void ShowInteractionHint(InteractionPoint point)
        {
            // Only show hint if in sailing state
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                var currentState = stateManager.CurrentActiveState;
                var statePath = currentState?.GetFullStatePath();
                
                if (statePath != null && statePath.Contains("Sailing"))
                {
                    // Show hint text through UI system
                    var uiManager = FindAnyObjectByType<BecomeSisyphus.UI.UIManager>();
                    if (uiManager != null)
                    {
                        string hintText = GetInteractionHintText(point);
                        uiManager.ShowInteractionHint(hintText);
                    }
                }
            }
        }

        private void HideInteractionHint()
        {
            // Hide hint text through UI system
            var uiManager = FindAnyObjectByType<BecomeSisyphus.UI.UIManager>();
            if (uiManager != null)
            {
                uiManager.HideInteractionHint();
            }
        }

        private string GetInteractionHintText(InteractionPoint point)
        {
            return point.type switch
            {
                InteractionPointType.Harbor => "Press [Enter] to dock at Harbor",
                InteractionPointType.Lighthouse => "Press [Enter] to approach Lighthouse",
                InteractionPointType.Island => "Press [Enter] to explore Island",
                InteractionPointType.Salvage => "Press [Enter] to investigate Salvage",
                InteractionPointType.AbandonedVessel => "Press [Enter] to board Abandoned Vessel",
                InteractionPointType.MysteriousRuins => "Press [Enter] to examine Mysterious Ruins",
                InteractionPointType.MemoryFragment => "Press [Enter] to collect Memory Fragment",
                InteractionPointType.TreasureSpot => "Press [Enter] to search for Treasure",
                _ => "Press [Enter] to interact"
            };
        }

        /// <summary>
        /// Called by input system when interaction key is pressed
        /// </summary>
        public void TryInteractWithNearbyPoint()
        {
            Debug.Log("[ThoughtBoatSailingController] 🎯 TryInteractWithNearbyPoint called");
            
            if (mindOceanSystem != null)
            {
                var nearbyPoint = mindOceanSystem.GetNearbyInteractionPoint();
                if (nearbyPoint != null)
                {
                    Debug.Log($"[ThoughtBoatSailingController] ✅ Found nearby point {nearbyPoint.id} ({nearbyPoint.type}) - Attempting interaction");
                    InteractWithPoint(nearbyPoint);
                }
                else
                {
                    Debug.Log("[ThoughtBoatSailingController] ❌ No nearby interaction point found");
                    // Debug: Show all available points and distances
                    var allPoints = mindOceanSystem.GetAllInteractionPoints();
                    Debug.Log($"[ThoughtBoatSailingController] 📍 Available interaction points: {allPoints.Count}");
                    Vector3 boatPosition = transform.position;
                    foreach (var point in allPoints)
                    {
                        float distance = Vector3.Distance(boatPosition, point.position);
                        Debug.Log($"  - {point.id} ({point.type}) at distance {distance:F2} (radius: {point.interactionRadius}, active: {point.isActive})");
                    }
                }
            }
            else
            {
                Debug.LogError("[ThoughtBoatSailingController] ❌ mindOceanSystem is null!");
            }
        }

        private void InteractWithPoint(InteractionPoint point)
        {
            Debug.Log($"ThoughtBoatSailingController: Interacting with {point.title} ({point.type})");
            
            // Convert to legacy InteractionType and call OpenInteraction
            var legacyType = ConvertToLegacyInteractionType(point.type);
            OpenInteraction(legacyType, point.id);
        }

        private InteractionType ConvertToLegacyInteractionType(InteractionPointType pointType)
        {
            return pointType switch
            {
                InteractionPointType.Island => InteractionType.Island,
                InteractionPointType.Lighthouse => InteractionType.Lighthouse,
                InteractionPointType.Harbor => InteractionType.Harbor,
                InteractionPointType.Salvage => InteractionType.Salvage,
                InteractionPointType.AbandonedVessel => InteractionType.Salvage, // Use salvage UI for now
                InteractionPointType.MysteriousRuins => InteractionType.Island, // Use island UI for now
                InteractionPointType.MemoryFragment => InteractionType.Island, // Use island UI for now
                InteractionPointType.TreasureSpot => InteractionType.Salvage, // Use salvage UI for now
                _ => InteractionType.Island
            };
        }

        private void OnDestroy()
        {
            // Clean up DOTween sequences
            movementTween?.Kill();
            velocityTween?.Kill();
            
            // Unsubscribe from events
            if (mindOceanSystem != null)
            {
                mindOceanSystem.OnInteractionPointEntered -= OnInteractionPointEntered;
                mindOceanSystem.OnInteractionPointExited -= OnInteractionPointExited;
                mindOceanSystem.OnInteractionPointDiscovered -= OnInteractionPointDiscovered;
            }
        }
    }
}