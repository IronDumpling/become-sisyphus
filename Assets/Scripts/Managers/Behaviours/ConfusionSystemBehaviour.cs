using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Managers.Systems;

public class ConfusionSystemBehaviour : MonoBehaviour
{
    private ConfusionSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<ConfusionSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update(Time.deltaTime, Time.time);
    }
} 