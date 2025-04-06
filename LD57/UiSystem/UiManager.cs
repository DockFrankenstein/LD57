using FontStashSharp;
using Myra;
using Stride.Core.Serialization;
using Stride.Engine.Network;
using Stride.Rendering.Compositing;

namespace LD57.UiSystem
{
    public class UiManager : AsyncScript
    {
        [DataMemberIgnore]
        public MyraRenderer Renderer { get; set; } = new MyraRenderer();

        public PriorityList<IUiCanvas> Canvases { get; set; }

        public LoadedFonts Fonts { get; set; } = new LoadedFonts();

        public override async Task Execute()
        {
            Canvases = new PriorityList<IUiCanvas>(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ui_priority.txt"),
                a => a.UiEnabled,
                a => a.UiName);

            Canvases.OnElementRegistered += _ => UpdateRenderer();
            Canvases.OnElementUnregistered += _ => UpdateRenderer();

            await Canvases.LoadPriorityAsync();

            Fonts.CaviarDreams = await LoadFont("Fonts/CaviarDreams");
            Fonts.CaviarDreamsItalic = await LoadFont("Fonts/CaviarDreams_Italic");
            Fonts.CaviarDreamsBold = await LoadFont("Fonts/CaviarDreams_Bold");
            Fonts.CaviarDreamsBoldItalic = await LoadFont("Fonts/CaviarDreams_BoldItalic");
            Fonts.Roboto = await LoadFont("Fonts/Roboto-Regular");

            MyraEnvironment.DefaultAssetManager = new AssetManagementBase.AssetManager(new StrideAssetAccessor(Content), AppDomain.CurrentDomain.BaseDirectory);
            MyraEnvironment.Game = (Game)Game;

            AddSceneRenderer((Game as Game).SceneSystem.GraphicsCompositor, Renderer);

            Services.AddService(this);
        }

        async Task<FontSystem> LoadFont(string path)
        {
            var font = new FontSystem();
            using (var stream = Content.OpenAsStream(path, Stride.Core.IO.StreamFlags.None))
            {
                var bytes = new byte[stream.Length];
                await stream.ReadAllAsync(bytes, 0, bytes.Length);
                font.AddFont(bytes);
            }
                
            return font;
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

        [DataContractIgnore]
        public class LoadedFonts
        {
            public FontSystem CaviarDreams { get; set; }
            public FontSystem CaviarDreamsItalic { get; set; }
            public FontSystem CaviarDreamsBold { get; set; }
            public FontSystem CaviarDreamsBoldItalic { get; set; }
            public FontSystem Roboto { get; set; }
        }
    }
}
