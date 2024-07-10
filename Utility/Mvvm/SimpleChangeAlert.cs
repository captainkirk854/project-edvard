namespace Utility.Mvvm
{
    using System;

    public class SimpleChangeAlert
    {
        public event EventHandler Changed;

        public void SetToChanged()
        {
            this.OnChanged(EventArgs.Empty);
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (this.Changed != null)
            {
                this.Changed(this, e);
            }
        }
    }
}