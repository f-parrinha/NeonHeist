namespace Core.Queue.Interfaces
{
    public interface IQueue<T>
    {
        T Evaluate();
        void Set(object setter, T value);
        void Unset(object setter);
        bool Contains(object setter);
    }
}