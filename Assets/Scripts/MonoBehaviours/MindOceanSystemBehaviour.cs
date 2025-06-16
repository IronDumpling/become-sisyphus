using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Systems;

public class MindOceanSystemBehaviour : MonoBehaviour
{
    private MindOceanSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<MindOceanSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update();
    }
} 