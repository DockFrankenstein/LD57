using Stride.Rendering;

namespace LD57
{
    public class TextureAnimator : SyncScript
    {
        public List<Material> Materials { get; set; } = new List<Material>();

        double t = 0;

        ModelComponent model;

        public override void Start()
        {
            model = Entity.Get<ModelComponent>();
        }

        public override void Update()
        {
            if (Materials.Count == 0) return;

            t += Game.UpdateTime.WarpElapsed.TotalSeconds;
            var index = (int)(t * Materials.Count) % Materials.Count;

            if (model.Materials.Count == 0 || model.Materials[0] != Materials[index])
            {
                model.Materials.Clear();
                model.Materials.Add(0, Materials[index]);
            }

            t %= 1.0;
        }
    }
}
