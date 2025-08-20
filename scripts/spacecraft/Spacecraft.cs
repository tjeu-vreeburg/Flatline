using Godot;

namespace FlatLine.Scripts
{
    public partial class Spacecraft : RigidBody2D
    {
        [Export] private Camera2D camera;
        [Export] private PackedScene bulletScene;
        [Export] private SpacecraftBulletMode bulletMode;
        [Export] private double thrustPower = 2.0f;
        [Export] private double reverseThrustPower = 0.8f;
        [Export] private double rotationThrustPower = 10.0f;
        [Export] private float generalForce = 100.0f;
        [Export] private double fireCooldown = 0.2D;

        public bool ThrustInput { get; private set; }
        public bool BulletInput { get; private set; }
        public bool ReverseThrustInput { get; private set; }
        public float RotationInput { get; private set; }
        public float CurrentThrust { get; private set; }

        public float Velocity() => LinearVelocity.Length();

        private double fireTimer = 0.0D;

        public override void _Process(double delta)
        {
            HandleInput();
            HandleShooting(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            ApplyThrust();
            ApplyTorque(RotationInput * (float)rotationThrustPower * generalForce);
        }

        private void HandleInput()
        {
            ThrustInput = Input.IsActionPressed("thrust");
            ReverseThrustInput = Input.IsActionPressed("reverse_thrust");

            if (Input.IsActionPressed("rotate_left"))
            {
                RotationInput = -1.0f;
            }
            else if (Input.IsActionPressed("rotate_right"))
            {
                RotationInput = 1.0f;
            }
            else
            {
                RotationInput = 0.0f;
            }
        }

        private void HandleShooting(double delta)
        {
            if (bulletMode == SpacecraftBulletMode.None) return;

            fireTimer -= delta;
            if (Input.IsActionPressed("shoot") && fireTimer <= 0.0D)
            {
                Shoot(bulletMode);
                BulletInput = true;
                fireTimer = fireCooldown;
            }
            else
            {
                BulletInput = false;
            }
        }

        private void ApplyThrust()
        {
            if (ThrustInput)
            {
                CurrentThrust = Thrust(Vector2.Up);
            }
            else if (ReverseThrustInput)
            {
                CurrentThrust = Thrust(Vector2.Down);
            }
            else
            {
                CurrentThrust = 0.0f;
            }
        }

        private float Thrust(Vector2 vector)
        {
            var thrustDirection = vector.Rotated(Rotation);
            var thrust = (vector == Vector2.Up) ? thrustPower : reverseThrustPower;
            var thrustForce = thrustDirection * (float)thrust * generalForce;
            ApplyForce(thrustForce);

            return thrustForce.Length();
        }

        private void Shoot(SpacecraftBulletMode bulletMode)
        {
            var bullet = bulletScene.Instantiate() as SpacecraftBullet;
            var direction = new Vector2();
            switch (bulletMode)
            {
                case SpacecraftBulletMode.Directional:
                    direction = Vector2.Up.Rotated(Rotation);
                    break;
                case SpacecraftBulletMode.Mouse:
                    var mousePosition = GetGlobalMousePosition();
                    direction = mousePosition - GlobalPosition;
                    break;
            }
            if (bulletScene == null) return;

            bullet.GlobalPosition = GlobalPosition;
            bullet.Scale = new(0.1f, 0.1f);
            bullet.ZIndex = -100;
            bullet.Shoot(direction);

            GetTree().Root.AddChild(bullet);
        }

        public Camera2D GetCamera()
        {
            return camera;
        }
    }
}