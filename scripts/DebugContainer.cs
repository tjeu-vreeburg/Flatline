using Godot;

namespace FlatLine.Scripts
{
    public partial class DebugContainer : PanelContainer
    {
        [Export] public SpinBox Thrust;
        [Export] public SpinBox ReverseThrust;
        [Export] public SpinBox RotationThrust;
        [Export] public SpinBox BulletSpeed;
        [Export] public SpinBox BulletLifeTime;
        [Export] public SpinBox BulletFireRate;
        [Export] public OptionButton BulletMode;
    }
}