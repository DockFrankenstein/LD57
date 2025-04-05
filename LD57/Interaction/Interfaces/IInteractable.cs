namespace LD57.Interaction
{
    public interface IInteractable
    {
        bool Interactable { get; }

        bool Focused { get; set; }

        void Interact();
    }
}