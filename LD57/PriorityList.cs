using System.Collections;

namespace LD57
{
    [DataContractIgnore]
    public class PriorityList<T> : IEnumerable<T> where T : class
    {
        public PriorityList(string path, Func<T, bool> isEnabled, Func<T, string> getId)
        {
            Path = path;
            IsEnabled = isEnabled;
            GetId = getId;
        }

        public List<string> Priority { get; set; } = new List<string>();
        List<Item> Elements { get; set; } = new List<Item>();

        public string Path { get; set; }

        public event Func<T, bool> IsEnabled;
        public event Func<T, string> GetId;

        public event Action<T> OnElementRegistered;
        public event Action<T> OnElementUnregistered;

        public void LoadPriority()
        {
            Task.WaitAny(Task.Run(LoadPriorityAsync));
        }

        public async Task LoadPriorityAsync()
        {
            var fileName = System.IO.Path.GetFileName(Path);
            try
            {
                qDebug.Log($"Loading priority list '{fileName}'");
                if (!File.Exists(Path)) return;

                var txt = await File.ReadAllTextAsync(Path);

                Priority.Clear();
                Priority.AddRange(txt.ReplaceLineEndings().Split(Environment.NewLine)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Where(x => !x.StartsWith('#')));

                qDebug.Log($"Priority list '{fileName}' loaded!");
            }
            catch (Exception e)
            {
                qDebug.LogError($"Failed to load priority list '{fileName}': {e}");
            }
        }

        public void Register(T element)
        {
            var id = GetId(element);

            var ownPrio = Priority.IndexOf(id);
            int i = 0;
            for (; i < Elements.Count; i++)
            {
                var prio = Priority.IndexOf(Elements[i].id);
                if (prio >= ownPrio) break;
            }

            Elements.Insert(i, new Item()
            {
                element = element,
                id = id,
            });

            OnElementRegistered?.Invoke(element);
        }

        public void Unregister(T element)
        {
            var targets = Elements.Where(x => x.element == element)
                .ToArray();

            foreach (var item in targets)
                Elements.Remove(item);

            OnElementUnregistered?.Invoke(element);
        }

        public T Selected { get; set; }

        private T GetSelectedSilent()
        {
            int i = 0;
            while (i < Elements.Count && !IsEnabled(Elements[i].element))
                i++;

            if (i < Elements.Count)
                return Elements[i].element;

            return null;
        }

        public T GetSelected()
        {
            UpdateSelected();
            return Selected;
        }

        public void UpdateSelected()
        {
            Selected = GetSelectedSilent();
        }

        public bool IsChanged() =>
            GetSelectedSilent() != Selected;

        public IEnumerator<T> GetEnumerator() =>
            Elements.Select(x => x.element)
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        class Item
        {
            public T element;
            public string id;
        }
    }
}
