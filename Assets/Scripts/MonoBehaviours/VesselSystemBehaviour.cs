using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Systems;

public class VesselSystemBehaviour : MonoBehaviour
{
    private VesselSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<VesselSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update();
    }
} 