namespace BecomeSisyphus.Core.Interfaces
{
    public interface ICommand
    {
        void Execute();
        // Optional: void Undo();
    }
} 