namespace GameKey.Binding.Writers
{
    using System.Data;

    public interface IKeyBindingWriter
    {
        bool Update(DataTable consolidatedkeybindings, bool updateChangeTag);
    }
}
