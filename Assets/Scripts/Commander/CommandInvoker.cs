

using System.Collections.Generic;

namespace Game.Commander
{

    public interface ICommandInvoker
    {
        void Enqueue(ICommand command);
        void ExecuteAll();
        void UndoLast();
        void ClearHistory();
    }

    public class CommandInvoker : ICommandInvoker
    {
        private readonly Queue<ICommand> queue = new();
        private readonly Stack<ICommand> history = new();

        public void Enqueue(ICommand command)
        {
            queue.Enqueue(command);
        }

        public void ExecuteAll()
        {
            while (queue.Count > 0)
            {
                var command = queue.Dequeue();
                command.Execute();
                history.Push(command);
            }
        }

        public void UndoLast()
        {
            if (history.Count == 0)
                return;

            var command = history.Pop();
            command.Undo();
        }

        public void ClearHistory() => history.Clear();
    }
}
