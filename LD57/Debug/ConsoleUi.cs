using LD57.UiSystem;
using Myra;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using qASIC.Console;
using System.Text;

namespace LD57.Debug
{
    public class ConsoleUi : SyncScript
    {
        public const int LOG_LIMIT = 256;

        public UiCanvas Canvas { get; set; }

        ConsoleWidget root;
        GameConsole console;

        public override void Start()
        {
            console = Services.GetService<GameConsole>();
            console.Logs.OnLog += _ => RefreshLogs();
            console.Logs.OnUpdateLog += _ => RefreshLogs();

            root = new ConsoleWidget();
            Canvas.Root = root;

            root.inputBox.Char += InputBox_Char;

            RefreshLogs();
        }

        private void InputBox_Char(object sender, Myra.Events.GenericEventArgs<char> e)
        {
            if (e.Data == '\r')
            {
                Execute();
            }
        }

        private void Execute()
        {
            var cmd = root.inputBox.Text?.TrimEnd('\r') ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(cmd))
                console.Execute(cmd);
            
            root.inputBox.Text = "";
        }

        private void RefreshLogs()
        {
            var txt = new StringBuilder();
            var length = Math.Min(console.Logs.Logs.Count, LOG_LIMIT);

            for (int i = console.Logs.Logs.Count - length; i < console.Logs.Logs.Count; i++)
            {
                var log = console.Logs.Logs[i];
                txt.AppendLine($"/c[{ToHex(console.Theme.GetLogColor(log))}] [{log.time:HH:mm:ss}] {log.message}/cd");
            }

            root.logs.Text = txt.Length == 0 ? "" : txt.ToString().Substring(0, txt.Length - 1);

            resetScrollNextFrame = true;
        }

        bool resetScrollNextFrame;
        bool enableInputNextFrame;


        public override void Update()
        {
            if (resetScrollNextFrame)
            {
                resetScrollNextFrame = false;
                root.scroll.ScrollPosition = new Point(0,root.scroll.ScrollMaximum.Y);
            }

            if (enableInputNextFrame)
            {
                enableInputNextFrame = false;
                root.inputBox.Enabled = true;
                root.inputBox.SetKeyboardFocus();
            }

            if (Input.IsKeyPressed(Keys.OemTilde) || Input.IsKeyPressed(Keys.Oem3))
            {
                Canvas.UiEnabled = !Canvas.UiEnabled;

                if (Canvas.UiEnabled)
                {
                    root.inputBox.Enabled = false;
                    enableInputNextFrame = true;
                }
            }
        }

        static string ToHex(qColor c) => $"#{c.red:X2}{c.green:X2}{c.blue:X2}";

        private class ConsoleWidget : Grid
        {
            public Button inputButton;
            public TextBox inputBox;
            public ScrollViewer scroll;
            public Label logs;

            public ConsoleWidget()
            {
                var window = new Grid()
                {
                    Margin = new Myra.Graphics2D.Thickness(100,100),
                    Padding = new Myra.Graphics2D.Thickness(1,1),
                    RowSpacing = 1,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = new SolidBrush(new Color(150, 150, 150, 255)),
                };

                window.RowsProportions.Add(new Proportion(ProportionType.Fill));
                window.RowsProportions.Add(new Proportion(ProportionType.Pixels, 20f));

                scroll = new ScrollViewer()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = new SolidBrush(new Color(20,20,20,255)),
                    ShowHorizontalScrollBar = false,
                };

                logs = new Label()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Wrap = true,
                };

                var input = new Grid()
                {
                    Background = new SolidBrush(new Color(150, 150, 150, 255)),
                    ColumnSpacing = 1,
                };

                input.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
                input.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, 140f));

                inputBox = new TextBox()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                };

                inputButton = new Button()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };

                var inputButtonText = new Label()
                {
                    Text = "Execute",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                Grid.SetRow(scroll, 0);
                Grid.SetRow(input, 1);

                Grid.SetColumn(inputBox, 0);
                Grid.SetColumn(inputButton, 1);

                base.Widgets.Add(window);
                window.Widgets.Add(scroll);
                window.Widgets.Add(input);

                input.Widgets.Add(inputBox);
                input.Widgets.Add(inputButton);

                scroll.Content = logs;

                inputButton.Content = inputButtonText;
            }
        }
    }
}
