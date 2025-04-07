namespace LD57.Interaction
{
    public class Indicator : SyncScript
    {
        public ModelComponent Ind { get; set; }

        IInteractable interactable;

        public override void Start()
        {
            interactable = Entity.BetterGet<IInteractable>();
        }

        public override void Update()
        {
            Ind.Enabled = interactable.Focused;
        }
    }
}
