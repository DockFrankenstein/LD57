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
            widget = new MenuWidget(Canvas.Desktop);
            widget.play.Click += (_, _) => { };
            widget.exit.Click += (_, _) => Environment.Exit(0);

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

            public MenuWidget(Desktop desktop)
            {
                RowsProportions.Add(new Proportion(ProportionType.Part, 0.35f));
                RowsProportions.Add(new Proportion(ProportionType.Fill));
                RowsProportions.Add(new Proportion(ProportionType.Pixels, 100));

                var title = new Label()
                {
                    Text = "Where is Jeremy Wattson?",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Myra.Graphics2D.Thickness(30,30)
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
                };

                settingsWindow = new Window()
                {
                    Width = 400,
                    Height = 600,
                    Title = "Settings",
                };

                SetRow(title, 0);
                SetRow(buttons, 2);

                Widgets.Add(title);
                Widgets.Add(buttons);

                buttons.Widgets.Add(play);
                buttons.Widgets.Add(settings);
                buttons.Widgets.Add(controls);
                buttons.Widgets.Add(exit);

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
                };

                button.Content = label;
                return button;
            }
        }
    }
}
