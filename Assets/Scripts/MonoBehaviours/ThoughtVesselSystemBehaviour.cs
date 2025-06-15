using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Systems;

public class ThoughtVesselSystemBehaviour : MonoBehaviour
{
    private ThoughtVesselSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<ThoughtVesselSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update(Time.deltaTime);
    }
} 