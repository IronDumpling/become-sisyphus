using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Managers.Systems;

public class SisyphusManagerBehaviour : MonoBehaviour
{
    private SisyphusManager manager;

    void Awake()
    {
        manager = GameManager.Instance.GetSystem<SisyphusManager>();
    }

    void Update()
    {
        if (manager != null)
            manager.Update(Time.deltaTime);
    }
} 