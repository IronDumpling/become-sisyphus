using UnityEngine;
using TMPro;
using DG.Tweening;

namespace BecomeSisyphus.UI.Components
{
    /// <summary>
    /// 提示文本组件 - 用于显示游戏提示信息
    /// </summary>
    public class HintTextComponent : MonoBehaviour
    {
        [Header("Text Settings")]
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        
        [Header("Animation")]
        [SerializeField] private bool enablePulseAnimation = true;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseIntensity = 0.2f;

        private CanvasGroup canvasGroup;
        private string currentText;
        private float originalAlpha;
        private Tween pulseTween;

        private void Awake()
        {
            // 获取或添加组件
            if (textComponent == null)
                textComponent = GetComponentInChildren<TextMeshProUGUI>();
            
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            originalAlpha = 1f; // Set default alpha to 1
            canvasGroup.alpha = 0f; // Start invisible for fade in
        }

        private void Start()
        {
            // 初始淡入效果
            FadeIn();
        }

        private void OnDestroy()
        {
            // Clean up tweens
            pulseTween?.Kill();
        }

        /// <summary>
        /// 设置提示文本
        /// </summary>
        public void SetText(string text)
        {
            currentText = text;
            if (textComponent != null)
            {
                textComponent.text = text;
            }
            
            // Start pulse animation if enabled
            if (enablePulseAnimation && !string.IsNullOrEmpty(currentText))
            {
                StartPulseAnimation();
            }
        }

        /// <summary>
        /// 淡入效果
        /// </summary>
        public void FadeIn()
        {
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(originalAlpha, fadeInDuration).SetEase(Ease.OutQuart);
            }
        }

        /// <summary>
        /// 淡出效果
        /// </summary>
        public void FadeOut(System.Action onComplete = null)
        {
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuart).OnComplete(() => onComplete?.Invoke());
            }
        }

        /// <summary>
        /// 启用/禁用脉冲动画
        /// </summary>
        public void SetPulseAnimation(bool enabled)
        {
            enablePulseAnimation = enabled;
            if (enabled && !string.IsNullOrEmpty(currentText))
            {
                StartPulseAnimation();
            }
            else
            {
                StopPulseAnimation();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = originalAlpha;
                }
            }
        }

        /// <summary>
        /// 开始脉冲动画
        /// </summary>
        private void StartPulseAnimation()
        {
            if (canvasGroup == null) return;
            
            StopPulseAnimation(); // Stop any existing pulse
            
            float minAlpha = originalAlpha - pulseIntensity;
            float maxAlpha = originalAlpha + pulseIntensity;
            
            pulseTween = canvasGroup.DOFade(maxAlpha, 1f / pulseSpeed)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .From(minAlpha);
        }

        /// <summary>
        /// 停止脉冲动画
        /// </summary>
        private void StopPulseAnimation()
        {
            pulseTween?.Kill();
            pulseTween = null;
        }
    }
} 