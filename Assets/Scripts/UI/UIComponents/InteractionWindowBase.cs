using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using BecomeSisyphus.Core.GameStateSystem;

namespace BecomeSisyphus.UI.Components
{
    /// <summary>
    /// 交互窗口基类 - 所有交互窗口的基础类
    /// </summary>
    public abstract class InteractionWindowBase : MonoBehaviour
    {
        [Header("Window Components")]
        [SerializeField] protected RectTransform windowContainer;
        [SerializeField] protected TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI descriptionText;
        [SerializeField] protected Button closeButton;
        
        [Header("Animation Settings")]
        [SerializeField] protected float openDuration = 0.5f;
        [SerializeField] protected float closeDuration = 0.3f;
        [SerializeField] protected Ease openEase = Ease.OutBack;
        [SerializeField] protected Ease closeEase = Ease.InBack;

        protected CanvasGroup canvasGroup;
        protected Vector3 originalScale;
        protected bool isOpen = false;
        protected Sequence openSequence;
        protected Sequence closeSequence;

        protected virtual void Awake()
        {
            // 获取组件
            if (windowContainer == null)
                windowContainer = GetComponent<RectTransform>();
            
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            originalScale = windowContainer.localScale;

            // 设置关闭按钮事件
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseWindow);
            }
        }

        protected virtual void Start()
        {
            // 初始化窗口内容
            InitializeWindow();
            
            // 播放开启动画
            OpenWindow();
        }

        protected virtual void OnDestroy()
        {
            // Clean up tweens
            openSequence?.Kill();
            closeSequence?.Kill();
            
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 初始化窗口内容 - 子类重写
        /// </summary>
        protected abstract void InitializeWindow();

        /// <summary>
        /// 设置窗口标题
        /// </summary>
        public virtual void SetTitle(string title)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }
        }

        /// <summary>
        /// 设置窗口描述
        /// </summary>
        public virtual void SetDescription(string description)
        {
            if (descriptionText != null)
            {
                descriptionText.text = description;
            }
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public virtual void OpenWindow()
        {
            if (isOpen) return;

            gameObject.SetActive(true);
            isOpen = true;

            // 设置初始状态
            canvasGroup.alpha = 0f;
            windowContainer.localScale = Vector3.zero;

            // 播放开启动画
            openSequence?.Kill();
            openSequence = DOTween.Sequence();
            
            openSequence.Append(canvasGroup.DOFade(1f, openDuration * 0.3f).SetEase(Ease.OutQuart));
            openSequence.Join(windowContainer.DOScale(originalScale, openDuration).SetEase(openEase));
            openSequence.OnComplete(OnWindowOpened);

            OnWindowOpening();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public virtual void CloseWindow()
        {
            if (!isOpen) return;

            isOpen = false;

            // 播放关闭动画
            closeSequence?.Kill();
            closeSequence = DOTween.Sequence();
            
            closeSequence.Append(windowContainer.DOScale(Vector3.zero, closeDuration).SetEase(closeEase));
            closeSequence.Join(canvasGroup.DOFade(0f, closeDuration * 0.7f).SetEase(Ease.InQuart).SetDelay(closeDuration * 0.3f));
            closeSequence.OnComplete(() => {
                gameObject.SetActive(false);
                OnWindowClosed();
            });

            // 切换到航行状态
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SwitchToState("InsideGame/InsideWorld/Sailing");
            }

            OnWindowClosing();
        }

        /// <summary>
        /// 窗口开启中回调
        /// </summary>
        protected virtual void OnWindowOpening() { }

        /// <summary>
        /// 窗口开启完成回调
        /// </summary>
        protected virtual void OnWindowOpened() { }

        /// <summary>
        /// 窗口关闭中回调
        /// </summary>
        protected virtual void OnWindowClosing() { }

        /// <summary>
        /// 窗口关闭完成回调
        /// </summary>
        protected virtual void OnWindowClosed() { }
    }
} 