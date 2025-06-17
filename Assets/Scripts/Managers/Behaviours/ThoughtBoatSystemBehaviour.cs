using UnityEngine;
using BecomeSisyphus;
using BecomeSisyphus.Managers.Systems;

namespace BecomeSisyphus.Managers.Behaviours
{
    public class ThoughtBoatSystemBehaviour : MonoBehaviour
    {
        private ThoughtBoatSystem system;

        void Awake()
        {
            system = GameManager.Instance.GetSystem<ThoughtBoatSystem>();
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