using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BecomeSisyphus.Core.GameStateSystem;

namespace BecomeSisyphus.UI.Components
{
    /// <summary>
    /// 港口交互窗口
    /// </summary>
    public class HarbourInteractionWindow : InteractionWindowBase
    {
        [Header("Harbour Specific UI")]
        [SerializeField] private Button restButton;
        [SerializeField] private Button startSailingButton;
        [SerializeField] private Button enterOutsideWorldButton;
        [SerializeField] private TextMeshProUGUI restStatusText;
        [SerializeField] private Slider restProgressSlider;

        [Header("Rest Settings")]
        [SerializeField] private float restDuration = 5f;
        [SerializeField] private float restEffectiveness = 1.2f;

        private bool isResting = false;
        private float restTimer = 0f;

        protected override void Awake()
        {
            base.Awake();

            // 设置按钮事件
            if (restButton != null)
                restButton.onClick.AddListener(StartResting);
            
            if (startSailingButton != null)
                startSailingButton.onClick.AddListener(StartSailing);
            
            if (enterOutsideWorldButton != null)
                enterOutsideWorldButton.onClick.AddListener(EnterOutsideWorld);
        }

        protected override void InitializeWindow()
        {
            SetTitle("港口休息站");
            SetDescription("在这里你可以安全地休息，恢复精神力量。");
            
            UpdateUI();
        }

        private void Update()
        {
            if (isResting)
            {
                UpdateResting();
            }
        }

        /// <summary>
        /// 开始休息
        /// </summary>
        private void StartResting()
        {
            isResting = true;
            restTimer = 0f;
            
            if (restButton != null)
                restButton.interactable = false;
            
            UpdateRestStatus("正在休息中...");
            Debug.Log("HarbourInteractionWindow: Started resting");
        }

        /// <summary>
        /// 停止休息
        /// </summary>
        private void StopResting()
        {
            isResting = false;
            restTimer = 0f;
            
            if (restButton != null)
                restButton.interactable = true;
            
            UpdateRestStatus("休息完成");
            
            if (restProgressSlider != null)
                restProgressSlider.value = 0f;
            
            Debug.Log("HarbourInteractionWindow: Stopped resting");
        }

        /// <summary>
        /// 更新休息状态
        /// </summary>
        private void UpdateResting()
        {
            restTimer += Time.deltaTime;
            float progress = restTimer / restDuration;
            
            if (restProgressSlider != null)
                restProgressSlider.value = progress;
            
            // 应用休息效果
            ApplyRestEffect();
            
            if (restTimer >= restDuration)
            {
                StopResting();
            }
        }

        /// <summary>
        /// 应用休息效果
        /// </summary>
        private void ApplyRestEffect()
        {
            // 获取精神力系统并恢复精神力
            if (GameManager.Instance != null)
            {
                var mindSystem = GameManager.Instance.GetSystem<BecomeSisyphus.Managers.Systems.SisyphusMindSystem>();
                if (mindSystem != null)
                {
                    // float regenAmount = mindSystem.MentalStrengthRegenRate * restEffectiveness * Time.deltaTime;
                    // mindSystem.RegenerateMentalStrength(regenAmount, Time.time);
                }
            }
        }

        /// <summary>
        /// 开始航行
        /// </summary>
        private void StartSailing()
        {
            Debug.Log("HarbourInteractionWindow: Starting sailing");
            
            // 切换到航行状态
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SwitchToState("InsideGame/InsideWorld/Sailing");
            }
            
            CloseWindow();
        }

        /// <summary>
        /// 进入外部世界
        /// </summary>
        private void EnterOutsideWorld()
        {
            Debug.Log("HarbourInteractionWindow: Entering outside world");
            
            // 切换到外部世界
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SwitchToLastOutsideWorldState();
            }
            
            CloseWindow();
        }

        /// <summary>
        /// 更新UI状态
        /// </summary>
        private void UpdateUI()
        {
            UpdateRestStatus(isResting ? "正在休息中..." : "准备休息");
            
            if (restProgressSlider != null)
                restProgressSlider.value = isResting ? (restTimer / restDuration) : 0f;
        }

        /// <summary>
        /// 更新休息状态文本
        /// </summary>
        private void UpdateRestStatus(string status)
        {
            if (restStatusText != null)
                restStatusText.text = status;
        }

        protected override void OnWindowClosing()
        {
            base.OnWindowClosing();
            
            // 停止休息
            if (isResting)
            {
                StopResting();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (restButton != null)
                restButton.onClick.RemoveAllListeners();
            
            if (startSailingButton != null)
                startSailingButton.onClick.RemoveAllListeners();
            
            if (enterOutsideWorldButton != null)
                enterOutsideWorldButton.onClick.RemoveAllListeners();
        }
    }
} 