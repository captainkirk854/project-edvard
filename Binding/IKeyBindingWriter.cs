namespace Binding
{
    using System.Data;

    public interface IKeyBindingWriter
    {
        bool Update(DataTable consolidatedkeybindings, bool updateChangeTag);
    }
}
