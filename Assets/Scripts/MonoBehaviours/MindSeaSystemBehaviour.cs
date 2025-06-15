using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Systems;

public class MindSeaSystemBehaviour : MonoBehaviour
{
    private MindSeaSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<MindSeaSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update(Time.deltaTime);
    }
} 