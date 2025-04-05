namespace LD57.Input
{
    public class InputFocusManager : AsyncScript
    {
        static qColor LogColor { get; } = new qColor(226, 45, 90);

        public List<string> PriorityList { get; private set; } = new List<string>();

        public List<IInputFocusable> Focusables { get; private set; } = new List<IInputFocusable>();
        IInputFocusable selected;

        public override async Task Execute()
        {
            await LoadPriority();

            Game.Services.AddService(this);

            while (Game.IsRunning)
            {
                IInputFocusable newSelected = null;
                foreach (var item in Focusables)
                {
                    if (item.WantsInputFocus)
                    {
                        newSelected = item;
                        break;
                    }
                }

                if (newSelected != selected)
                {
                    if (selected != null)
                        selected.HasInputFocus = false;

                    selected = newSelected;

                    if (selected != null)
                        selected.HasInputFocus = true;
                }

                await Script.NextFrame();
            }
        }

        public override void Cancel()
        {
            Game.Services.RemoveService(this);
        }

        public void RegisterFocusable(IInputFocusable focusable)
        {
            var ownPrio = PriorityList.IndexOf(focusable.InputFocusableName);
            int i = 0;
            for (; i < Focusables.Count; i++)
            {
                var prio = PriorityList.IndexOf(Focusables[i].InputFocusableName);
                if (prio >= ownPrio) break;
            }

            Focusables.Insert(i, focusable);
        }
        
        public void UnregisterFocusable(IInputFocusable focusable)
        {
            Focusables.Remove(focusable);
            if (selected == focusable)
                selected = null;
        }

        private async Task LoadPriority()
        {
            qDebug.Log("[Input Focus] Loading priority list...", LogColor);

            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/input_priority.txt");
                if (!File.Exists(path)) return;

                var txt = await File.ReadAllTextAsync(path);

                PriorityList.Clear();
                PriorityList.AddRange(txt.ReplaceLineEndings().Split(Environment.NewLine)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Where(x => !x.StartsWith('#')));

                qDebug.Log("[Input Focus] Priority list loaded!", LogColor);
            }
            catch (Exception e)
            {
                qDebug.LogError($"There was an error while loading input priority list: {e}");
            }
        }
    }
}
