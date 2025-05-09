namespace Core.Common.Interfaces
{ 
    public interface IInitializable
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}