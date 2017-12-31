namespace Utility.Mvvm
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// An <see cref="ICommand"/> whose delegates do not take any parameters for <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// </summary>
    /// <see cref="DelegateCommandBase"/>
    /// <see cref="DelegateCommandGeneric{T}"/>
    public class DelegateCommand : DelegateCommandBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> with <see cref="Action"/> to invoke on execution
        /// </summary>
        /// <param name="executeMethod"><see cref="Action"/> to invoke when <see cref="ICommand.Execute"/> is called</param>
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="DelegateCommand"/> with <see cref="Action"/> to invoke on execution and a <see langword="Func" /> to query for determining if the command can execute
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{TResult}"/> to invoke when <see cref="ICommand.CanExecute"/> is called</param>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base((o) => executeMethod(), (o) => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException("executeMethod", "Neither the executeMethod nor the canExecuteMethod delegates can be null.");
            }
        }

        private DelegateCommand(Func<Task> executeMethod)
            : this(executeMethod, () => true)
        {
        }

        private DelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base((o) => executeMethod(), (o) => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException("executeMethod", "Neither the executeMethod nor the canExecuteMethod delegates can be null.");
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> from an awaitable handler method
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on command.</param>
        /// <returns>Constructed instance of <see cref="DelegateCommand"/></returns>
        public static DelegateCommand FromAsyncHandler(Func<Task> executeMethod)
        {
            return new DelegateCommand(executeMethod);
        }

        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> from an awaitable handler method.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <returns>Constructed instance of <see cref="DelegateCommand"/></returns>
        public static DelegateCommand FromAsyncHandler(Func<Task> executeMethod, Func<bool> canExecuteMethod)
        {
            return new DelegateCommand(executeMethod, canExecuteMethod);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public virtual async Task Execute()
        {
            await Execute(null);
        }

        /// <summary>
        /// Determines if command can be executed
        /// </summary>
        /// <returns>Returns <see langword="true"/> if command can execute, otherwise returns <see langword="false"/>.</returns>
        public virtual bool CanExecute()
        {
            return CanExecute(null);
        }
    }
}
