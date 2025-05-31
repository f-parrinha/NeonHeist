namespace Core.Common.Interfaces
{
    public interface IEnableable 
    {
        bool IsEnabled { get; }

        void Enable();
        void Disable();
    }
}