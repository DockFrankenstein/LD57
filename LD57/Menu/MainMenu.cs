using LD57.LevelManagement;
using LD57.UiSystem;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace LD57.Menu
{
    public class MainMenu : StartupScript
    {
        public UiCanvas Canvas { get; set; }

        MenuWidget widget;

        public override void Start()
        {
            widget = new MenuWidget(Services.GetService<UiManager>(), Canvas.Desktop);
            widget.exit.Click += (_, _) => Environment.Exit(0);
            widget.LoadLevel += s =>
                Services.GetService<LevelManager>().LoadLevel(s);

            Canvas.Root = widget;
        }

        public class MenuWidget : Grid
        {
            public Button play;
            public Button settings;
            public Button controls;
            public Button exit;

            Window selectWindow;
            Window settingsWindow;

            public Action<string> LoadLevel;

            public MenuWidget(UiManager manager, Desktop desktop)
            {
                RowsProportions.Add(new Proportion(ProportionType.Part, 0.35f));
                RowsProportions.Add(new Proportion(ProportionType.Fill));
                RowsProportions.Add(new Proportion(ProportionType.Pixels, 100));

                var title = new Label()
                {
                    Text = "Where is Jeremy Wattson?",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Myra.Graphics2D.Thickness(30,30),
                    Font = manager.Fonts.CaviarDreamsBold.GetFont(72),
                };

                var buttons = new HorizontalStackPanel()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Spacing = 8,
                    Margin = new Myra.Graphics2D.Thickness(30,30)
                };

                play = CreateButton("Play");
                settings = CreateButton("Settings");
                controls = CreateButton("Controls");
                exit = CreateButton("Exit");

                selectWindow = new Window()
                {
                    Width = 500,
                    Height = 300,
                    Title = "Level Select",
                    TitleFont = manager.Fonts.CaviarDreams.GetFont(18),
                };

                var selectScroll = new ScrollViewer()
                {
                    ShowHorizontalScrollBar = false,
                };

                var selectStack = new VerticalStackPanel()
                {
                    Spacing = 4,
                };

                settingsWindow = new Window()
                {
                    Width = 400,
                    Height = 600,
                    Title = "Settings",
                    TitleFont = manager.Fonts.CaviarDreams.GetFont(18),
                };

                SetRow(title, 0);
                SetRow(buttons, 2);

                Widgets.Add(title);
                Widgets.Add(buttons);

                buttons.Widgets.Add(play);
                buttons.Widgets.Add(settings);
                buttons.Widgets.Add(controls);
                buttons.Widgets.Add(exit);

                selectWindow.Content = selectScroll;
                selectScroll.Content = selectStack;

                selectStack.Widgets.Add(CreateLevel("Level 0.5 - Hello", "lv0_5"));
                selectStack.Widgets.Add(CreateLevel("Level 1 - Introduction", "lv1"));
                selectStack.Widgets.Add(CreateLevel("Level 2 - Switches", "lv2"));
                selectStack.Widgets.Add(CreateLevel("Level 3 - Switches More", "lv3"));
                selectStack.Widgets.Add(CreateLevel("Level 4 - Limits", "lv4"));
                selectStack.Widgets.Add(CreateLevel("Level 5 - Hombre", "lv5"));
                selectStack.Widgets.Add(CreateLevel("End", "end"));

                play.Click += (_, _) =>
                {
                    switch (selectWindow.IsPlaced)
                    {
                        case true:
                            selectWindow.Close();
                            break;
                        case false:
                            selectWindow.Show(desktop);
                            selectWindow.CenterOnDesktop();
                            break;
                    }
                };

                settings.Click += (_, _) =>
                {
                    switch (settingsWindow.IsPlaced)
                    {
                        case true:
                            settingsWindow.Close();
                            break;
                        case false:
                            settingsWindow.Show(desktop);
                            settingsWindow.CenterOnDesktop();
                            break;
                    }
                };


                Button CreateLevel(string text, string levelName)
                {
                    var button = new Button()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Height = 30,
                        Background = new SolidBrush(new Color(100, 100, 100)),
                        BorderThickness = new Myra.Graphics2D.Thickness(0, 0),
                    };

                    var label = new Label()
                    {
                        Text = text,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Font = manager.Fonts.CaviarDreams.GetFont(16),
                    };

                    button.Content = label;
                    button.Click += (_, _) =>
                        LoadLevel?.Invoke(levelName);

                    return button;
                }

                Button CreateButton(string text)
                {
                    var button = new Button()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Height = 40,
                        Width = 100,
                        Background = new SolidBrush(new Color(10,10,10)),
                        Border = new SolidBrush(new Color(150,150,150)),
                        BorderThickness = new Myra.Graphics2D.Thickness(2,2),
                    };

                    var label = new Label()
                    {
                        Text = text,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Font = manager.Fonts.CaviarDreams.GetFont(18),
                    };

                    button.Content = label;
                    return button;
                }
            }
        }
    }
}
