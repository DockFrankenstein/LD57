using Stride.BepuPhysics;
using System.Windows.Documents;

namespace LD57.Puzzle
{
    public class Bridge : AsyncScript
    {
        public const float AnimationDuration = 0.2f;

        public BodyComponent axis;

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

        public BodyComponent UpBlock { get; set; }
        public BodyComponent LeftBlock { get; set; }
        public BodyComponent DownBlock { get; set; }
        public BodyComponent RightBlock { get; set; }

        public bool UpFree { get; set; }
        public bool LeftFree { get; set; }
        public bool DownFree { get; set; }
        public bool RightFree { get; set; }

        Direction curDir = Direction.Up;

        public override async Task Execute()
        {
            foreach (var item in Triggers)
                item.OnActivate += Switch;

            if (UpFree) ToggleBlock(UpBlock, false);
            if (LeftFree) ToggleBlock(LeftBlock, false);
            if (DownFree) ToggleBlock(DownBlock, false);
            if (RightFree) ToggleBlock(RightBlock, false);

            while (Game.IsRunning && Entity?.Scene != null)
            {
                if (States.Count > 0 && curDir != States[Index])
                {
                    Direction d = curDir;
                    curDir = States[Index];
                    float t = 0;

                    ToggleBlock(GetBlock(d), true);
                    ToggleBlock(GetBlock(curDir), false);

                    while (t < 1f)
                    {
                        await axis.Simulation.NextUpdate();
                        t += (float)Game.UpdateTime.WarpElapsed.TotalSeconds / AnimationDuration;
                        LerpRotation(d, curDir, Tween.InOut(t, 3f));
                    }
                }

                await axis.Simulation.NextUpdate();
            }
        }

        public void Switch()
        {
            if (States.Count == 0)
                return;

            Index = (Index + 1) % States.Count;
        }

        BodyComponent GetBlock(Direction dir)
        {
            return dir switch
            {
                Direction.Left => LeftBlock,
                Direction.Down => DownBlock,
                Direction.Right => RightBlock,
                _ => UpBlock,
            };
        }

        void ToggleBlock(BodyComponent block, bool active)
        {
            var pos = block.Position;
            pos.Y = active ? 0f : -20f;
            block.Position = pos;
        }

        void LerpRotation(Direction a, Direction b, float t)
        {
            axis.Orientation = Quaternion.Lerp(GetRotation(a), GetRotation(b), Math.Clamp(t, 0f, 1f));
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