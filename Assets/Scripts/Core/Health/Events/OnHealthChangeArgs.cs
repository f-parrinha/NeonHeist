namespace Core.Health.Events
{
    public class OnHealthChangeArgs
    {
        public float Change { get; set; }
        public float NewHealth { get; set; }
        public float OldHealth { get; set; }
    }
}