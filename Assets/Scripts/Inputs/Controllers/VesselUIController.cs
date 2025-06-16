using UnityEngine;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class VesselUIController : MonoBehaviour
    {
        public void OpenVesselUI()
        {
            Debug.Log("Opening Thought Vessel UI");
            // TODO: Implement UI display logic
        }

        public void CloseVesselUI()
        {
            Debug.Log("Closing Thought Vessel UI");
            // TODO: Implement UI hide logic
        }

        public void SelectGrid(Vector2Int direction)
        {
            Debug.Log($"Selecting grid at: {direction}");
            // TODO: Implement grid selection logic
        }

        public bool SelectCargo(VesselCargo cargo)
        {
            Debug.Log($"Selecting cargo: {cargo.name}");
            // TODO: Implement cargo selection logic
            return true; // Placeholder
        }

        public void MoveCargo(Vector2Int direction)
        {
            Debug.Log($"Moving selected cargo: {direction}");
            // TODO: Implement cargo movement logic
        }

        public void RotateCargo()
        {
            Debug.Log("Rotating selected cargo");
            // TODO: Implement cargo rotation logic
        }

        public void ExitCargoSelection()
        {
            Debug.Log("Exiting cargo selection");
            // TODO: Implement logic to deselect cargo
        }
    }
} 