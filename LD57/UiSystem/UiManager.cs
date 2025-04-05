using Myra;
using Silk.NET.OpenXR;
using Stride.Rendering.Compositing;

namespace LD57.UiSystem
{
    public class UiManager : AsyncScript
    {
        [DataMemberIgnore]
        public MyraRenderer Renderer { get; set; } = new MyraRenderer();

        [DataMemberIgnore]
        public List<string> CanvasPriority { get; private set; } = new List<string>();

        public override async Task Execute()
        {
            await LoadPriority();

            MyraEnvironment.DefaultAssetManager = new AssetManagementBase.AssetManager(new StrideAssetAccessor(Content), AppDomain.CurrentDomain.BaseDirectory);
            MyraEnvironment.Game = (Game)Game;

            AddSceneRenderer((Game as Game).SceneSystem.GraphicsCompositor, Renderer);

            Services.AddService(this);
        }

        async Task LoadPriority()
        {
            var txt = await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ui_priority.txt"));
            CanvasPriority = txt.ReplaceLineEndings().Split(Environment.NewLine)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => !x.StartsWith("#"))
                .ToList();
        }

        public override void Cancel()
        {
            Renderer.Canvases.Clear();
        }

        public void RegisterCanvas(IUiCanvas canvas)
        {
            var i = Renderer.Canvases.Count;
            var newPrio = CanvasPriority.IndexOf(canvas.UiName);

            for (; i > 0; i--)
            {
                var prio = CanvasPriority.IndexOf(Renderer.Canvases[i-1].UiName);
                if (newPrio >= prio)
                    break;
            }

            Renderer.Canvases.Insert(i, canvas);
        }

        public void UnregisterCanvas(IUiCanvas canvas)
        {
            Renderer.Canvases.Remove(canvas);
        }

        private static GraphicsCompositor AddSceneRenderer(GraphicsCompositor graphicsCompositor, SceneRendererBase sceneRenderer)
        {
            if (graphicsCompositor.Game is SceneRendererCollection sceneRendererCollection)
            {
                sceneRendererCollection.Children.Add(sceneRenderer);
            }
            else
            {
                var newSceneRendererCollection = new SceneRendererCollection();

                newSceneRendererCollection.Children.Add(graphicsCompositor.Game);
                newSceneRendererCollection.Children.Add(sceneRenderer);

                graphicsCompositor.Game = newSceneRendererCollection;
            }

            return graphicsCompositor;
        }
    }
}
