using BepuPhysics.Collidables;
using LD57.Interaction;
using Stride.BepuPhysics;
using Stride.BepuPhysics.Components;
using Stride.BepuPhysics.Definitions;
using System.Collections.ObjectModel;

namespace LD57.Player
{
    public class PlayerInteract : SyncScript, ISimulationUpdate
    {
        PlayerInput _input;

        public override void Start()
        {
            _input = Entity.Get<PlayerInput>();
        }

        public override void Update()
        {
            if (_input.HasInputFocus && Focused != null)
            {
                if (Input.IsKeyPressed(Keys.E))
                    Focused.Interact();

                if (Input.IsKeyPressed(Keys.Q))
                    Focused.Interact2();
            }

            DebugText.Print($"Focused: {(Focused == null ? "NULL" : (Focused as ScriptComponent).Entity.Name)}", new Int2(50,50));
        }

        Sphere sphere = new Sphere(2f);

        List<IInteractable> inRange = new List<IInteractable>();

        IInteractable _focused;

        [DataMemberIgnore] public IInteractable Focused
        {
            get => _focused;
            set
            {
                if (_focused == value) return;

                if (_focused != null)
                    _focused.Focused = false;

                _focused = value;

                if (_focused != null)
                    _focused.Focused = true;
            }
        }

        public void SimulationUpdate(BepuSimulation simulation, float simTimeStep)
        {
            Collection<HitInfo> res = new Collection<HitInfo>();
            simulation.SweepCastPenetrating(sphere, 
                new RigidPose(Entity.Transform.Position + 6f*Vector3.UnitY, Entity.Transform.Rotation), 
                new BodyVelocity(-Vector3.UnitY, Vector3.Zero), 
                10f, 
                res);

            inRange.Clear();

            foreach (var item in res)
            {
                var interactable = item.Collidable.Entity.BetterGet<IInteractable>();
                if (interactable == null) continue;
                inRange.Add(interactable);
            }
        }

        public void AfterSimulationUpdate(BepuSimulation simulation, float simTimeStep)
        {
            Entity.Transform.UpdateWorldMatrix();
            var ownPos = Entity.Transform.WorldMatrix.TranslationVector;

            float targetRange = 0f;
            IInteractable target = null;
            foreach (var item in inRange)
            {
                if (!item.Interactable)
                    continue;

                var entity = (item as ScriptComponent).Entity;
                entity.Transform.UpdateWorldMatrix();
                var pos = entity.Transform.WorldMatrix.TranslationVector;
                var range = Vector3.Distance(ownPos, pos);

                if (target == null || range < targetRange)
                {
                    target = item;
                    targetRange = range;
                }
            }

            Focused = target;
        }
    }
}
