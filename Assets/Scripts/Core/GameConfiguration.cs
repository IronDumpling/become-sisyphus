using UnityEngine;
using Unity.Cinemachine;
using BecomeSisyphus.Core.Data;

[CreateAssetMenu(menuName = "Game/GameConfiguration")]
public class GameConfiguration : ScriptableObject
{
    [Header("Sisyphus Mind System")]
    public float maxMentalStrength = 100f;
    public float mentalStrengthRegenRate = 1f;
    public float mentalStrengthRegenDelay = 5f;

    [Header("Sisyphus Manager")]
    public float managerMentalStrength = 100f;
    public int managerMaxBrainCapacity = 100;
    public float managerMentalStrengthRegenRate = 1f;

    [Header("Confusion System")]
    public float confusionGenerationInterval = 30f;
    public float temporaryConfusionDuration = 60f;

    [Header("Thought Vessel System")]
    public int initialRows = 3;
    public int initialColumns = 3;
    public float loadRatioThreshold = 0.5f;
    public float mentalStrengthConsumptionRate = 2f;

    [Header("Time System")]
    public float timeScale = 1f;
    public float dayLength = 24f;

    [Header("Camera System")]
    public float cameraTransitionDuration = 1.0f;
    public PrioritySettings cameraDefaultPriority = new PrioritySettings { Value = 10 };
    public PrioritySettings cameraActivePriority = new PrioritySettings { Value = 20 };

    // Add more configuration fields as needed for other systems
} 