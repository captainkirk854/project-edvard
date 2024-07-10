namespace Utility.Mvvm
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// </summary>
    /// <typeparam name="T">Parameter type.</typeparam>
    /// <remarks>
    /// The constructor deliberately prevents the use of value types.
    /// Because ICommand takes an object, having a value type for T would cause unexpected behavior when CanExecute(null) is called during XAML initialization for command bindings.
    /// </remarks>
    public class DelegateCommandGeneric<T> : DelegateCommandBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommandGeneric{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><see cref="CanExecute"/> will always return true.</remarks>
        public DelegateCommandGeneric(Action<T> executeMethod)
            : this(executeMethod, (o) => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommandGeneric{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <exception cref="ArgumentNullException">When both <paramref name="executeMethod"/> and <paramref name="canExecuteMethod"/> ar <see langword="null" />.</exception>
        public DelegateCommandGeneric(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base((o) => executeMethod((T)o), (o) => canExecuteMethod((T)o))
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException("executeMethod", "T for DelegateCommand<T> is not an object nor Nullable.");
            }

            TypeInfo genericTypeInfo = typeof(T).GetTypeInfo();

            // DelegateCommand allows object or Nullable<>.
            // note: Nullable<> is a struct so we cannot use a class constraint.
            if (genericTypeInfo.IsValueType)
            {
                if ((!genericTypeInfo.IsGenericType) || (!typeof(Nullable<>).GetTypeInfo().IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo())))
                {
                    throw new InvalidCastException("T for DelegateCommand<T> is not an object nor Nullable.");
                }
            }
        }

        private DelegateCommandGeneric(Func<T, Task> executeMethod)
            : this(executeMethod, (o) => true)
        {
        }

        private DelegateCommandGeneric(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
            : base((o) => executeMethod((T)o), (o) => canExecuteMethod((T)o))
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException("executeMethod", "Neither the executeMethod nor the canExecuteMethod delegates can be null.");
            }
        }

        /// <summary>
        /// Factory method to create a new instance of <see cref="DelegateCommandGeneric{T}"/> from an awaitable handler method.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.</param>
        /// <returns>Constructed instance of <see cref="DelegateCommandGeneric{T}"/></returns>
        public static DelegateCommandGeneric<T> FromAsyncHandler(Func<T, Task> executeMethod)
        {
            return new DelegateCommandGeneric<T>(executeMethod);
        }

        /// <summary>
        /// Factory method to create a new instance of <see cref="DelegateCommandGeneric{T}"/> from an awaitable handler method.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <returns>Constructed instance of <see cref="DelegateCommandGeneric{T}"/></returns>
        public static DelegateCommandGeneric<T> FromAsyncHandler(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
        {
            return new DelegateCommandGeneric<T>(executeMethod, canExecuteMethod);
        }

        /// <summary>
        /// Determines if the command can execute by invoked the <see cref="Func{T,Bool}"/> provided during construction.
        /// </summary>
        /// <param name="parameter">Data used by the command to determine if it can execute.</param>
        /// <returns>
        /// <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool CanExecute(T parameter)
        {
            return base.CanExecute(parameter);
        }

        /// <summary>
        /// Executes the command and invokes the <see cref="Action{T}"/> provided during construction.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        public virtual async Task Execute(T parameter)
        {
            await base.Execute(parameter);
        }
    }
}
