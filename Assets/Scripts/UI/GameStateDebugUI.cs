using UnityEngine;
using UnityEngine.UI;
using BecomeSisyphus.Core.GameStateSystem;

namespace BecomeSisyphus.UI
{
    /// <summary>
    /// 游戏状态调试UI
    /// </summary>
    public class GameStateDebugUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text currentStateText;
        [SerializeField] private Button switchToOutsideButton;
        [SerializeField] private Button switchToInsideButton;
        [SerializeField] private Button returnToMenuButton;

        private void Start()
        {
            SetupButtons();
        }

        private void Update()
        {
            UpdateStateDisplay();
        }

        private void SetupButtons()
        {
            if (switchToOutsideButton != null)
            {
                switchToOutsideButton.onClick.AddListener(() => {
                    GameStateManager.Instance?.SwitchToState("InsideGame/OutsideWorld/Climbing");
                });
            }

            if (switchToInsideButton != null)
            {
                switchToInsideButton.onClick.AddListener(() => {
                    GameStateManager.Instance?.SwitchToState("InsideGame/InsideWorld/Sailing");
                });
            }

            if (returnToMenuButton != null)
            {
                returnToMenuButton.onClick.AddListener(() => {
                    GameStateManager.Instance?.SwitchToRootState("MainMenu");
                });
            }
        }

        private void UpdateStateDisplay()
        {
            if (currentStateText != null && GameStateManager.Instance != null)
            {
                var currentPath = GameStateManager.Instance.GetCurrentStatePath();
                currentStateText.text = $"Current State: {currentPath}";
            }
        }

        private void OnDestroy()
        {
            if (switchToOutsideButton != null) switchToOutsideButton.onClick.RemoveAllListeners();
            if (switchToInsideButton != null) switchToInsideButton.onClick.RemoveAllListeners();
            if (returnToMenuButton != null) returnToMenuButton.onClick.RemoveAllListeners();
        }
    }
} 