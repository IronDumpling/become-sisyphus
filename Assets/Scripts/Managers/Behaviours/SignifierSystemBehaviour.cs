using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Managers.Systems;

public class SignifierSystemBehaviour : MonoBehaviour
{
    private SignifierSystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<SignifierSystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update();
    }
} 