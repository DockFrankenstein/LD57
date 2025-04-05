using Stride.BepuPhysics;

namespace LD57.Puzzle
{
    public class TheBigObject : AsyncScript
    {
        const float LERP_DURATION = 3f;

        public int Index { get; private set; } = 0;
        public List<State> States { get; set; } = new List<State>();

        int curState = 0;

        public void ChangeState(int delta)
        {
            if (States.Count == 0)
                return;

            Index = Math.Clamp(Index + delta, 0, States.Count - 1);
        }

        public override async Task Execute()
        {
            for (int i = 0; i < States.Count; i++)
                ToggleCollision(i, false);
            
            if (States.Count > 0)
            {
                LerpState(States[0], 1f);
                ToggleCollision(0, true);
            }


            while (Entity?.Scene != null)
            {
                if (States.Count > 0 && curState != Index)
                {
                    ToggleCollision(curState, false);
                    curState = Index;
                    ToggleCollision(curState, true);
                    var t = 0f;
                    while (t < 1f)
                    {
                        if (curState != Index)
                        {
                            if (t > 0.5f)
                                t = 1f - t;

                            ToggleCollision(curState, false);
                            curState = Index;
                            ToggleCollision(curState, true);
                        }

                        await Script.NextFrame();
                        t += (float)Game.UpdateTime.WarpElapsed.TotalSeconds / LERP_DURATION;
                        LerpState(States[curState], Tween.InOut(t, 3f));
                    }
                }

                await Script.NextFrame();
            }
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

            var pos = States[index].collision.Position;
            pos.Y = active ? 0f : -20f;
            States[index].collision.Position = pos;
        }

        [DataContract]
        public struct State
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public BodyComponent collision;

            public Quaternion GetRotation() =>
                Quaternion.RotationYawPitchRoll(
                    rotation.Y / 180f * (float)Math.PI, 
                    rotation.X / 180f * (float)Math.PI, 
                    rotation.Z / 180f * (float)Math.PI);
        }
    }
}
