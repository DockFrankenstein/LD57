namespace LD57.UiSystem
{
    public interface IUiCanvas
    {
        string UiName { get; }
        bool UiEnabled { get; set; }

        void DrawUi();
    }
}