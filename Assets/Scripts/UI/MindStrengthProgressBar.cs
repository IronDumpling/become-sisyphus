using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BecomeSisyphus;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.UI
{
    public class MindStrengthProgressBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image valueImage;
        [SerializeField] private TextMeshProUGUI titleText;
        
        [Header("Settings")]
        [SerializeField] private bool useManagerSystem = true; // true for SisyphusManager, false for SisyphusMindSystem
        [SerializeField] private bool showNumericValue = true;
        [SerializeField] private string titlePrefix = "Mind";
        
        [Header("Animation")]
        [SerializeField] private float animationSpeed = 2f;
        [SerializeField] private bool smoothTransition = true;
        
        [Header("Visual Effects")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color lowColor = Color.red;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private float lowThreshold = 0.3f;
        [SerializeField] private float criticalThreshold = 0.1f;
        
        // System references
        private SisyphusManager sisyphusManager;
        private SisyphusMindSystem sisyphusMindSystem;
        
        // Current values
        private float currentDisplayValue;
        private float targetValue;
        private float maxValue;
        
        private void Awake()
        {
            // Try to find UI components if not assigned
            if (valueImage == null)
            {
                valueImage = transform.Find("Bar/Value")?.GetComponent<Image>();
            }
            
            if (titleText == null)
            {
                titleText = transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            }
            
            // Validate components
            if (valueImage == null)
            {
                Debug.LogError("MindStrengthProgressBar: Value Image not found! Please assign it in the inspector or ensure the hierarchy has Bar/Value with Image component.");
            }
            
            if (titleText == null)
            {
                Debug.LogError("MindStrengthProgressBar: Title Text not found! Please assign it in the inspector or ensure the hierarchy has Title with TextMeshProUGUI component.");
            }
        }
        
        private void Start()
        {
            // Get system references
            if (GameManager.Instance != null)
            {
                sisyphusManager = GameManager.Instance.GetSystem<SisyphusManager>();
                sisyphusMindSystem = GameManager.Instance.GetSystem<SisyphusMindSystem>();
                
                // Subscribe to events
                SubscribeToEvents();
                
                // Initialize display
                UpdateDisplay();
            }
            else
            {
                Debug.LogError("MindStrengthProgressBar: GameManager.Instance is null!");
            }
        }
        
        private void SubscribeToEvents()
        {
            if (useManagerSystem && sisyphusManager != null)
            {
                sisyphusManager.OnMentalStrengthChanged += OnMentalStrengthChanged;
                maxValue = 100f; // SisyphusManager uses fixed max of 100
                
                // Initialize current values from the system
                targetValue = sisyphusManager.MentalStrength;
                currentDisplayValue = targetValue;
                
                Debug.Log($"MindStrengthProgressBar: Subscribed to SisyphusManager events. Initial value: {targetValue}/{maxValue}");
            }
            else if (!useManagerSystem && sisyphusMindSystem != null)
            {
                sisyphusMindSystem.OnMentalStrengthChanged += OnMentalStrengthChanged;
                maxValue = sisyphusMindSystem.MaxMentalStrength;
                
                // Initialize current values from the system
                targetValue = sisyphusMindSystem.MentalStrength;
                currentDisplayValue = targetValue;
                
                Debug.Log($"MindStrengthProgressBar: Subscribed to SisyphusMindSystem events. Initial value: {targetValue}/{maxValue}");
            }
            else
            {
                Debug.LogWarning($"MindStrengthProgressBar: Could not subscribe to events. UseManagerSystem={useManagerSystem}, SisyphusManager={sisyphusManager != null}, SisyphusMindSystem={sisyphusMindSystem != null}");
            }
        }
        
        private void UnsubscribeFromEvents()
        {
            if (sisyphusManager != null)
            {
                sisyphusManager.OnMentalStrengthChanged -= OnMentalStrengthChanged;
            }
            
            if (sisyphusMindSystem != null)
            {
                sisyphusMindSystem.OnMentalStrengthChanged -= OnMentalStrengthChanged;
            }
        }
        
        private void OnMentalStrengthChanged(float newValue)
        {
            targetValue = newValue;
            
            if (!smoothTransition)
            {
                currentDisplayValue = targetValue;
                UpdateDisplay();
            }
            
            Debug.Log($"MindStrengthProgressBar: Mental strength changed to {newValue}/{maxValue}");
        }
        
        private void Update()
        {
            if (smoothTransition && Mathf.Abs(currentDisplayValue - targetValue) > 0.01f)
            {
                // Smooth transition to target value
                currentDisplayValue = Mathf.Lerp(currentDisplayValue, targetValue, animationSpeed * Time.deltaTime);
                UpdateDisplay();
            }
        }
        
        private void UpdateDisplay()
        {
            if (valueImage == null) return;
            
            // Calculate fill amount (0-1)
            float fillAmount = maxValue > 0 ? currentDisplayValue / maxValue : 0f;
            fillAmount = Mathf.Clamp01(fillAmount);
            
            // Update progress bar fill
            valueImage.fillAmount = fillAmount;
            
            // Update color based on thresholds
            Color targetColor = GetColorForValue(fillAmount);
            valueImage.color = targetColor;
            
            // Update title text
            if (titleText != null)
            {
                if (showNumericValue)
                {
                    titleText.text = $"{titlePrefix}: {currentDisplayValue:F0}/{maxValue:F0}";
                }
                else
                {
                    titleText.text = titlePrefix;
                }
            }
        }
        
        private Color GetColorForValue(float normalizedValue)
        {
            if (normalizedValue <= criticalThreshold)
            {
                return criticalColor;
            }
            else if (normalizedValue <= lowThreshold)
            {
                // Lerp between low and critical color
                float t = (normalizedValue - criticalThreshold) / (lowThreshold - criticalThreshold);
                return Color.Lerp(criticalColor, lowColor, t);
            }
            else
            {
                // Lerp between normal and low color
                float t = (normalizedValue - lowThreshold) / (1f - lowThreshold);
                return Color.Lerp(lowColor, normalColor, t);
            }
        }
        
        // Public method to manually refresh the display
        public void RefreshDisplay()
        {
            if (useManagerSystem && sisyphusManager != null)
            {
                targetValue = sisyphusManager.MentalStrength;
                maxValue = 100f;
            }
            else if (!useManagerSystem && sisyphusMindSystem != null)
            {
                targetValue = sisyphusMindSystem.MentalStrength;
                maxValue = sisyphusMindSystem.MaxMentalStrength;
            }
            
            currentDisplayValue = targetValue;
            UpdateDisplay();
        }
        
        // Public method to switch between systems
        public void SwitchToManagerSystem(bool useManager)
        {
            UnsubscribeFromEvents();
            useManagerSystem = useManager;
            SubscribeToEvents();
            RefreshDisplay();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        // Debug method to test the progress bar
        [ContextMenu("Test Progress Bar")]
        private void TestProgressBar()
        {
            targetValue = Random.Range(0f, maxValue);
            Debug.Log($"MindStrengthProgressBar: Testing with value {targetValue}/{maxValue}");
        }
    }
} 