using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Managers.Systems;

public class GameHistorySystemBehaviour : MonoBehaviour
{
    private GameHistorySystem system;

    void Awake()
    {
        system = GameManager.Instance.GetSystem<GameHistorySystem>();
    }

    void Update()
    {
        if (system != null)
            system.Update();
    }
} 