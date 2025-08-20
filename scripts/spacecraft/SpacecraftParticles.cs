using Godot;

namespace FlatLine.Scripts
{
    public partial class SpacecraftParticles : Node2D
    {
        [Export] private Spacecraft spacecraft;
        [Export] private CpuParticles2D thrustParticles;
        [Export] private CpuParticles2D rcsLeftParticles;
        [Export] private CpuParticles2D rcsRightParticles;
        [Export] private Vector2 reversePosition;

        private Vector2 initialPosition;

        public override void _Ready()
        {
            initialPosition = rcsLeftParticles.Position;
        }

        public override void _Process(double delta)
        {
            if (thrustParticles != null)
            {
                thrustParticles.Emitting = spacecraft.ThrustInput;
            }

            if (rcsLeftParticles != null)
            {
                var xPosition = new Vector2(reversePosition.X, initialPosition.X);
                UpdateParticles(spacecraft.RotationInput == -1, rcsLeftParticles, xPosition);
            }

            if (rcsRightParticles != null)
            {
                var xPosition = new Vector2(-reversePosition.X, -initialPosition.X);
                UpdateParticles(spacecraft.RotationInput == 1, rcsRightParticles, xPosition);
            }
        }

        private void UpdateParticles(bool emitting, CpuParticles2D particles, Vector2 xPosition)
        {
            particles.Emitting = emitting;
            if (particles.Emitting) return;

            var x = spacecraft.ReverseThrustInput ? xPosition.X : xPosition.Y;
            var y = spacecraft.ReverseThrustInput ? reversePosition.Y : initialPosition.Y;
            var d = spacecraft.ReverseThrustInput ? -1.0f : 1.0f;

            particles.Direction = new(0.0f, d);
            particles.Position = new(x, y);
            particles.Emitting = spacecraft.ReverseThrustInput;
        }
    }
}