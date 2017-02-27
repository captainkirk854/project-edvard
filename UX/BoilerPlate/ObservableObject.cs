namespace UX.BoilerPlate
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices; // For [CallerMemberName] attribute ...

    /// <summary>
    /// Common events and methods for observable ViewModel data
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises property changed event.
        /// </summary>
        /// <remarks>
        ///  > The [CallerMemberName] attribution instructs the compiler to automatically use the correct Property Name.
        /// </remarks>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
