namespace Core.Utilities.DesignPatterns
{

    /// <summary>
    /// Class <c>BuilderBase</c> - Abstract class that defines a generic Builder class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericBuilder<T> where T : new()
    {

        protected T instance;

        public GenericBuilder()
        {
            instance = new T();
        }

        public virtual T Build()
        {
            return instance;
        }
    }
}