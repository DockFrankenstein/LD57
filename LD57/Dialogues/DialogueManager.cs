using Ink;
using qASIC.Console;
using qASIC.Text;

namespace LD57.Dialogues
{
    public class DialogueManager : AsyncScript
    {
        public override async Task Execute()
        {
            this.RegisterInQ();
            await CompileInk();
            await LoadInk();
        }

        public override void Cancel()
        {
            this.UnregisterInQ();
        }

        public static async Task CompileInk()
        {
            var inkPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ink");
            var compiledPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ink_compiled");

            Directory.CreateDirectory(inkPath);
            Directory.CreateDirectory(compiledPath);;

            foreach (var item in Directory.GetFiles(inkPath))
            {
                var targetFileName = $"{Path.GetFileNameWithoutExtension(item)}.json";
                var targetFilePath = Path.Combine(compiledPath, targetFileName);
                if (File.Exists(targetFilePath)) continue;
                
                try
                {

                    var txt = await File.ReadAllTextAsync(item);
                    var compiler = new Compiler(txt);
                    var story = compiler.Compile();

                    await File.WriteAllTextAsync(targetFilePath, story.ToJson());
                }
                catch (Exception e)
                {
                    qDebug.LogError($"There was an error while compiling ink file at '{item}': {e}");
                }
            }
        }

        public Dictionary<string, Ink.Runtime.Story> LoadedStories { get; private set; } = new Dictionary<string, Ink.Runtime.Story>();

        [Command("inkstories", Description = "Shows a list of all loaded ink stories.")]
        private void Cmd_Stories(GameCommandContext context)
        {
            var tree = TextTree.Fancy;
            var root = new TextTreeItem("Loaded ink stories:");

            foreach (var item in LoadedStories)
                root.Add(item.Key);

            context.Logs.Log(tree.GenerateTree(root));
        }

        public async Task LoadInk()
        {
            LoadedStories.Clear();

            var inkPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ink_compiled");
            foreach (var item in Directory.GetFiles(inkPath))
            {
                try
                {
                    var name = Path.GetFileNameWithoutExtension(item);
                    var txt = await File.ReadAllTextAsync(item);
                    var story = new Ink.Runtime.Story(txt);

                    if (LoadedStories.ContainsKey(name))
                    {
                        LoadedStories[name] = story;
                        continue;
                    }

                    LoadedStories.Add(name, story);
                }
                catch (Exception e)
                {
                    qDebug.LogError($"There was an error while loading ink file at '{item}': {e}");
                }
            }
        }

        public void StartDialogue(string name)
        {
            if (!LoadedStories.ContainsKey(name))
            {
                qDebug.LogError($"Couldn't load ink story of name '{name}' - story doesn't exist!");
                return;
            }
        }
    }
}
