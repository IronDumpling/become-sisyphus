using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

namespace BecomeSisyphus.UI.Components
{
    /// <summary>
    /// 提示文本组件 - 用于显示游戏提示信息
    /// 支持淡入淡出动画和自动避免重叠
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
        
        [Header("Overlap Prevention")]
        [SerializeField] private float overlapCheckDistance = 100f;
        [SerializeField] private float verticalOffset = 60f;
        [SerializeField] private bool enableOverlapPrevention = true;
        
        [Header("Auto Fade")]
        [SerializeField] private bool enableAutoFade = true;
        [SerializeField] private float autoFadeDelay = 7f; // 7 seconds default

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private string currentText;
        private float originalAlpha;
        private Tween pulseTween;
        private Tween fadeInTween;
        private Tween fadeOutTween;
        private Tween autoFadeTween;
        private Vector3 originalPosition;

        // Static list to track all active hint components
        private static List<HintTextComponent> activeHints = new List<HintTextComponent>();

        private void Awake()
        {
            // 获取或添加组件
            if (textComponent == null)
                textComponent = GetComponentInChildren<TextMeshProUGUI>();
            
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            rectTransform = GetComponent<RectTransform>();
            
            originalAlpha = 1f;
            canvasGroup.alpha = 0f; // Start invisible for fade in
            
            // Store original position
            if (rectTransform != null)
            {
                Vector2 pos = rectTransform.anchoredPosition;
                originalPosition = new Vector3(pos.x, pos.y, 0f);
            }
        }

        private void Start()
        {
            // Register this hint component
            if (!activeHints.Contains(this))
            {
                activeHints.Add(this);
            }
            
            // Check for overlaps and adjust position if needed
            if (enableOverlapPrevention)
            {
                AdjustPositionToAvoidOverlap();
            }
            
            // Start fade in animation
            FadeIn();
        }

        private void OnDestroy()
        {
            // Clean up tweens
            pulseTween?.Kill();
            fadeInTween?.Kill();
            fadeOutTween?.Kill();
            autoFadeTween?.Kill();
            
            // Remove from active hints list
            if (activeHints.Contains(this))
            {
                activeHints.Remove(this);
            }
        }

        /// <summary>
        /// 设置提示文本并开始动画
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
            
            // Reset auto fade timer when setting new text
            if (enableAutoFade && canvasGroup != null && canvasGroup.alpha > 0f)
            {
                ResetAutoFadeTimer();
            }
        }

        /// <summary>
        /// 改进的淡入效果 - 带有弹性和移动动画
        /// </summary>
        public void FadeIn()
        {
            if (canvasGroup == null) return;
            
            // Kill existing fade tweens
            fadeInTween?.Kill();
            fadeOutTween?.Kill();
            
            // Start from invisible
            canvasGroup.alpha = 0f;
            
            // Scale and position animation for more dynamic effect
            if (rectTransform != null)
            {
                Vector3 startScale = Vector3.one * 0.8f;
                Vector3 targetScale = Vector3.one;
                
                rectTransform.localScale = startScale;
                
                // Animate scale
                rectTransform.DOScale(targetScale, fadeInDuration)
                    .SetEase(Ease.OutBack, 1.2f);
                
                // Animate from slightly below
                Vector2 startPos = rectTransform.anchoredPosition + Vector2.down * 20f;
                rectTransform.anchoredPosition = startPos;
                rectTransform.DOAnchorPos(rectTransform.anchoredPosition + Vector2.up * 20f, fadeInDuration)
                    .SetEase(Ease.OutQuart);
            }
            
            // Fade in alpha
            fadeInTween = canvasGroup.DOFade(originalAlpha, fadeInDuration)
                .SetEase(Ease.OutQuart)
                .OnComplete(() =>
                {
                    // Start pulse animation after fade in completes
                    if (enablePulseAnimation && !string.IsNullOrEmpty(currentText))
                    {
                        StartPulseAnimation();
                    }
                    
                    // Start auto fade timer
                    if (enableAutoFade)
                    {
                        StartAutoFadeTimer();
                    }
                });
        }

        /// <summary>
        /// 改进的淡出效果 - 带有缩放动画
        /// </summary>
        public void FadeOut(System.Action onComplete = null)
        {
            if (canvasGroup == null)
            {
                onComplete?.Invoke();
                return;
            }
            
            // Kill existing tweens
            fadeInTween?.Kill();
            fadeOutTween?.Kill();
            autoFadeTween?.Kill();
            StopPulseAnimation();
            
            // Scale down animation
            if (rectTransform != null)
            {
                rectTransform.DOScale(Vector3.one * 0.8f, fadeOutDuration)
                    .SetEase(Ease.InBack, 1.2f);
                
                // Move slightly down
                rectTransform.DOAnchorPos(rectTransform.anchoredPosition + Vector2.down * 15f, fadeOutDuration)
                    .SetEase(Ease.InQuart);
            }
            
            // Fade out alpha
            fadeOutTween = canvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(Ease.InQuart)
                .OnComplete(() => onComplete?.Invoke());
        }

        /// <summary>
        /// 检查并调整位置以避免与其他提示重叠
        /// </summary>
        private void AdjustPositionToAvoidOverlap()
        {
            if (rectTransform == null || activeHints.Count <= 1) return;
            
            Vector2 currentPos = rectTransform.anchoredPosition;
            bool hasOverlap = true;
            int attempts = 0;
            const int maxAttempts = 10;
            
            while (hasOverlap && attempts < maxAttempts)
            {
                hasOverlap = false;
                
                foreach (var otherHint in activeHints)
                {
                    if (otherHint == this || otherHint == null || otherHint.rectTransform == null)
                        continue;
                    
                    // Calculate distance between hints
                    float distance = Vector2.Distance(currentPos, otherHint.rectTransform.anchoredPosition);
                    
                    if (distance < overlapCheckDistance)
                    {
                        // Move this hint up to avoid overlap
                        currentPos.y += verticalOffset;
                        hasOverlap = true;
                        break;
                    }
                }
                
                attempts++;
            }
            
            // Apply the adjusted position
            if (attempts > 0)
            {
                rectTransform.anchoredPosition = currentPos;
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
            
            float minAlpha = Mathf.Max(0.1f, originalAlpha - pulseIntensity);
            float maxAlpha = Mathf.Min(1f, originalAlpha + pulseIntensity);
            
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

        /// <summary>
        /// 开始自动淡出计时器
        /// </summary>
        private void StartAutoFadeTimer()
        {
            if (!enableAutoFade) return;
            
            // Kill existing auto fade timer
            autoFadeTween?.Kill();
            
            // Start countdown to auto fade
            autoFadeTween = DOVirtual.DelayedCall(autoFadeDelay, () =>
            {
                FadeOut(() =>
                {
                    // Destroy the hint object after fade out completes
                    if (gameObject != null)
                    {
                        Destroy(gameObject);
                    }
                });
            });
        }

        /// <summary>
        /// 停止自动淡出计时器
        /// </summary>
        private void StopAutoFadeTimer()
        {
            autoFadeTween?.Kill();
            autoFadeTween = null;
        }

        /// <summary>
        /// 重置自动淡出计时器（重新开始计时）
        /// </summary>
        public void ResetAutoFadeTimer()
        {
            if (enableAutoFade)
            {
                StopAutoFadeTimer();
                StartAutoFadeTimer();
            }
        }

        /// <summary>
        /// 设置重叠检测参数
        /// </summary>
        public void SetOverlapPrevention(bool enabled, float checkDistance = 100f, float offset = 60f)
        {
            enableOverlapPrevention = enabled;
            overlapCheckDistance = checkDistance;
            verticalOffset = offset;
        }

        /// <summary>
        /// 设置自动淡出参数
        /// </summary>
        public void SetAutoFade(bool enabled, float delay = 7f)
        {
            enableAutoFade = enabled;
            autoFadeDelay = delay;
            
            if (enabled)
            {
                // If we're enabling auto fade and the hint is already visible, start the timer
                if (canvasGroup != null && canvasGroup.alpha > 0f)
                {
                    StartAutoFadeTimer();
                }
            }
            else
            {
                // If we're disabling auto fade, stop the timer
                StopAutoFadeTimer();
            }
        }

        /// <summary>
        /// 手动调整位置（用于外部调用）
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(position.x, position.y);
            }
        }

        /// <summary>
        /// 获取当前位置
        /// </summary>
        public Vector3 GetPosition()
        {
            if (rectTransform != null)
            {
                Vector2 pos = rectTransform.anchoredPosition;
                return new Vector3(pos.x, pos.y, 0f);
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 静态方法：清理所有已销毁的引用
        /// </summary>
        public static void CleanupDestroyedHints()
        {
            activeHints.RemoveAll(hint => hint == null);
        }
    }
} 