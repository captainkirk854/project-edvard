namespace Utility.Mvvm
{
    using System;

    public class SimpleChangeAlert
    {
        public event EventHandler Changed;

        public void SetToChanged()
        {
            OnChanged(EventArgs.Empty);
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }
    }
}