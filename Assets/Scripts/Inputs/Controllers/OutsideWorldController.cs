using UnityEngine;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class OutsideWorldController : MonoBehaviour
    {
        public void SelectSignifier(Vector2 direction)
        {
            Debug.Log($"Selecting signifier in Outside World: {direction}");
            // TODO: Implement signifier selection logic
        }

        public void UsePerceptionSkill()
        {
            Debug.Log("Using perception skill in Outside World.");
            // TODO: Implement perception skill logic
        }
    }
} 