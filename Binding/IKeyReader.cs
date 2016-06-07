namespace Binding
{
    using System.Data;

    public interface IKeyReader
    {
        DataTable GetBindableCommands();

        DataTable GetBoundCommands();
    }
}