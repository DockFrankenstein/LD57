using LD57.Interaction;
using LD57.LevelManagement;
using LD57.UiSystem;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace LD57.Puzzle
{
    public class Hole : SyncScript, IInteractable
    {
        public UiCanvas Canvas { get; set; }
        public TheBigObject Target { get; set; }
        public string NextLevel { get; set; }
        public string EndText { get; set; }

        HoleWidget ui;

        public bool Interactable => Target != null &&
            (Target.Index == Target.States.Count - 1 || Target.States.Count == 0);
        [DataMemberIgnore] public bool Focused { get; set; }

        float t = 0f;

        public void Interact()
        {
            Canvas.UiEnabled = true;
            Target.Grab();
        }

        public void Interact2() =>
            Interact();

        public override void Start()
        {
            ui = new HoleWidget(Services.GetService<UiManager>());
            Canvas.Root = ui;

            try
            {
                var txt = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Data/items/{EndText}.txt"));
                if (txt.Length > 1)
                {
                    ui.youGot.Text = txt[0];
                    ui.description.Text = string.Join('\n', txt.Skip(1));
                }
            }
            catch (Exception e)
            {
                qDebug.LogError($"Failed to load '{EndText}.txt': {e}");
            }
        }

        public override void Update()
        {
            if (Canvas.UiEnabled && Canvas.HasInputFocus)
            {
                t += (float)Game.UpdateTime.WarpElapsed.TotalSeconds;

                ui.background.Color = new Color(0,0,0, (byte)Math.Clamp(255 * (t - 0.3f), 0, 200));
                ui.youGot.Opacity = Math.Clamp((t-0.6f)*3f, 0f, 1f);
                ui.description.Opacity = Math.Clamp((t-0.7f)*3f, 0f, 1f);

                if (Input.IsKeyPressed(Keys.E))
                    LoadNextLevel();
            }
        }

        public void LoadNextLevel()
        {
            Services.GetService<LevelManager>().LoadLevel(NextLevel);
        }

        public class HoleWidget : Grid
        {
            public SolidBrush background;
            public Label youGot;
            public Label description;

            public HoleWidget(UiManager manager)
            {
                background = new SolidBrush(new Color(0f,0f,0f,0f));
                Background = background;

                RowsProportions.Add(new Proportion(ProportionType.Part, 0.4f));
                RowsProportions.Add(new Proportion(ProportionType.Fill));
                RowsProportions.Add(new Proportion(ProportionType.Part, 0.25f));

                var cont = new Label()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "PRESS [E] TO CONTINUE",
                    Font = manager.Fonts.CaviarDreams.GetFont(30),
                };

                youGot = new Label()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "You got something!",
                    Font = manager.Fonts.CaviarDreamsBold.GetFont(48),
                    Opacity = 0f,
                };

                description = new Label()
                {
                    Wrap = true,
                    Font = manager.Fonts.CaviarDreams.GetFont(24),
                    Text = "Lorem ipsum\ndolor sit amet",
                    Margin = new Myra.Graphics2D.Thickness(300, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                    Opacity = 0f,
                };

                SetRow(youGot, 0);
                SetRow(description, 1);
                SetRow(cont, 2);

                Widgets.Add(youGot);
                Widgets.Add(description);
                Widgets.Add(cont);
            }
        }
    }
}
