namespace Core.Hacking.Interfaces
{
    public interface IHackeable
    {
        IHackSystem HackSystem { get; }
        bool IsHacked { get; }

        void UponHack(bool success);
    }
}