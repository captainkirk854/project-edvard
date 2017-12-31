namespace GameKey.Binding.Readers
{
    using System.Data;

    public interface IKeyBindingReader
    {
        DataTable GetBindableCommands();

        DataTable GetBoundCommands();
    }
}