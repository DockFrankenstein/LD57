namespace LD57.Camera
{
    public class CameraTarget : StartupScript
    {
        public string TargetName { get; set; }
        public bool TargetActive { get; set; }

        public override void Start()
        {
            Services.GetService<CameraController>().Targets.Register(this);
        }

        public override void Cancel()
        {
            Services.GetService<CameraController>().Targets.Unregister(this);
        }
    }
}
