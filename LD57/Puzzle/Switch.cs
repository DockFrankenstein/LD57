using LD57.Interaction;

namespace LD57.Puzzle
{
    public class Switch : StartupScript, IInteractable
    {
        public bool State { get; set; }

        public bool CanEnable { get; set; }
        public bool CanDisable { get; set; }

        [DataMemberIgnore]
        public bool Interactable =>
            (State && CanDisable) || (!State && CanEnable);

        [DataMemberIgnore] public bool Focused { get; set; }

        public void Interact()
        {
            State = !State;
        }
    }
}
