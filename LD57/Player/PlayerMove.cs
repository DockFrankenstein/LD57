using qASIC.Console;
using Silk.NET.OpenXR;
using Stride.BepuPhysics;
using Stride.BepuPhysics.Components;

namespace LD57.Player
{
    public class PlayerMove : SyncScript, ISimulationUpdate
    {
        public override void Start()
        {
            body = Entity.Get<BodyComponent>();
            rot = body.Orientation;
            this.RegisterInQ();
        }

        public override void Update()
        {
            inputVector = GetInputVector();
        }

        public override void Cancel()
        {
            this.UnregisterInQ();
        }

        [Command("plspeed", Description = "Player speed.")]
        public float Speed { get; set; } = 6f;

        Vector2 inputVector;

        BodyComponent body;
        Quaternion rot;

        Vector2 GetInputVector()
        {
            var vec = Vector2.Zero;

            if (Input.IsKeyDown(Keys.W)) vec.Y += 1f;
            if (Input.IsKeyDown(Keys.S)) vec.Y -= 1f;
            if (Input.IsKeyDown(Keys.D)) vec.X += 1f;
            if (Input.IsKeyDown(Keys.A)) vec.X -= 1f;

            return vec;
        }


        public void AfterSimulationUpdate(BepuSimulation simulation, float simTimeStep)
        {
            body.AngularVelocity = Vector3.Zero;
            body.Orientation = rot;
        }

        public void SimulationUpdate(BepuSimulation simulation, float simTimeStep)
        {
            body.Awake = true;

            Entity.Transform.UpdateWorldMatrix();
            var path = Entity.Transform.WorldMatrix.Forward * inputVector.Y +
                Entity.Transform.WorldMatrix.Right * inputVector.X;

            path.Normalize();
            path *= Speed;

            var velocity = new Vector3(path.X, 0f, path.Z);
            body.LinearVelocity = velocity;
            body.AngularVelocity = Vector3.Zero;
            body.Orientation = rot;
        }
    }
}
