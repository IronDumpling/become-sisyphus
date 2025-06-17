using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;

namespace BecomeSisyphus.Managers.Systems
{
    public class ThoughtBoatSystem : ISystem
    {
        private ThoughtBoatSailingController activeBoat;
        private Vector2 lastKnownPosition;
        private float lastKnownRotation;
        
        public ThoughtBoatSailingController ActiveBoat => activeBoat;
        
        public void Initialize() 
        {
            Debug.Log("ThoughtBoatSystem: Initialized");
        }

        public void RegisterActiveBoat(ThoughtBoatSailingController boat)
        {
            activeBoat = boat;
            Debug.Log($"ThoughtBoatSystem: Registered active boat: {boat?.name}");
            
            if (activeBoat != null && lastKnownPosition != Vector2.zero)
            {
                // Restore last known position if exists
                activeBoat.transform.position = new Vector3(lastKnownPosition.x, activeBoat.transform.position.y, lastKnownPosition.y);
                activeBoat.transform.rotation = Quaternion.Euler(0, lastKnownRotation, 0);
                Debug.Log($"ThoughtBoatSystem: Restored boat position to {lastKnownPosition} and rotation to {lastKnownRotation}");
            }
        }

        public void Update()
        {
            if (activeBoat != null)
            {
                // Store current position and rotation for persistence
                lastKnownPosition = new Vector2(activeBoat.transform.position.x, activeBoat.transform.position.z);
                lastKnownRotation = activeBoat.transform.rotation.eulerAngles.y;
            }
        }

        public void Cleanup()
        {
            Debug.Log("ThoughtBoatSystem: Cleanup");
            activeBoat = null;
        }
    }
} 