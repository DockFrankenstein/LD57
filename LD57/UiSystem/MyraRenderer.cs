using Stride.Rendering.Compositing;
using Stride.Rendering;

namespace LD57.UiSystem
{
    public class MyraRenderer : SceneRendererBase, IIdentifiable
    {
        public List<IUiCanvas> Canvases { get; set; } = new List<IUiCanvas>();

        protected override void DrawCore(RenderContext context, RenderDrawContext drawContext)
        {
            drawContext.CommandList.Clear(GraphicsDevice.Presenter.DepthStencilBuffer, Stride.Graphics.DepthStencilClearOptions.DepthBuffer);
            foreach (var item in Canvases)
                item.DrawUi();
        }
    }
}
