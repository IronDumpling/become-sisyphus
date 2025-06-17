using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace BecomeSisyphus.UI.Components
{
    /// <summary>
    /// 按键提示组件 - 用于显示操作提示
    /// </summary>
    public class KeyHintsComponent : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI hintsText;
        [SerializeField] private RectTransform container;
        
        [Header("Display Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.2f;
        [SerializeField] private float slideDistance = 20f;
        
        [Header("Styling")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightColor = Color.yellow;

        private CanvasGroup canvasGroup;
        private Vector3 originalPosition;
        private List<string> currentHints = new List<string>();

        private void Awake()
        {
            // 获取或添加组件
            if (hintsText == null)
                hintsText = GetComponentInChildren<TextMeshProUGUI>();
            
            if (container == null)
                container = GetComponent<RectTransform>();

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            originalPosition = container.anchoredPosition;
        }

        private void Start()
        {
            // 初始动画
            AnimateIn();
        }

        /// <summary>
        /// 设置按键提示
        /// </summary>
        public void SetHints(string hintsString)
        {
            if (hintsText != null)
            {
                hintsText.text = hintsString;
                hintsText.color = normalColor;
            }
            
            // 解析提示字符串
            ParseHints(hintsString);
        }

        /// <summary>
        /// 设置多个按键提示
        /// </summary>
        public void SetHints(List<string> hints)
        {
            currentHints = new List<string>(hints);
            string combinedHints = string.Join(" | ", hints);
            SetHints(combinedHints);
        }

        /// <summary>
        /// 高亮特定的按键提示
        /// </summary>
        public void HighlightHint(string keyName)
        {
            if (hintsText != null && currentHints.Count > 0)
            {
                string highlightedText = "";
                foreach (string hint in currentHints)
                {
                    if (hint.Contains(keyName))
                    {
                        highlightedText += $"<color=#{ColorUtility.ToHtmlStringRGB(highlightColor)}>{hint}</color>";
                    }
                    else
                    {
                        highlightedText += hint;
                    }
                    
                    if (hint != currentHints[currentHints.Count - 1])
                    {
                        highlightedText += " | ";
                    }
                }
                
                hintsText.text = highlightedText;
            }
        }

        /// <summary>
        /// 清除高亮
        /// </summary>
        public void ClearHighlight()
        {
            if (currentHints.Count > 0)
            {
                SetHints(currentHints);
            }
        }

        /// <summary>
        /// 动画进入
        /// </summary>
        public void AnimateIn()
        {
            if (canvasGroup != null && container != null)
            {
                // 设置初始状态
                canvasGroup.alpha = 0f;
                container.anchoredPosition = originalPosition + Vector3.down * slideDistance;
                
                // 淡入和滑入动画
                // LeanTween.alphaCanvas(canvasGroup, 1f, fadeInDuration).setEaseOutQuart();
                // LeanTween.move(container, originalPosition, fadeInDuration).setEaseOutBack();
            }
        }

        /// <summary>
        /// 动画退出
        /// </summary>
        public void AnimateOut(System.Action onComplete = null)
        {
            if (canvasGroup != null && container != null)
            {
                // 淡出和滑出动画
                // LeanTween.alphaCanvas(canvasGroup, 0f, fadeOutDuration).setEaseInQuart();
                // LeanTween.move(container, originalPosition + Vector3.down * slideDistance, fadeOutDuration)
                //     .setEaseInBack()
                //     .setOnComplete(() => onComplete?.Invoke());
            }
        }

        /// <summary>
        /// 解析提示字符串
        /// </summary>
        private void ParseHints(string hintsString)
        {
            currentHints.Clear();
            if (!string.IsNullOrEmpty(hintsString))
            {
                string[] parts = hintsString.Split('|');
                foreach (string part in parts)
                {
                    currentHints.Add(part.Trim());
                }
            }
        }
    }
} 