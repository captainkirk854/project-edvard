namespace Bindings
{
    using System.Data;

    public interface IKeyBindingUpdater
    {
        bool Write(DataTable consolidatedkeybindings);
    }
}
