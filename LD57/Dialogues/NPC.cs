using LD57.Camera;
using LD57.Interaction;
using LD57.LevelManagement;

namespace LD57.Dialogues
{
    public class NPC : StartupScript, IInteractable
    {
        public string DialogueName { get; set; }
        public string NextLevelName { get; set; }

        public bool Interactable => true;

        public List<CameraTarget> Cameras { get; set; } = new List<CameraTarget>();

        [DataMemberIgnore] public bool Focused { get; set; }

        DialogueManager manager;

        public override void Start()
        {
            manager = Services.GetService<DialogueManager>();
        }

        public void Interact()
        {
            manager.StartDialogue(DialogueName);

            manager.OnEndStory += Manager_OnEndStory;
            manager.OnProgressStory += Manager_OnProgressStory;
        }

        private void Manager_OnProgressStory()
        {
            if (manager.ActiveStory.variablesState["cam"] is string camTxt &&
                int.TryParse(camTxt, out int cam))
            {
                for (int i = 0; i < Cameras.Count; i++)
                    Cameras[i].TargetActive = i == cam;
            }
        }

        private void Manager_OnEndStory(Ink.Runtime.Story obj)
        {
            manager.OnEndStory -= Manager_OnEndStory;
            manager.OnProgressStory -= Manager_OnProgressStory;

            if (!string.IsNullOrWhiteSpace(NextLevelName))
                Services.GetService<LevelManager>().LoadLevel(NextLevelName);
        }

        public void Interact2() =>
            Interact();
    }
}
