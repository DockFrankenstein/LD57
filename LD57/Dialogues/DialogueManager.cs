using Ink;
using LD57.UiSystem;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using qASIC.Console;
using qASIC.Text;

namespace LD57.Dialogues
{
    public class DialogueManager : AsyncScript
    {
        static qColor LogColor = new qColor(166, 110, 173);

        public UiCanvas canvas;

        DialogueWidget ui;

        public event Action<Ink.Runtime.Story> OnStartStory;
        public event Action<Ink.Runtime.Story> OnEndStory;
        public event Action OnProgressStory;

        public override async Task Execute()
        {
            this.RegisterInQ();
            await CompileInk();
            await LoadInk();

            Services.AddService(this);

            ui = new DialogueWidget(Services.GetService<UiManager>());
            canvas.Root = ui;

            while (Entity?.Scene != null)
            {
                if (ActiveStory != null && Input.IsKeyPressed(Keys.E) && canvas.HasInputFocus)
                {
                    if (ActiveStory.currentChoices.Count == 0 && !ActiveStory.canContinue)
                    {
                        EndDialogue();
                        continue;
                    }

                    if (ActiveStory.currentChoices.Count > 0)
                    {
                        ActiveStory.ChooseChoiceIndex(0);
                        ContinueDialogue();
                    }

                    if (ActiveStory.canContinue)
                    {
                        ContinueDialogue();
                    }
                }

                await Script.NextFrame();
            }
        }

        public override void Cancel()
        {
            this.UnregisterInQ();
        }

        public static async Task CompileInk()
        {
            var logColor = new qColor(178, 237, 18);

            qDebug.Log("[Dialogue Manager] Compiling ink scripts...", logColor);

            var inkPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ink");
            var compiledPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ink_compiled");

            Directory.CreateDirectory(inkPath);
            Directory.CreateDirectory(compiledPath);;

            foreach (var item in Directory.GetFiles(inkPath))
            {
                var targetFileName = $"{Path.GetFileNameWithoutExtension(item)}.json";
                var targetFilePath = Path.Combine(compiledPath, targetFileName);

#if !DEBUG
                if (File.Exists(targetFilePath)) continue;
#endif

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

            qDebug.Log("[Dialogue Manager] Ink compilation compleated!", logColor);
        }

        public Dictionary<string, Ink.Runtime.Story> LoadedStories { get; private set; } = new Dictionary<string, Ink.Runtime.Story>();

        [DataMemberIgnore] public string ActiveStoryName { get; set; } = "";
        [DataMemberIgnore] public Ink.Runtime.Story ActiveStory { get; set; } = null;

        [Command("inkstories", Description = "Shows a list of all loaded ink stories.")]
        private void Cmd_Stories(GameCommandContext context)
        {
            var tree = TextTree.Fancy;
            var root = new TextTreeItem("[Dialogue Manager] Loaded ink stories:");

            foreach (var item in LoadedStories)
                root.Add(item.Key);

            context.Logs.Log(tree.GenerateTree(root), LogColor);
        }

        public async Task LoadInk()
        {
            qDebug.Log("[Dialogue Manager] Loading ink scripts...", LogColor);
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
            
            qDebug.Log($"[Dialogue Manager] Loaded {LoadedStories.Count} ink stories.", LogColor);
        }

        [Command("playink", Description = "Plays an ink story by name.")]
        public void StartDialogue(string name)
        {
            if (!LoadedStories.ContainsKey(name))
            {
                qDebug.LogError($"Couldn't load ink story of name '{name}' - story doesn't exist!");
                return;
            }

            ActiveStoryName = name;
            ActiveStory = LoadedStories[name];

            ContinueDialogue();
            canvas.UiEnabled = true;

            OnStartStory?.Invoke(ActiveStory);

            qDebug.Log($"[Dialogue Manager] Started story '{name}'.", LogColor);
        }

        [Command("endink", Description = "Finishes an ink story.")]
        public void EndDialogue()
        {
            var story = ActiveStory;
            if (ActiveStory == null) return;
            ActiveStory.ResetState();
            ActiveStory = null;
            ActiveStoryName = "";

            canvas.UiEnabled = false;

            OnEndStory?.Invoke(story);

            qDebug.Log("[Dialogue Manager] Finished a story.", LogColor);
        }

        public void ContinueDialogue()
        {
            if (ActiveStory == null) return;
            if (!ActiveStory.canContinue) return;

            ui.txt.Text = ActiveStory.Continue().TrimEnd();
            
            var speaker = ActiveStory.variablesState["speaker"] as string;
            if (!string.IsNullOrWhiteSpace(speaker))
                ui.speaker.Text = speaker;

            OnProgressStory?.Invoke();
        }

        public class DialogueWidget : Grid
        {
            public Label speaker;
            public Label txt;

            public DialogueWidget(UiManager manager)
            {
                Padding = new Myra.Graphics2D.Thickness(50,50);

                var window = new Grid()
                {
                    Background = new SolidBrush(new Color(150,150,150)),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Myra.Graphics2D.Thickness(4,4),
                    RowSpacing = 4,
                    Width = 600,
                };

                var speakerBox = new Grid()
                {
                    Background = new SolidBrush(new Color(30, 30, 30)),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };

                speaker = new Label()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                    Font = manager.Fonts.CaviarDreamsBold.GetFont(32),
                };

                var txtBox = new Grid()
                {
                    Background = new SolidBrush(new Color(30, 30, 30)),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };

                txt = new Label()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                    Wrap = true,
                    Font = manager.Fonts.CaviarDreams.GetFont(24),
                };

                RowsProportions.Add(new Proportion(ProportionType.Fill));
                RowsProportions.Add(new Proportion(ProportionType.Pixels, 120));

                window.RowsProportions.Add(new Proportion(ProportionType.Pixels, 32));
                window.RowsProportions.Add(new Proportion(ProportionType.Fill));

                Grid.SetRow(window, 1);

                Grid.SetRow(speakerBox, 0);
                Grid.SetRow(txtBox, 1);

                Widgets.Add(window);

                window.Widgets.Add(speakerBox);
                window.Widgets.Add(txtBox);

                speakerBox.Widgets.Add(speaker);
                txtBox.Widgets.Add(txt);
            }
        }
    }
}
