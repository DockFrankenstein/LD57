using Stride.BepuPhysics;
using Stride.Rendering;

namespace LD57.Puzzle
{
    public class TheBigObject : AsyncScript
    {
        const float LERP_SPEED = 20f;
        const float POS_SPEED = 20f;
        const float ROT_SPEED = 360f;
        const float SCALE_SPEED = 8f;

        public ModelComponent HandModel { get; set; }
        public List<Material> Hands { get; set; } = new List<Material>();

        public int Index { get; private set; } = 0;
        public List<State> States { get; set; } = new List<State>();

        ModelComponent model;

        State cur;

        public void ChangeState(int delta)
        {
            if (States.Count == 0)
                return;

            ToggleCollision(Index, false);
            Index = Math.Clamp(Index + delta, 0, States.Count - 1);
            ToggleCollision(Index, true);
        }

        float handAnim = -1f;

        public override async Task Execute()
        {
            model = Entity.Get<ModelComponent>();

            for (int i = 0; i < States.Count; i++)
                ToggleCollision(i, false);

            if (States.Count > 0)
            {
                LerpState(States[0], 1f);
                ToggleCollision(0, true);
                cur = new State()
                {
                    position = States[0].position,
                    rotation = States[0].rotation,
                    scale = States[0].scale,
                };
            }

            while (Entity?.Scene != null)
            {
                var time = (float)Game.UpdateTime.WarpElapsed.TotalSeconds;
                if (States.Count > 0)
                {
                    cur.position = MoveLerp(cur.position, States[Index].position, time * POS_SPEED);
                    cur.rotation = MoveLerp(cur.rotation, States[Index].rotation, time * ROT_SPEED);
                    cur.scale = MoveLerp(cur.scale, States[Index].scale, time * SCALE_SPEED);
                }

                LerpState(cur, time * LERP_SPEED);

                DebugText.Print($"Big Object: {Index + 1}", new Int2(50, 100));

                if (handAnim >= 0f)
                    handAnim += time;

                UpdateHandAnim();

                await Script.NextFrame();
            }
        }

        void UpdateHandAnim()
        {
            if (handAnim < 0f || handAnim > Hands.Count / 12f)
            {
                HandModel.Enabled = false;
                handAnim = -1f;
                return;
            }

            HandModel.Enabled = true;

            var index = (int)(handAnim * 12f) % Hands.Count;
            var curMaterial = Hands[index];
            if (HandModel.Materials.Count == 0 || HandModel.Materials[0] != curMaterial)
            {
                HandModel.Materials.Clear();
                HandModel.Materials.Add(0, curMaterial);
            }

            model.Enabled = index < 4;
        }

        void LerpState(State b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            Entity.Transform.Position = Vector3.Lerp(Entity.Transform.Position, b.position, t);
            Entity.Transform.Rotation = Quaternion.Lerp(Entity.Transform.Rotation, b.GetRotation(), t);
            Entity.Transform.Scale = Vector3.Lerp(Entity.Transform.Scale, b.scale, t);
        }

        void ToggleCollision(int index, bool active)
        {
            if (!States.IndexInRange(index)) return;
            if (States[index].collision == null) return;

            var collisions = States[index].collision.GetChildren()
                .Select(x => x.Get<BodyComponent>())
                .Where(x => x != null);

            foreach (var item in collisions)
            {
                var pos = item.Position;
                pos.Y = active ? 0f : -20f;
                item.Position = pos;
            }
        }

        public void Grab()
        {
            handAnim = 0f;
        }

        static Vector3 MoveLerp(Vector3 a, Vector3 b, float distance)
        {
            var diff = b - a;

            if (diff.Length() < distance)
                return a + diff;

            diff.Normalize();
            diff *= distance;
            return a + diff;
        }

        [DataContract]
        public struct State
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public Entity collision;

            public Quaternion GetRotation() =>
                Quaternion.RotationYawPitchRoll(
                    rotation.Y / 180f * (float)Math.PI, 
                    rotation.X / 180f * (float)Math.PI, 
                    rotation.Z / 180f * (float)Math.PI);
        }
    }
}
