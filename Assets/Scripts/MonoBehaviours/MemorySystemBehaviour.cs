using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Systems;

public class MemorySystemBehaviour : MonoBehaviour
{
    private MemorySystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<MemorySystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update();
    }
} 