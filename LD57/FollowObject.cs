using Stride.BepuPhysics;
using Stride.BepuPhysics.Components;

namespace LD57
{
    public class FollowObject : SyncScript, ISimulationUpdate
    {
        public Vector3 PosOffset { get; set; }
        public Entity Target { get; set; }

        public bool FollowPosition { get; set; } = true;
        public bool FollowRotation { get; set; } = true;

        public bool WhenUpdate { get; set; } = true;
        public bool WhenSimulationUpdate { get; set; }
        public bool WhenAfterSimulationUpdate { get; set; }

        BodyComponent _body;

        public override void Start()
        {
            _body = Entity.Get<BodyComponent>();
        }

        public override void Update()
        {
            if (WhenUpdate)
                UpdateFollow();
        }

        public void AfterSimulationUpdate(BepuSimulation simulation, float simTimeStep)
        {
            if (WhenAfterSimulationUpdate)
                UpdateFollow();
        }

        public void SimulationUpdate(BepuSimulation simulation, float simTimeStep)
        {
            if (WhenSimulationUpdate)
                UpdateFollow();
        }

        public void UpdateFollow()
        {
            if (Target == null)
            {
                qDebug.LogError("No target assigned to follow!");
                return;
            }

            Target.Transform.UpdateWorldMatrix();
            Target.Transform.WorldMatrix.Decompose(out _, out Quaternion rot, out Vector3 pos);

            pos += PosOffset;

            if (_body != null)
            {
                if (FollowPosition) _body.Position = pos;
                if (FollowRotation) _body.Orientation = rot;

                return;
            }

            if (FollowPosition) Entity.Transform.Position = pos;
            if (FollowRotation) Entity.Transform.Rotation = rot;
        }
    }
}
