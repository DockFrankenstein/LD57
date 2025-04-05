namespace LD57.Camera
{
    public class CameraController : SyncScript
    {
        public override void Start()
        {
            Targets = new PriorityList<CameraTarget>(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/cam_priority.txt"),
                a => a.TargetActive,
                a => a.TargetName);

            Targets.LoadPriority();

            Game.Services.AddService(this);
        }

        public PriorityList<CameraTarget> Targets { get; set; }

        public override void Update()
        {
            Targets.UpdateSelected();
            if (Targets.Selected == null)
            {
                DebugText.Print("There are no camera targets.", new Int2(300, 300));
                return;
            }

            Targets.Selected.Entity.Transform.UpdateWorldMatrix();
            Targets.Selected.Entity.Transform.WorldMatrix.Decompose(out float yaw, out float pitch, out float roll);

            Entity.Transform.Position = Targets.Selected.Entity.Transform.WorldMatrix.TranslationVector;
            Entity.Transform.Rotation = Quaternion.RotationYawPitchRoll(yaw, pitch, roll);
        }

        public override void Cancel()
        {
            Game.Services.RemoveService(this);
        }
    }
}