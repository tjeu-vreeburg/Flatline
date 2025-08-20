using System;
using Godot;

namespace FlatLine.Scripts
{
    public partial class SpacecraftHud : PanelContainer
    {
        [Export] private ProgressBar progressBar;
        [Export] private Label velocityLabel;
        [Export] private Label spacecraftLabel;
        [Export] private float thrustSmoothing = 5.0f;

        private Spacecraft spacecraft;

        public void Intialise(Spacecraft spacecraft)
        {
            this.spacecraft = spacecraft;
            spacecraftLabel.Text = spacecraft.Name.ToString().ToUpper();
        }

        public override void _Process(double delta)
        {
            progressBar.Value = Mathf.Lerp(progressBar.Value, spacecraft.CurrentThrust, (float)(delta * thrustSmoothing));
            velocityLabel.Text = $"{(int)spacecraft.Velocity()} k/ph";
        }
    }
}