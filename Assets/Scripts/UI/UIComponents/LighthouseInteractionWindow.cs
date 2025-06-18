using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BecomeSisyphus.Core.GameStateSystem;

namespace BecomeSisyphus.UI.Components
{
    /// <summary>
    /// 灯塔交互窗口
    /// </summary>
    public class LighthouseInteractionWindow : InteractionWindowBase
    {
        [Header("Lighthouse Specific UI")]
        [SerializeField] private Button observeButton;
        [SerializeField] private Button restButton;
        [SerializeField] private TextMeshProUGUI observationText;
        [SerializeField] private Image lighthouseImage;

        [Header("Lighthouse Settings")]
        [SerializeField] private string[] observationMessages = {
            "你看到远方的船只在海上航行...",
            "灯塔的光芒照亮了前方的道路...",
            "海面上波光粼粼，充满了未知的可能...",
            "你感受到了内心的平静与力量..."
        };

        protected override void Awake()
        {
            base.Awake();

            // 设置按钮事件
            if (observeButton != null)
                observeButton.onClick.AddListener(ObserveSurroundings);
            
            if (restButton != null)
                restButton.onClick.AddListener(RestAtLighthouse);
        }

        protected override void InitializeWindow()
        {
            SetTitle("古老的灯塔");
            SetDescription("这座古老的灯塔屹立在海边，为迷失的船只指引方向。");
            
            if (observationText != null)
                observationText.text = "站在灯塔下，你可以观察周围的环境...";
        }

        /// <summary>
        /// 观察周围环境
        /// </summary>
        private void ObserveSurroundings()
        {
            if (observationMessages.Length > 0)
            {
                string randomMessage = observationMessages[Random.Range(0, observationMessages.Length)];
                
                if (observationText != null)
                    observationText.text = randomMessage;
                
                Debug.Log($"LighthouseInteractionWindow: Observing - {randomMessage}");
            }
        }

        /// <summary>
        /// 在灯塔休息
        /// </summary>
        private void RestAtLighthouse()
        {
            Debug.Log("LighthouseInteractionWindow: Resting at lighthouse");
            
            // 应用休息效果
            if (GameManager.Instance != null)
            {
                var mindSystem = GameManager.Instance.GetSystem<BecomeSisyphus.Managers.Systems.SisyphusMindSystem>();
                if (mindSystem != null)
                {
                    float restAmount = 20f; // 固定恢复量
                    // mindSystem.RegenerateMentalStrength(restAmount, Time.time);
                    
                    if (observationText != null)
                        observationText.text = $"你在灯塔下休息了一会儿，恢复了 {restAmount} 点精神力量。";
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (observeButton != null)
                observeButton.onClick.RemoveAllListeners();
            
            if (restButton != null)
                restButton.onClick.RemoveAllListeners();
        }
    }
} 