namespace Utility.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// </summary>
    public abstract class DelegateCommandBase : ICommand
    {
        private readonly Func<object, Task> _executeMethod;

        private readonly Func<object, bool> _canExecuteMethod;

        private List<WeakReference> _canExecuteChangedHandlers;

        /// <summary>
        /// Creates a new instance of a <see cref="DelegateCommandBase"/>, specifying both execute action and can execute function
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to execute when <see cref="ICommand.Execute"/> is invoked.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{Object,Bool}"/> to invoked when <see cref="ICommand.CanExecute"/> is invoked.</param>
        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException("executeMethod", "Neither the executeMethod nor the canExecuteMethod delegates can be null.");
            }

            this._executeMethod = (arg) =>
            {
                executeMethod(arg);
                return Task.Delay(0);
            };
            this._canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="DelegateCommandBase"/>, specifying both the Execute action as an awaitable Task and the CanExecute function.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Func{Object,Task}"/> to execute when <see cref="ICommand.Execute"/> is invoked.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{Object,Bool}"/> to invoked when <see cref="ICommand.CanExecute"/> is invoked.</param>
        protected DelegateCommandBase(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException("executeMethod", "Neither the executeMethod nor the canExecuteMethod delegates can be null.");
            }

            this._executeMethod = executeMethod;
            this._canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        /// <remarks>
        /// When subscribing to the <see cref="ICommand.CanExecuteChanged"/> event from code-behind (not when binding using XAML), 
        /// keep a hard reference to event handler to prevent garbage collection of the event handler. 
        /// 
        /// The command implements the Weak Event pattern so it does not have a hard reference to this handler.
        /// 
        /// In most scenarios, there is no reason to sign up to the CanExecuteChanged event directly, but if you do, you
        /// are responsible for maintaining the reference.
        /// 
        /// The following code holds a reference to the event handler. 
        /// The myEventHandlerReference value should be stored ain an instance member to avoid it from being garbage collected.
        /// 
        /// <code>
        ///   EventHandler myEventHandlerReference = new EventHandler(this.OnCanExecuteChanged);
        ///   command.CanExecuteChanged += myEventHandlerReference;
        /// </code>
        /// 
        /// </remarks>
        public virtual event EventHandler CanExecuteChanged
        {
            add
            {
                EventHandlerManager.AddWeakReferenceHandler(ref this._canExecuteChangedHandlers, value, 2);
            }

            remove
            {
                EventHandlerManager.RemoveWeakReferenceHandler(this._canExecuteChangedHandlers, value);
            }
        }

        /// <summary>
        /// Raises <see cref="DelegateCommandBase.CanExecuteChanged"/> on UI thread so every command invoker can requery to check if command can execute
        /// <remarks>
        /// This will trigger the execution of <see cref="DelegateCommandBase.CanExecute"/> once for each invoker.
        /// </remarks>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This isn't an event, it's intended to allow raising an event.")]
        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        async void ICommand.Execute(object parameter)
        {
            await this.Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter);
        }

        /// <summary>
        /// Raises <see cref="ICommand.CanExecuteChanged"/> on the UI thread so every command invoker can requery <see cref="ICommand.CanExecute"/>.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            EventHandlerManager.CallWeakReferenceHandlers(this, this._canExecuteChangedHandlers);
        }

        /// <summary>
        /// Executes the command with the provided parameter by invoking the <see cref="Action{Object}"/> supplied during construction.
        /// </summary>
        /// <param name="parameter"></param>
        protected async Task Execute(object parameter)
        {
            await this._executeMethod(parameter);
        }

        /// <summary>
        /// Determines if command can execute with provided parameter by invoking the <see cref="Func{Object,Bool}"/> supplied during construction.
        /// </summary>
        /// <param name="parameter">The parameter to use when determining if this command can execute.</param>
        /// <returns>Returns <see langword="true"/> if the command can execute.  <see langword="False"/> otherwise.</returns>
        protected bool CanExecute(object parameter)
        {
            return this._canExecuteMethod == null || this._canExecuteMethod(parameter);
        }
    }
}