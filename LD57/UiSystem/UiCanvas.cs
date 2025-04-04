using Myra.Graphics2D.UI;

namespace LD57.UiSystem
{
    public class UiCanvas : StartupScript, IUiCanvas
    {
        [DataMemberIgnore] public Desktop Desktop { get; private set; }

        public bool EnabledByDefault { get; set; }

        public string UiName { get; set; }
        [DataMemberIgnore] public bool UiEnabled { get; set; }

        private Widget _root;
        [DataMemberIgnore]
        public Widget Root
        {
            get => _root;
            set
            {
                _root = value;
                if (Desktop != null)
                    Desktop.Root = value;
            }
        }

        public override void Start()
        {
            Desktop = new Desktop()
            {
                Root = _root,
            };

            Services.GetService<UiManager>().RegisterCanvas(this);

            if (EnabledByDefault)
                UiEnabled = true;
        }

        public override void Cancel()
        {
            Services.GetService<UiManager>().UnregisterCanvas(this);
        }

        public void DrawUi()
        {
            if (UiEnabled)
                Desktop.Render();
        }
    }
}
