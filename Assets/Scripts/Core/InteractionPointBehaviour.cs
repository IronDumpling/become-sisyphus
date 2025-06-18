using UnityEngine;
using BecomeSisyphus.Core.Data;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Core
{
    /// <summary>
    /// 场景中的交互点行为组件
    /// 将此组件添加到场景中的交互点物体上，会自动注册到MindOceanSystem
    /// </summary>
    public class InteractionPointBehaviour : MonoBehaviour
    {
        [Header("Interaction Point Settings")]
        [SerializeField] private string interactionId;
        [SerializeField] private InteractionPointType pointType = InteractionPointType.Island;
        [SerializeField] private string displayTitle = "交互点";
        [SerializeField] private string displayDescription = "一个可以交互的地点";
        [SerializeField] private float interactionRadius = 2f;
        
        [Header("Harbor Settings (if Harbor type)")]
        [SerializeField] private float restEffectiveness = 1.2f;
        [SerializeField] private float restDuration = 5f;
        [SerializeField] private bool canRepairVessel = true;
        
        [Header("Lighthouse Settings (if Lighthouse type)")]
        [SerializeField] private float illuminationRadius = 15f;
        [SerializeField] private bool providesNavigation = true;
        [SerializeField] private string lightkeeperMessage = "灯塔守护者的信息";
        
        [Header("Island Settings (if Island type)")]
        [SerializeField] private bool hasSecrets = false;
        [SerializeField] private string explorationReward = "探索奖励";
        
        [Header("Salvage Settings (if Salvage type)")]
        [SerializeField] private float salvageDifficulty = 1f;
        [SerializeField] private bool isExhausted = false;

        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.green;

        private InteractionPoint interactionPoint;
        private MindOceanSystem mindOceanSystem;

        private void Start()
        {
            // Generate ID if not set
            if (string.IsNullOrEmpty(interactionId))
            {
                interactionId = $"{pointType}_{GetInstanceID()}";
            }

            // Get MindOceanSystem reference
            if (GameManager.Instance != null)
            {
                mindOceanSystem = GameManager.Instance.GetSystem<MindOceanSystem>();
                if (mindOceanSystem != null)
                {
                    RegisterInteractionPoint();
                }
                else
                {
                    Debug.LogError($"InteractionPointBehaviour ({gameObject.name}): MindOceanSystem not found!");
                }
            }
            else
            {
                Debug.LogError($"InteractionPointBehaviour ({gameObject.name}): GameManager.Instance is null!");
            }
        }

        private void RegisterInteractionPoint()
        {
            // Create interaction point data
            interactionPoint = new InteractionPoint(interactionId, pointType, transform.position)
            {
                title = displayTitle,
                description = displayDescription,
                interactionRadius = interactionRadius,
                isDiscovered = false,
                isActive = true,
                data = CreateInteractionData()
            };

            // Register with MindOceanSystem
            mindOceanSystem.AddInteractionPoint(interactionPoint);
            
            Debug.Log($"[InteractionPointBehaviour] ✅ Successfully registered {pointType} interaction point '{displayTitle}' (ID: {interactionId}) at position {transform.position} with radius {interactionRadius}");
        }

        private InteractionPointData CreateInteractionData()
        {
            return pointType switch
            {
                InteractionPointType.Harbor => new HarborData
                {
                    restEffectiveness = restEffectiveness,
                    restDuration = restDuration,
                    canRepairVessel = canRepairVessel,
                    availableServices = new System.Collections.Generic.List<string> { "休息", "船只维修", "补给" }
                },
                InteractionPointType.Lighthouse => new LighthouseData
                {
                    illuminationRadius = illuminationRadius,
                    providesNavigation = providesNavigation,
                    lightkeeperMessage = lightkeeperMessage
                },
                InteractionPointType.Island => new IslandData
                {
                    availableResources = new System.Collections.Generic.List<string> { "记忆碎片", "精神之果", "智慧结晶" },
                    hasSecrets = hasSecrets,
                    explorationReward = explorationReward
                },
                InteractionPointType.Salvage => new SalvageData
                {
                    availableItems = new System.Collections.Generic.List<string> { "古老的航海图", "神秘的指南针", "失落的日记" },
                    salvageDifficulty = salvageDifficulty,
                    isExhausted = isExhausted
                },
                InteractionPointType.AbandonedVessel => new AbandonedVesselData
                {
                    vesselType = "废弃商船",
                    salvageableItems = new System.Collections.Generic.List<string> { "船舶零件", "旧日记", "神秘货物" },
                    vesselStory = "这艘船似乎有着不寻常的过去..."
                },
                InteractionPointType.MysteriousRuins => new MysteriousRuinsData
                {
                    ruinType = "古代遗迹",
                    requiresSpecialKey = false,
                    secretReward = "古老的智慧"
                },
                InteractionPointType.MemoryFragment => new MemoryFragmentData
                {
                    memoryContent = "一段珍贵的回忆",
                    mentalStrengthReward = 10f
                },
                InteractionPointType.TreasureSpot => new TreasureSpotData
                {
                    treasureItems = new System.Collections.Generic.List<string> { "金币", "宝石", "神秘物品" },
                    requiresSpecialTool = false,
                    hint = "这里似乎埋藏着什么..."
                },
                _ => new IslandData()
            };
        }

        private void OnDestroy()
        {
            // Unregister from MindOceanSystem when destroyed
            if (mindOceanSystem != null && !string.IsNullOrEmpty(interactionId))
            {
                mindOceanSystem.RemoveInteractionPoint(interactionId);
                Debug.Log($"InteractionPointBehaviour: Unregistered interaction point {interactionId}");
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (showGizmos)
            {
                // Draw interaction radius
                Gizmos.color = gizmoColor;
                Gizmos.DrawWireSphere(transform.position, interactionRadius);
                
                // Draw a small cube at the center
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
            }
        }

        private void OnDrawGizmos()
        {
            if (showGizmos)
            {
                // Always show a small indicator
                Gizmos.color = gizmoColor * 0.5f;
                Gizmos.DrawWireSphere(transform.position, interactionRadius * 0.5f);
            }
        }

        // Editor helper methods
        [System.Serializable]
        public class InteractionPointInfo
        {
            public string id;
            public InteractionPointType type;
            public Vector3 position;
            public float radius;
        }

        public InteractionPointInfo GetInfo()
        {
            return new InteractionPointInfo
            {
                id = interactionId,
                type = pointType,
                position = transform.position,
                radius = interactionRadius
            };
        }

        // Public methods for runtime configuration
        public void SetInteractionRadius(float radius)
        {
            interactionRadius = radius;
            if (interactionPoint != null)
            {
                interactionPoint.interactionRadius = radius;
            }
        }

        public void SetActive(bool active)
        {
            if (interactionPoint != null)
            {
                interactionPoint.isActive = active;
            }
        }

        public string GetInteractionId()
        {
            return interactionId;
        }

        // Public properties for Editor access
        public string DisplayTitle => displayTitle;
        public string DisplayDescription => displayDescription;
        public InteractionPointType PointType => pointType;
        public float InteractionRadius => interactionRadius;
    }
} 