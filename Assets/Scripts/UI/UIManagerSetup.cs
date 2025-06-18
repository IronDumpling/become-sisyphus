using UnityEngine;

namespace BecomeSisyphus.UI
{
    /// <summary>
    /// UIManager设置脚本 - 用于在场景中自动配置UIManager
    /// </summary>
    [RequireComponent(typeof(UIManager))]
    public class UIManagerSetup : MonoBehaviour
    {
        [Header("Auto Setup Settings")]
        [SerializeField] private bool autoLoadPrefabs = true;
        [SerializeField] private bool autoCreateContainers = true;
        
        [Header("Prefab Paths (Resources folder)")]
        [SerializeField] private string progressBarPrefabPath = "Prefabs/UI/ProgressBar";
        [SerializeField] private string timePrefabPath = "Prefabs/UI/Time";
        [SerializeField] private string hintTextPrefabPath = "Prefabs/UI/HintText";
        [SerializeField] private string harbourWindowPrefabPath = "Prefabs/UI/HarbourWindow";
        [SerializeField] private string lighthouseWindowPrefabPath = "Prefabs/UI/LighthouseWindow";
        [SerializeField] private string salvageWindowPrefabPath = "Prefabs/UI/SalvageWindow";
        [SerializeField] private string islandWindowPrefabPath = "Prefabs/UI/IslandWindow";

        [Header("Container Names")]
        [SerializeField] private string hudContainerName = "HUD Container";
        [SerializeField] private string hintContainerName = "Hint Container";
        [SerializeField] private string windowContainerName = "Window Container";

        private UIManager uiManager;

        private void Awake()
        {
            uiManager = GetComponent<UIManager>();
            
            if (autoLoadPrefabs)
            {
                LoadPrefabs();
            }
            
            if (autoCreateContainers)
            {
                CreateContainers();
            }
        }

        /// <summary>
        /// 自动加载预制体
        /// </summary>
        private void LoadPrefabs()
        {
            // 使用反射设置UIManager的私有字段
            var uiManagerType = typeof(UIManager);
            
            // 加载预制体
            SetPrefabField("progressBarPrefab", progressBarPrefabPath);
            SetPrefabField("timePrefab", timePrefabPath);
            SetPrefabField("hintTextPrefab", hintTextPrefabPath);
            SetPrefabField("harbourWindowPrefab", harbourWindowPrefabPath);
            SetPrefabField("lighthouseWindowPrefab", lighthouseWindowPrefabPath);
            SetPrefabField("salvageWindowPrefab", salvageWindowPrefabPath);
            SetPrefabField("islandWindowPrefab", islandWindowPrefabPath);
            
            Debug.Log("UIManagerSetup: Prefabs loaded automatically");
        }

        /// <summary>
        /// 设置预制体字段
        /// </summary>
        private void SetPrefabField(string fieldName, string prefabPath)
        {
            if (string.IsNullOrEmpty(prefabPath)) return;
            
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab != null)
            {
                var field = typeof(UIManager).GetField(fieldName, 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                
                if (field != null)
                {
                    field.SetValue(uiManager, prefab);
                    Debug.Log($"UIManagerSetup: Loaded {fieldName} from {prefabPath}");
                }
            }
            else
            {
                Debug.LogWarning($"UIManagerSetup: Could not load prefab from path: {prefabPath}");
            }
        }

        /// <summary>
        /// 自动创建容器
        /// </summary>
        private void CreateContainers()
        {
            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("UIManagerSetup: No Canvas found in children!");
                return;
            }

            // 创建容器
            Transform hudContainer = CreateContainer(hudContainerName, canvas.transform);
            Transform hintContainer = CreateContainer(hintContainerName, canvas.transform);
            Transform windowContainer = CreateContainer(windowContainerName, canvas.transform);

            // 设置容器字段
            SetContainerField("hudContainer", hudContainer);
            SetContainerField("hintContainer", hintContainer);
            SetContainerField("windowContainer", windowContainer);
            SetContainerField("uiCanvas", canvas);

            Debug.Log("UIManagerSetup: UI containers created automatically");
        }

        /// <summary>
        /// 创建UI容器
        /// </summary>
        private Transform CreateContainer(string containerName, Transform parent)
        {
            // 检查是否已存在
            Transform existing = parent.Find(containerName);
            if (existing != null)
            {
                return existing;
            }

            // 创建新容器
            GameObject container = new GameObject(containerName);
            container.transform.SetParent(parent, false);

            // 添加RectTransform组件
            RectTransform rectTransform = container.AddComponent<RectTransform>();
            
            // 设置为全屏
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            return container.transform;
        }

        /// <summary>
        /// 设置容器字段
        /// </summary>
        private void SetContainerField(string fieldName, Component component)
        {
            var field = typeof(UIManager).GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(uiManager, component);
                Debug.Log($"UIManagerSetup: Set {fieldName} container");
            }
        }

        /// <summary>
        /// 手动设置预制体（在Inspector中调用）
        /// </summary>
        [ContextMenu("Load Prefabs")]
        public void ManualLoadPrefabs()
        {
            LoadPrefabs();
        }

        /// <summary>
        /// 手动创建容器（在Inspector中调用）
        /// </summary>
        [ContextMenu("Create Containers")]
        public void ManualCreateContainers()
        {
            CreateContainers();
        }
    }
} 