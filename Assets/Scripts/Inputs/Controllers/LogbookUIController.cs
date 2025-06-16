using UnityEngine;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Inputs.Controllers
{
    public class LogbookUIController : MonoBehaviour
    {
        public void OpenLogbook(string page = null)
        {
            Debug.Log($"Opening Logbook to page: {page}");
            // TODO: Implement UI display logic
        }

        public void FlipPage(int direction)
        {
            Debug.Log($"Flipping Logbook page {direction}");
            // TODO: Implement page flipping logic
        }

        public void MoveMap(Vector2 direction)
        {
            Debug.Log($"Moving Logbook map: {direction}");
            // TODO: Implement map movement logic
        }

        public void SelectLogEntry(Vector2 direction)
        {
            Debug.Log($"Selecting LogEntry: {direction}");
            // TODO: Implement log entry selection logic
        }

        public void RecallMemory(Signifier signifier)
        {
            Debug.Log($"Recalling memory for signifier: {signifier.name}");
            // TODO: Implement memory recall UI/logic
        }

        public void CloseLogbook()
        {
            Debug.Log("Closing Logbook");
            // TODO: Implement UI hide logic
        }
    }
} 