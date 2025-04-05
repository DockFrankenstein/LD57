using Myra;
using Stride.Rendering.Compositing;

namespace LD57.UiSystem
{
    public class UiManager : AsyncScript
    {
        [DataMemberIgnore]
        public MyraRenderer Renderer { get; set; } = new MyraRenderer();

        public PriorityList<IUiCanvas> Canvases { get; set; }

        public override async Task Execute()
        {
            Canvases = new PriorityList<IUiCanvas>(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ui_priority.txt"),
                a => a.UiEnabled,
                a => a.UiName);

            Canvases.OnElementRegistered += _ => UpdateRenderer();
            Canvases.OnElementUnregistered += _ => UpdateRenderer();

            await Canvases.LoadPriorityAsync();

            MyraEnvironment.DefaultAssetManager = new AssetManagementBase.AssetManager(new StrideAssetAccessor(Content), AppDomain.CurrentDomain.BaseDirectory);
            MyraEnvironment.Game = (Game)Game;

            AddSceneRenderer((Game as Game).SceneSystem.GraphicsCompositor, Renderer);

            Services.AddService(this);
        }

        void UpdateRenderer()
        {
            Renderer.Canvases.Clear();
            Renderer.Canvases.AddRange(Canvases.Reverse());
        }

        public override void Cancel()
        {
            Renderer.Canvases.Clear();
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
