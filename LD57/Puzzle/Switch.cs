using LD57.Interaction;

namespace LD57.Puzzle
{
    public class Switch : StartupScript, IInteractable, IActivatable
    {
        public bool State { get; set; }

        public bool CanEnable { get; set; }
        public bool CanDisable { get; set; }

        public event Action OnActivate;

        [DataMemberIgnore]
        public bool Interactable =>
            (State && CanDisable) || (!State && CanEnable);

        [DataMemberIgnore] public bool Focused { get; set; }

        public void Interact()
        {
            State = !State;
            OnActivate?.Invoke();
        }
    }
}
