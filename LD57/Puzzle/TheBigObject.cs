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
            if (States.Count > 0)
                LerpState(States[0], 1f);

            while (Entity?.Scene != null)
            {
                if (States.Count > 0 && curState != Index)
                {
                    curState = Index;
                    var t = 0f;
                    while (t < 1f)
                    {
                        if (curState != Index)
                        {
                            if (t > 0.5f)
                                t = 1f - t;
                            curState = Index;
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

        [DataContract]
        public struct State
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public float transparency;

            public Quaternion GetRotation() =>
                Quaternion.RotationYawPitchRoll(
                    rotation.Y / 180f * (float)Math.PI, 
                    rotation.X / 180f * (float)Math.PI, 
                    rotation.Z / 180f * (float)Math.PI);
        }
    }
}
