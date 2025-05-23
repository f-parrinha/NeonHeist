namespace Core.Interactions
{
    public interface IInteractionerHolder
    {
        MultiInteractable Interactioner { get; }

        void SetInteractioner(MultiInteractable interactioner);
    }

}
