using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Managers.Systems;

public class ExplorationSystemBehaviour : MonoBehaviour
{
    private ExplorationSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<ExplorationSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update();
    }
} 