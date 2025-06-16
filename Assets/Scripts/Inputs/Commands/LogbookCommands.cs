using UnityEngine;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Inputs.Controllers;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Inputs.Commands
{
    // Logbook Interface Commands
    public class FlipLogbookPageCommand : ICommand
    {
        private LogbookUIController controller;
        private int direction; // 1 for next, -1 for previous

        public FlipLogbookPageCommand(LogbookUIController controller, int direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.FlipPage(direction);
            Debug.Log($"Logbook: Flipping page {(direction > 0 ? "forward" : "backward")}");
        }
    }

    public class MoveLogbookMapCommand : ICommand
    {
        private LogbookUIController controller;
        private Vector2 direction;

        public MoveLogbookMapCommand(LogbookUIController controller, Vector2 direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.MoveMap(direction);
            Debug.Log($"Logbook Map: Moving {direction}");
        }
    }

    public class SelectLogEntryCommand : ICommand
    {
        private LogbookUIController controller;
        private Vector2 direction;

        public SelectLogEntryCommand(LogbookUIController controller, Vector2 direction)
        {
            this.controller = controller;
            this.direction = direction;
        }

        public void Execute()
        {
            controller.SelectLogEntry(direction);
            Debug.Log($"Logbook: Selecting LogEntry {direction}");
        }
    }

    public class RecallMemoryCommand : ICommand
    {
        private LogbookUIController controller;
        private Signifier selectedSignifier; // Assume this is passed when recalling memory

        public RecallMemoryCommand(LogbookUIController controller, Signifier signifier)
        {
            this.controller = controller;
            this.selectedSignifier = signifier;
        }

        public void Execute()
        {
            controller.RecallMemory(selectedSignifier);
            Debug.Log($"Logbook: Recalling memory with {selectedSignifier.name}");
            // TODO: Implement logic to switch to recall interface, and handle signifier selection there.
        }
    }

    public class CloseLogbookCommand : ICommand
    {
        private LogbookUIController controller;

        public CloseLogbookCommand(LogbookUIController controller)
        {
            this.controller = controller;
        }

        public void Execute()
        {
            controller.CloseLogbook();
            // GameManager.Instance.ChangeState(GameManager.Instance.PreviousGameState); // Assuming GameManager tracks previous state
            Debug.Log("Logbook: Closing and returning to previous state");
        }
    }
} 