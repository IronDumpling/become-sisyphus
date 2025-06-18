using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BecomeSisyphus.Core.GameStateSystem;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.UI.Components
{
    /// <summary>
    /// 港口交互窗口
    /// </summary>
    public class HarbourInteractionWindow : InteractionWindowBase
    {
        [Header("Harbour Specific UI")]
        [SerializeField] private Button restButton;
        [SerializeField] private TextMeshProUGUI restStatusText;

        [Header("Rest Settings")]
        [SerializeField] private float restDuration = 5f;
        [SerializeField] private float restEffectiveness = 1.2f;

        private bool isResting = false;
        private float restTimer = 0f;
        private InteractionPoint currentInteractionPoint;
        private HarborData harborData;

        protected override void Awake()
        {
            base.Awake();

            // 设置按钮事件
            if (restButton != null)
                restButton.onClick.AddListener(StartResting);
        }

        /// <summary>
        /// Set the interaction point data for this window
        /// </summary>
        public void SetInteractionPoint(InteractionPoint interactionPoint)
        {
            currentInteractionPoint = interactionPoint;
            if (interactionPoint?.data is HarborData harbor)
            {
                harborData = harbor;
                restDuration = harbor.restDuration;
                restEffectiveness = harbor.restEffectiveness;
            }
        }

        protected override void InitializeWindow()
        {
            if (currentInteractionPoint != null)
            {
                SetTitle(currentInteractionPoint.title);
                SetDescription(currentInteractionPoint.description);
            }
            else
            {
                SetTitle("港口");
                SetDescription("在这里你可以安全地休息，恢复精神力量。");
            }
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
            
            Debug.Log("HarbourInteractionWindow: Stopped resting");
        }

        /// <summary>
        /// 更新休息状态
        /// </summary>
        private void UpdateResting()
        {
            restTimer += Time.deltaTime;
            float progress = restTimer / restDuration;
            
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
        }
    }
} 