using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Systems;

public class TimeSystemBehaviour : MonoBehaviour
{
    private TimeSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<TimeSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update();
    }
} 