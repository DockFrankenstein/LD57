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
            if (Target == null)
            {
                qDebug.LogError("Big Object not assigned to reel!");
                return;
            }

            Target?.ChangeState(1);
        }

        public void Interact2()
        {
            if (Target == null)
            {
                qDebug.LogError("Big Object not assigned to reel!");
                return;
            }

            Target.ChangeState(-1);
        }
    }
}
