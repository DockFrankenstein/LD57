using LD57.Interaction;

namespace LD57.Puzzle
{
    public class TheBigObjectReel : StartupScript, IInteractable
    {
        public TheBigObject Target { get; set; }
        public List<bool> StateAvaliability { get; set; } = new List<bool>();

        public bool Interactable { get; set; } = true;
        [DataMemberIgnore] public bool Focused { get; set; }

        public void Interact()
        {
            if (Target == null)
            {
                qDebug.LogError("Big Object not assigned to reel!");
                return;
            }

            if (Avaliable(Target.Index) && Avaliable(Target.Index + 1))
                Target?.ChangeState(1);
        }

        public void Interact2()
        {
            if (Target == null)
            {
                qDebug.LogError("Big Object not assigned to reel!");
                return;
            }

            if (Avaliable(Target.Index) && Avaliable(Target.Index - 1))
                Target.ChangeState(-1);
        }

        bool Avaliable(int index) =>
            index >= 0 && (StateAvaliability.Count <= index || StateAvaliability[index]);
    }
}
