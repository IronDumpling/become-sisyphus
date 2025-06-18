using UnityEngine;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Managers.Systems;
using DG.Tweening;

namespace BecomeSisyphus.Inputs.Controllers
{
    /// <summary>
    /// 船只航行控制器 - 包含移动控制和交互触发检测
    /// 需要配置SphereCollider (IsTrigger = true) 用于交互检测
    /// </summary>
    [RequireComponent(typeof(Collider))]
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
            
            // Ensure we have a trigger collider for interaction detection
            var collider = GetComponent<Collider>();
            if (collider != null && !collider.isTrigger)
            {
                Debug.LogWarning("[ThoughtBoatSailingController] Collider should be set as Trigger for interaction detection!");
            }
            else if (collider == null)
            {
                Debug.LogError("[ThoughtBoatSailingController] No Collider found! Please add a SphereCollider with IsTrigger = true");
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
                
                // Update position in MindOceanSystem (simplified for trigger system)
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
            Debug.Log($"[ThoughtBoatSailingController] 🏝️ Opening island interaction for: {islandId}");
            
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                // Switch to island interaction state (using Interaction/Harbour as template for now)
                Debug.Log($"[ThoughtBoatSailingController] Switching to island interaction state");
                stateManager.SwitchToState("InsideGame/InsideWorld/Interaction/Harbour");
                
                // Get interaction point data and pass to UI
                if (mindOceanSystem != null)
                {
                    var interactionPoint = mindOceanSystem.GetInteractionPoint(islandId);
                    if (interactionPoint != null)
                    {
                        Debug.Log($"[ThoughtBoatSailingController] Found interaction point: {interactionPoint.title}");
                        // Find and configure the harbor window (reusing for island for now)
                        var harborWindow = FindAnyObjectByType<BecomeSisyphus.UI.Components.HarbourInteractionWindow>();
                        if (harborWindow != null)
                        {
                            harborWindow.SetInteractionPoint(interactionPoint);
                        }
                        else
                        {
                            Debug.LogWarning($"[ThoughtBoatSailingController] ⚠️ HarbourInteractionWindow not found!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[ThoughtBoatSailingController] ⚠️ Interaction point {islandId} not found in MindOceanSystem!");
                    }
                }
            }
            else
            {
                Debug.LogError("[ThoughtBoatSailingController] ❌ GameStateManager not found! Cannot open island interaction properly.");
            }
        }

        public void OpenSalvageInteraction(string salvageId)
        {
            Debug.Log($"[ThoughtBoatSailingController] ⚓ Opening salvage interaction for: {salvageId}");
            
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                // Switch to salvage interaction state (using Interaction/Harbour as template for now)
                Debug.Log($"[ThoughtBoatSailingController] Switching to salvage interaction state");
                stateManager.SwitchToState("InsideGame/InsideWorld/Interaction/Harbour");
                
                // Get interaction point data and pass to UI
                if (mindOceanSystem != null)
                {
                    var interactionPoint = mindOceanSystem.GetInteractionPoint(salvageId);
                    if (interactionPoint != null)
                    {
                        Debug.Log($"[ThoughtBoatSailingController] Found interaction point: {interactionPoint.title}");
                        // Find and configure the harbor window (reusing for salvage for now)
                        var harborWindow = FindAnyObjectByType<BecomeSisyphus.UI.Components.HarbourInteractionWindow>();
                        if (harborWindow != null)
                        {
                            harborWindow.SetInteractionPoint(interactionPoint);
                            Debug.Log($"[ThoughtBoatSailingController] ✅ Configured harbor window for salvage interaction");
                        }
                        else
                        {
                            Debug.LogWarning($"[ThoughtBoatSailingController] ⚠️ HarbourInteractionWindow not found!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[ThoughtBoatSailingController] ⚠️ Interaction point {salvageId} not found in MindOceanSystem!");
                    }
                }
            }
            else
            {
                Debug.LogError("[ThoughtBoatSailingController] ❌ GameStateManager not found! Cannot open salvage interaction properly.");
            }
        }

        private void OpenLighthouseInteraction(string lighthouseId)
        {
            Debug.Log($"[ThoughtBoatSailingController] 🗼 Opening lighthouse interaction for: {lighthouseId}");
            
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                // Switch to lighthouse interaction state (using Interaction/Lighthouse as template for now)
                Debug.Log($"[ThoughtBoatSailingController] Switching to lighthouse interaction state");
                stateManager.SwitchToState("InsideGame/InsideWorld/Interaction/Lighthouse");
                
                // Get interaction point data and pass to UI
                if (mindOceanSystem != null)
                {
                    var interactionPoint = mindOceanSystem.GetInteractionPoint(lighthouseId);
                    if (interactionPoint != null)
                    {
                        Debug.Log($"[ThoughtBoatSailingController] Found interaction point: {interactionPoint.title}");
                        // Find and configure the harbor window (reusing for lighthouse for now)
                        var harborWindow = FindAnyObjectByType<BecomeSisyphus.UI.Components.HarbourInteractionWindow>();
                        if (harborWindow != null)
                        {
                            harborWindow.SetInteractionPoint(interactionPoint);
                            Debug.Log($"[ThoughtBoatSailingController] ✅ Configured harbor window for lighthouse interaction");
                        }
                        else
                        {
                            Debug.LogWarning($"[ThoughtBoatSailingController] ⚠️ HarbourInteractionWindow not found!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[ThoughtBoatSailingController] ⚠️ Interaction point {lighthouseId} not found in MindOceanSystem!");
                    }
                }
            }
            else
            {
                Debug.LogError("[ThoughtBoatSailingController] ❌ GameStateManager not found! Cannot open lighthouse interaction properly.");
            }
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
                        var harborWindow = FindAnyObjectByType<BecomeSisyphus.UI.Components.HarbourInteractionWindow>();
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
            if (mindOceanSystem != null)
            {
                var nearbyPoint = mindOceanSystem.GetNearbyInteractionPoint();
                if (nearbyPoint != null)
                {
                    InteractWithPoint(nearbyPoint);
                }
            }
        }

        private void InteractWithPoint(InteractionPoint point)
        {            
            // Convert to legacy InteractionType and call OpenInteraction
            var legacyType = ConvertToLegacyInteractionType(point.type);
            Debug.Log($"[ThoughtBoatSailingController] Converting {point.type} to legacy type: {legacyType}");
            
            Debug.Log($"[ThoughtBoatSailingController] Calling OpenInteraction({legacyType}, {point.id})");
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

        // ========== INTERACTION TRIGGER DETECTION (3D) ==========
        
        /// <summary>
        /// 3D触发器进入检测 - 检测与交互点的碰撞
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[ThoughtBoatSailingController] 🔍 OnTriggerEnter called with object: {other.name} (Layer: {other.gameObject.layer})");
            
            // Check if we hit an interaction point
            var interactionPointBehaviour = other.GetComponent<InteractionPointBehaviour>();
            if (interactionPointBehaviour != null)
            {                
                if (mindOceanSystem != null)
                {
                    string interactionId = interactionPointBehaviour.GetInteractionId();
                    Debug.Log($"[ThoughtBoatSailingController] 🔍 Looking for interaction point with ID: {interactionId}");
                    
                    // Get the interaction point data
                    var interactionPoint = mindOceanSystem.GetInteractionPoint(interactionId);
                    if (interactionPoint != null)
                    {
                        mindOceanSystem.OnInteractionPointTriggered(interactionPoint);
                    }
                    else
                    {
                        Debug.LogWarning($"[ThoughtBoatSailingController] ❌ InteractionPoint not found for ID: {interactionId}");
                        Debug.LogWarning($"[ThoughtBoatSailingController] Available interaction points: {string.Join(", ", mindOceanSystem.GetAllInteractionPoints().ConvertAll(p => p.id))}");
                    }
                }
                else
                {
                    Debug.LogError($"[ThoughtBoatSailingController] ❌ mindOceanSystem is null!");
                }
            }
        }
        
        /// <summary>
        /// 3D触发器退出检测 - 检测离开交互点
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            // Check if we left an interaction point
            var interactionPointBehaviour = other.GetComponent<InteractionPointBehaviour>();
            if (interactionPointBehaviour != null && mindOceanSystem != null)
            {
                // Get the interaction point data
                var interactionPoint = mindOceanSystem.GetInteractionPoint(interactionPointBehaviour.GetInteractionId());
                if (interactionPoint != null)
                {
                    Debug.Log($"[ThoughtBoatSailingController] 🚪 Exited trigger for {interactionPoint.title} ({interactionPoint.type})");
                    mindOceanSystem.HandleInteractionPointExit(interactionPoint);
                }
            }
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