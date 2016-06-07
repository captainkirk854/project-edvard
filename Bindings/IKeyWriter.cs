namespace Binding
{
    using System.Data;

    public interface IKeyWriter
    {
        bool Update(DataTable consolidatedkeybindings);
    }
}
