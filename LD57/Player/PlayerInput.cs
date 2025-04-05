using LD57.Input;

namespace LD57.Player
{
    public class PlayerInput : StartupScript, IInputFocusable
    {
        public string InputFocusableName => "player";
        public bool WantsInputFocus => true;
        [DataMemberIgnore]
        public bool HasInputFocus { get; set; }

        public override void Start()
        {
            this.RegisterInInputFocus();
        }

        public override void Cancel()
        {
            this.UnregisterInInputFocus();
        }
    }
}