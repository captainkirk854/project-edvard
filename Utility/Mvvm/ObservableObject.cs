namespace Utility.Mvvm
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets property and notifies listeners when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter</param>
        /// <param name="value">Desired value for property</param>
        /// <param name="propertyName">Name of property used to notify listeners. 
        /// This value is optional and can be provided automatically when invoked from compilers that support CallerMemberName.</param>
        /// <returns>True if value was changed, false if the existing value matched desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Notifies listeners that property value has changed
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners
        protected void OnPropertyChanged(string propertyName)
        {
            /*
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
             */

            this.OnPropertyChangedAuto(propertyName);
        }

        /// <summary>
        /// Notifies listeners that property value has changed
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners
        /// This value is optional and can be provided automatically when invoked from compilers that support CallerMemberName.</param>
        /// [CallerMemberName] attribution automatically instructs compiler of correct Property Name
        protected void OnPropertyChangedAuto([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="propertyExpression">Lambda expression representing property that has a new value</param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = PropertyHelper.ExtractPropertyName(propertyExpression);
            OnPropertyChanged(propertyName);
        }
    }
}
