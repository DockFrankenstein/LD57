namespace LD57.Input
{
    public interface IInputFocusable
    {
        string InputFocusableName { get; }
        bool WantsInputFocus { get; }
        bool HasInputFocus { get; set; }
    }
}
