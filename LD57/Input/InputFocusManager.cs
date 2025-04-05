namespace LD57.Input
{
    public class InputFocusManager : AsyncScript
    {
        public PriorityList<IInputFocusable> Focusables { get; set; }

        public override async Task Execute()
        {
            Focusables = new PriorityList<IInputFocusable>(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/input_priority.txt"),
                a => a.WantsInputFocus,
                a => a.InputFocusableName);

            await Focusables.LoadPriorityAsync();

            Game.Services.AddService(this);

            while (Game.IsRunning)
            {
                if (Focusables.IsChanged())
                {
                    if (Focusables.Selected != null)
                        Focusables.Selected.HasInputFocus = false;

                    Focusables.UpdateSelected();

                    if (Focusables.Selected != null)
                        Focusables.Selected.HasInputFocus = true;
                }

                await Script.NextFrame();
            }
        }

        public override void Cancel()
        {
            Game.Services.RemoveService(this);
        }
    }
}
