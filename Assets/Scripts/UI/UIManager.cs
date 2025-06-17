using UnityEngine;
using System.Collections.Generic;
using BecomeSisyphus.UI.Components;
using BecomeSisyphus.Core.GameStateSystem;

namespace BecomeSisyphus.UI
{
    /// <summary>
    /// UI管理器 - 根据GameState自动显示/隐藏对应的UI组件
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Prefabs")]
        [SerializeField] private GameObject progressBarPrefab;
        [SerializeField] private GameObject timePrefab;
        [SerializeField] private GameObject hintTextPrefab;
        [SerializeField] private GameObject keyHintsPrefab;
        [SerializeField] private GameObject harbourWindowPrefab;
        [SerializeField] private GameObject lighthouseWindowPrefab;
        [SerializeField] private GameObject salvageWindowPrefab;
        [SerializeField] private GameObject islandWindowPrefab;

        [Header("UI Containers")]
        [SerializeField] private Transform hudContainer;        // HUD元素容器（进度条、时间等）
        [SerializeField] private Transform hintContainer;       // 提示文本容器
        [SerializeField] private Transform windowContainer;     // 窗口容器
        [SerializeField] private Canvas uiCanvas;

        // UI组件实例
        private Dictionary<string, GameObject> activeUIComponents = new Dictionary<string, GameObject>();
        
        // UI状态配置
        private Dictionary<string, UIStateConfig> stateUIConfigs = new Dictionary<string, UIStateConfig>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeUIConfigs();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 订阅GameStateManager的状态变化事件
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnStateTransition += OnGameStateChanged;
                
                // 初始化当前状态的UI
                var currentState = GameStateManager.Instance.CurrentActiveState;
                if (currentState != null)
                {
                    UpdateUIForState(currentState);
                }
            }
            else
            {
                Debug.LogError("UIManager: GameStateManager.Instance is null!");
            }
        }

        /// <summary>
        /// 初始化各个状态的UI配置
        /// </summary>
        private void InitializeUIConfigs()
        {
            // Outside World States
            stateUIConfigs["InsideGame/OutsideWorld/MountainFoot"] = new UIStateConfig
            {
                showProgressBar = false,
                showTimeUI = false,
                hintText = "Press: [Enter] to Start Climbing",
                windowType = WindowType.None
            };

            stateUIConfigs["InsideGame/OutsideWorld/Climbing"] = new UIStateConfig
            {
                showProgressBar = true,
                showTimeUI = true,
                hintText = "Press: [Space] to Enter Inner World",
                windowType = WindowType.None
            };

            stateUIConfigs["InsideGame/InsideWorld"] = new UIStateConfig
            {
                showProgressBar = true,
                showTimeUI = true,
                windowType = WindowType.None
            };

            // Inside World States - Sailing
            stateUIConfigs["InsideGame/InsideWorld/Sailing"] = new UIStateConfig
            {
                showProgressBar = true,
                showTimeUI = true,
                hintText = "Press: [WASD] to Start Sailing",
                windowType = WindowType.None
            };

            // Inside World States - Interactions
            stateUIConfigs["InsideGame/InsideWorld/Interaction"] = new UIStateConfig
            {
                showProgressBar = true,
                showTimeUI = true,
                hintText = "Press: [Esc] to Stop Resting",
                windowType = WindowType.None
            };

            stateUIConfigs["InsideGame/InsideWorld/Interaction/Harbour"] = new UIStateConfig
            {
                showProgressBar = true,
                showTimeUI = true,
                hintText = "Press: [Esc] to Stop Resting",
                windowType = WindowType.Harbour
            };

            stateUIConfigs["InsideGame/InsideWorld/Interaction/Lighthouse"] = new UIStateConfig
            {
                showProgressBar = true,
                showTimeUI = true,
                hintText = "Press: [Esc] to Stop Resting",
                windowType = WindowType.Lighthouse
            };

            stateUIConfigs["InsideGame/InsideWorld/Interaction/Salvage"] = new UIStateConfig
            {
                showProgressBar = true,
                showTimeUI = true,
                hintText = "Press: [Esc] to Stop Salvaging",
                windowType = WindowType.Salvage
            };

            stateUIConfigs["InsideGame/InsideWorld/Interaction/Island"] = new UIStateConfig
            {
                showProgressBar = true,
                showTimeUI = true,
                hintText = "Press: [Esc] to Stop Interaction",
                windowType = WindowType.Island
            };

            Debug.Log("UIManager: UI configurations initialized");
        }

        /// <summary>
        /// 游戏状态变化时的回调
        /// </summary>
        private void OnGameStateChanged(IGameState previousState, IGameState newState)
        {
            Debug.Log($"UIManager: State changed from {previousState?.GetFullStatePath()} to {newState?.GetFullStatePath()}");
            UpdateUIForState(newState);
        }

        /// <summary>
        /// 根据游戏状态更新UI
        /// </summary>
        private void UpdateUIForState(IGameState gameState)
        {
            if (gameState == null) return;

            string statePath = gameState.GetFullStatePath();
            Debug.Log($"UIManager: Updating UI for state: {statePath}");

            // 清除所有现有UI
            ClearAllUI();

            // 查找匹配的UI配置
            if (stateUIConfigs.TryGetValue(statePath, out UIStateConfig config))
            {
                ApplyUIConfig(config);
            }
            else
            {
                Debug.LogWarning($"UIManager: No UI configuration found for state: {statePath}");
            }
        }

        /// <summary>
        /// 应用UI配置
        /// </summary>
        private void ApplyUIConfig(UIStateConfig config)
        {
            // 显示进度条
            if (config.showProgressBar)
            {
                ShowProgressBar();
            }

            // 显示时间UI
            if (config.showTimeUI)
            {
                ShowTimeUI();
            }

            // 显示提示文本
            if (!string.IsNullOrEmpty(config.hintText))
            {
                ShowHintText(config.hintText);
            }

            // 显示按键提示
            if (!string.IsNullOrEmpty(config.keyHints))
            {
                ShowKeyHints(config.keyHints);
            }

            // 显示窗口
            if (config.windowType != WindowType.None)
            {
                ShowWindow(config.windowType);
            }
        }

        /// <summary>
        /// 显示进度条
        /// </summary>
        private void ShowProgressBar()
        {
            if (progressBarPrefab != null && !activeUIComponents.ContainsKey("ProgressBar"))
            {
                GameObject progressBar = Instantiate(progressBarPrefab, hudContainer);
                activeUIComponents["ProgressBar"] = progressBar;
                Debug.Log("UIManager: Progress bar shown");
            }
        }

        /// <summary>
        /// 显示时间UI
        /// </summary>
        private void ShowTimeUI()
        {
            if (timePrefab != null && !activeUIComponents.ContainsKey("TimeUI"))
            {
                GameObject timeUI = Instantiate(timePrefab, hudContainer);
                activeUIComponents["TimeUI"] = timeUI;
                Debug.Log("UIManager: Time UI shown");
            }
        }

        /// <summary>
        /// 显示提示文本
        /// </summary>
        private void ShowHintText(string text)
        {
            if (hintTextPrefab != null && !activeUIComponents.ContainsKey("HintText"))
            {
                GameObject hintText = Instantiate(hintTextPrefab, hintContainer);
                
                // 使用HintTextComponent设置文本内容
                var hintComponent = hintText.GetComponent<HintTextComponent>();
                if (hintComponent != null)
                {
                    hintComponent.SetText(text);
                }
                else
                {
                    // 回退到直接设置TextMeshPro组件
                    var textComponent = hintText.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        textComponent.text = text;
                    }
                    
                    // 确保CanvasGroup可见
                    var canvasGroup = hintText.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 1f;
                    }
                }

                activeUIComponents["HintText"] = hintText;
                Debug.Log($"UIManager: Hint text shown: {text}");
            }
        }

        /// <summary>
        /// 显示按键提示
        /// </summary>
        private void ShowKeyHints(string hints)
        {
            if (keyHintsPrefab != null && !activeUIComponents.ContainsKey("KeyHints"))
            {
                GameObject keyHints = Instantiate(keyHintsPrefab, hintContainer);
                
                // 设置提示内容
                var textComponent = keyHints.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = hints;
                }

                activeUIComponents["KeyHints"] = keyHints;
                Debug.Log($"UIManager: Key hints shown: {hints}");
            }
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        private void ShowWindow(WindowType windowType)
        {
            GameObject windowPrefab = GetWindowPrefab(windowType);
            if (windowPrefab != null && !activeUIComponents.ContainsKey("Window"))
            {
                GameObject window = Instantiate(windowPrefab, windowContainer);
                activeUIComponents["Window"] = window;
                Debug.Log($"UIManager: Window shown: {windowType}");
            }
        }

        /// <summary>
        /// 根据窗口类型获取对应的预制体
        /// </summary>
        private GameObject GetWindowPrefab(WindowType windowType)
        {
            return windowType switch
            {
                WindowType.Harbour => harbourWindowPrefab,
                WindowType.Lighthouse => lighthouseWindowPrefab,
                WindowType.Salvage => salvageWindowPrefab,
                WindowType.Island => islandWindowPrefab,
                _ => null
            };
        }

        /// <summary>
        /// 清除所有UI
        /// </summary>
        private void ClearAllUI()
        {
            foreach (var uiComponent in activeUIComponents.Values)
            {
                if (uiComponent != null)
                {
                    Destroy(uiComponent);
                }
            }
            activeUIComponents.Clear();
            Debug.Log("UIManager: All UI cleared");
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnStateTransition -= OnGameStateChanged;
            }
        }
    }

    /// <summary>
    /// UI状态配置
    /// </summary>
    [System.Serializable]
    public class UIStateConfig
    {
        public bool showProgressBar;
        public bool showTimeUI;
        public string hintText;
        public string keyHints;
        public WindowType windowType;
    }

    /// <summary>
    /// 窗口类型枚举
    /// </summary>
    public enum WindowType
    {
        None,
        Harbour,
        Lighthouse,
        Salvage,
        Island
    }
} 