using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Managers.Behaviours
{
    public class ThoughtBoatSystemBehaviour : MonoBehaviour
    {
        private ThoughtBoatSystem system;

        void Start()
        {
            if (GameManager.Instance != null)
            {
                system = GameManager.Instance.GetSystem<ThoughtBoatSystem>();
                if (system != null)
                {
                    Debug.Log("ThoughtBoatSystemBehaviour: Successfully got ThoughtBoatSystem reference");
                }
                else
                {
                    Debug.LogError("ThoughtBoatSystemBehaviour: Failed to get ThoughtBoatSystem reference");
                }
            }
            else
            {
                Debug.LogError("ThoughtBoatSystemBehaviour: GameManager.Instance is null");
            }
        }

        void Update()
        {
            if (system != null)
            {
                system.Update();
            }
        }
    }
} 