using Godot;

namespace FlatLine.Scripts
{
    public partial class SpacecraftSound : Node2D
    {
        [Export] private Spacecraft spacecraft;
        [Export] private AudioStreamPlayer thrustSound;
        [Export] private AudioStreamPlayer rcsSound;
        [Export] private AudioStreamPlayer bulletSound;
        [Export] private float thrustSmoothing = 5.0f;

        public override void _Process(double delta)
        {
            if (spacecraft == null) return;

            if (spacecraft.ThrustInput)
            {
                if (!thrustSound.Playing) thrustSound.Play();
            }
            else
            {
                if (thrustSound.Playing) thrustSound.Stop();
            }

            if (spacecraft.RotationInput == -1 || spacecraft.RotationInput == 1 || spacecraft.ReverseThrustInput)
            {
                if (!rcsSound.Playing) rcsSound.Play();
            }
            else
            {
                if (rcsSound.Playing) rcsSound.Stop();
            }
            
            if (spacecraft.BulletInput)
            {
                bulletSound.Play();
            }
        }
    }
}