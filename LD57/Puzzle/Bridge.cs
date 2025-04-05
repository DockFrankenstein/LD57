namespace LD57.Puzzle
{
    public class Bridge : AsyncScript
    {
        public const float AnimationDuration = 0.2f;

        public TransformComponent sprite;

        public enum Direction
        {
            Up,
            Left,
            Down,
            Right,
        }

        public int Index { get; private set; } = 0;
        public List<Direction> States { get; set; } = new List<Direction>();

        public List<IActivatable> Triggers { get; set; } = new List<IActivatable>();

        Direction curDir = Direction.Up;

        public override async Task Execute()
        {
            foreach (var item in Triggers)
                item.OnActivate += Switch;

            while (Game.IsRunning && Entity?.Scene != null)
            {
                if (States.Count > 0 && curDir != States[Index])
                {
                    Direction d = curDir;
                    curDir = States[Index];
                    float t = 0;
                    while (t < 1f)
                    {
                        await Script.NextFrame();
                        t += (float)Game.UpdateTime.Elapsed.TotalSeconds / AnimationDuration;
                        LerpRotation(d, curDir, Tween.InOut(t, 2f));
                    }
                }

                await Script.NextFrame();
            }
        }

        public void Switch()
        {
            if (States.Count == 0)
                return;

            Index = (Index + 1) % States.Count;
        }

        void LerpRotation(Direction a, Direction b, float t)
        {
            sprite.Rotation = Quaternion.Lerp(GetRotation(a), GetRotation(b), Math.Clamp(t, 0f, 1f));
        }

        Quaternion GetRotation(Direction dir)
        {
            var angle = dir switch
            {
                Direction.Up => 0f,
                Direction.Left => 0.5f,
                Direction.Down => 1f,
                Direction.Right => 1.5f,
                _ => 0f,
            } * (float)Math.PI;

            return Quaternion.RotationYawPitchRoll(angle, 0f, 0f);
        }
    }
}