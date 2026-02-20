namespace Game.Commander
{
    public interface ICommandInvoker
    {
        void Enqueue(ICommand command);
        void ExecuteAll();
        void UndoLast();
        void ClearHistory();
    }
}
