using Godot;

namespace FlatLine.Scripts.Environment
{
    public partial class PlanetSystem : Node2D
    {
        [Export] private string planetsPath;
        public override void _Ready()
        {
            using var dir = DirAccess.Open(planetsPath);
            if (dir == null) return;

            dir.ListDirBegin();
            string fileName = dir.GetNext();

            while (fileName != "")
            {
                 
            }
        }
    }
}