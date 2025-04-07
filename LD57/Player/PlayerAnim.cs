using qASIC.Console;

namespace LD57.Player
{
    public class PlayerAnim : SyncScript
    {
        public TransformComponent sprite;

        const double ANIM_SPEED = 1.6;
        const double STAND_REGION_RANGE = 0.05;

        static Vector3 lowPos = new Vector3(0f, 0.4f, 0f);
        static Vector3 highPos = new Vector3(0f, 0.7f, 0f);
        static Vector3 leftRot = new Vector3(0f, 0f, 0.08f*3.1415f);
        static Vector3 rightRot = new Vector3(0f, 0f, -0.08f*3.1415f);

        double t = 0.0;

        Vector3 prevPos;

        [Command("plalwaysanim", Description = "Sets player to always animate movement.")]
        public bool AlwaysAnim { get; set; }

        public override void Start()
        {
            prevPos = Entity.Transform.Position;
            this.RegisterInQ();
        }

        public override void Cancel()
        {
            this.UnregisterInQ();
        }

        public override void Update()
        {
            if (prevPos != Entity.Transform.Position || !IsInStandingRegion() || AlwaysAnim)
            {
                t += Game.UpdateTime.WarpElapsed.TotalSeconds * ANIM_SPEED;
                prevPos = Entity.Transform.Position;
            }

            var pos = Vector3.Lerp(lowPos, highPos, (float)Math.Abs(Math.Sin(t * Math.PI * 2)));
            var rot = Vector3.Lerp(leftRot, rightRot, (float)(Math.Cos(t * Math.PI * 2) + 1f) / 2f);

            sprite.Rotation = Quaternion.RotationYawPitchRoll(rot.Y, rot.X, rot.Z);
            sprite.Position = pos;

            t %= 1.0;
        }

        private bool IsInStandingRegion() =>
            t < STAND_REGION_RANGE ||
            (t > 0.5 - STAND_REGION_RANGE && t < 0.5 + STAND_REGION_RANGE) ||
            (t > 1.0 - STAND_REGION_RANGE);
    }
}
