using UnityEngine;
using TMPro;

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

        private void Awake()
        {
            // 获取或添加组件
            if (textComponent == null)
                textComponent = GetComponentInChildren<TextMeshProUGUI>();
            
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            originalAlpha = canvasGroup.alpha;
        }

        private void Start()
        {
            // 初始淡入效果
            FadeIn();
        }

        private void Update()
        {
            // 脉冲动画
            if (enablePulseAnimation && !string.IsNullOrEmpty(currentText))
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
                canvasGroup.alpha = originalAlpha + pulse;
            }
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
        }

        /// <summary>
        /// 淡入效果
        /// </summary>
        public void FadeIn()
        {
            if (canvasGroup != null)
            {
                // LeanTween.alphaCanvas(canvasGroup, originalAlpha, fadeInDuration).setEaseOutQuart();
            }
        }

        /// <summary>
        /// 淡出效果
        /// </summary>
        public void FadeOut(System.Action onComplete = null)
        {
            if (canvasGroup != null)
            {
                // LeanTween.alphaCanvas(canvasGroup, 0f, fadeOutDuration).setEaseInQuart().setOnComplete(() => onComplete?.Invoke());
            }
        }

        /// <summary>
        /// 启用/禁用脉冲动画
        /// </summary>
        public void SetPulseAnimation(bool enabled)
        {
            enablePulseAnimation = enabled;
            if (!enabled)
            {
                canvasGroup.alpha = originalAlpha;
            }
        }
    }
} 