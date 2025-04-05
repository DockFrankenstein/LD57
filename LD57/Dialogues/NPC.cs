using LD57.Interaction;

namespace LD57.Dialogues
{
    public class NPC : StartupScript, IInteractable
    {
        public string DialogueName { get; set; }

        public bool Interactable => true;

        [DataMemberIgnore] public bool Focused { get; set; }

        DialogueManager manager;

        public override void Start()
        {
            manager = Services.GetService<DialogueManager>();
        }

        public void Interact()
        {
            manager.StartDialogue(DialogueName);
        }

        public void Interact2() =>
            Interact();
    }
}
