using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Systems;

public class SisyphusMindSystemBehaviour : MonoBehaviour
{
    private SisyphusMindSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<SisyphusMindSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update(Time.deltaTime, Time.time);
    }
} 