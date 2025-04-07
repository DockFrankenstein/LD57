using LD57.Interaction;
using Stride.Rendering;

namespace LD57.Puzzle
{
    public class TheBigObjectReel : SyncScript, IInteractable
    {
        const float IND_ROOT_Y = -3f;

        public TheBigObject Target { get; set; }
        public List<bool> StateAvaliability { get; set; } = new List<bool>();

        public bool Interactable { get; set; } = true;
        [DataMemberIgnore] public bool Focused { get; set; }

        public Model IndicatorModel { get; set; }
        public Material IndMatOn { get; set; }
        public Material IndMatOff { get; set; }
        public Model IndicatorHeadModel { get; set; }

        public void Interact()
        {
            if (Target == null)
            {
                qDebug.LogError("Big Object not assigned to reel!");
                return;
            }

            if (Avaliable(Target.Index) && Avaliable(Target.Index + 1))
                Target?.ChangeState(1);
        }

        public void Interact2()
        {
            if (Target == null)
            {
                qDebug.LogError("Big Object not assigned to reel!");
                return;
            }

            if (Avaliable(Target.Index) && Avaliable(Target.Index - 1))
                Target.ChangeState(-1);
        }

        bool Avaliable(int index) =>
            index >= 0 && (StateAvaliability.Count <= index || StateAvaliability[index]);

        Entity indRoot;
        List<Indicator> Indicators = new List<Indicator>();
        Entity indHead;

        public override void Start()
        {
            indRoot = new Entity(position: new Vector3(0f, IND_ROOT_Y + 2f, 0f));
            indHead = new Entity();
            var indHeadInd = new Entity(position: new Vector3(0f, -IND_ROOT_Y+0.3f, 0f));
            var indHeadModel = new ModelComponent(IndicatorHeadModel);

            Entity.AddChild(indRoot);
            indRoot.AddChild(indHead);
            indHead.AddChild(indHeadInd);
            indHeadInd.Add(indHeadModel);

            if (Target != null)
            {
                var rollOffset = GetRollForIndex((Target.States.Count - 1) / 2f);

                for (int i = 0; i < Target.States.Count; i++)
                {
                    var ind = new Indicator()
                    {
                        pivot = new Entity(
                            rotation: Quaternion.RotationYawPitchRoll(0f, 0f, rollOffset - GetRollForIndex(i))),
                        
                        model = new ModelComponent(IndicatorModel),
                    };

                    var modelEntity = new Entity(
                        position: new Vector3(0f, -IND_ROOT_Y, 0f),
                        rotation: Quaternion.RotationYawPitchRoll(0f, -30f / 180f * (float)Math.PI, 0f));
                    modelEntity.Add(ind.model);

                    ind.model.Materials.Clear();
                    ind.model.Materials.Add(0, Avaliable(i) ? IndMatOn : IndMatOff);

                    ind.pivot.AddChild(modelEntity);
                    indRoot.AddChild(ind.pivot);

                    Indicators.Add(ind);
                }
            }

            LerpIndHead(1f);
        }

        public override void Update()
        {
            LerpIndHead((float)Game.UpdateTime.WarpElapsed.TotalSeconds * 20f);
        }

        void LerpIndHead(float t)
        {
            if (Target != null && Target.States.Count > 0)
                indHead.Transform.Rotation = Quaternion.Lerp(indHead.Transform.Rotation, Indicators[Target.Index].pivot.Transform.Rotation, t);
        }

        float GetRollForIndex(float i) =>
            i * (float)Math.PI * 2f * 0.03f;

        class Indicator
        {
            public Entity pivot;
            public ModelComponent model;
        }
    }
}
