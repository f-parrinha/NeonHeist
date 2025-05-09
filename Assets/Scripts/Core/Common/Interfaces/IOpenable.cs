namespace Core.Common.Interfaces
{
    public interface IOpenable
    {
        bool IsOpened { get; }

        void Open();
        void Close();
        bool Toggle();
    }
}