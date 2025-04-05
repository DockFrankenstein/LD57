using LD57.Interaction;

namespace LD57.Puzzle
{
    public class TheBigObjectReel : StartupScript, IInteractable
    {
        public TheBigObject Target { get; set; }

        public bool Interactable { get; set; } = true;
        [DataMemberIgnore] public bool Focused { get; set; }

        public void Interact()
        {
            Target.ChangeState(1);
        }

        public void Interact2()
        {
            Target.ChangeState(-1);
        }
    }
}
