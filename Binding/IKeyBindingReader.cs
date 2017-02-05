namespace Binding
{
    using System.Data;

    public interface IKeyBindingReader
    {
        DataTable GetBindableCommands();

        DataTable GetBoundCommands();
    }
}