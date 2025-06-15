using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Systems;

public class LogbookSystemBehaviour : MonoBehaviour
{
    private LogbookSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<LogbookSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update();
    }
} 