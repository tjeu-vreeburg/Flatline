using Godot;

namespace FlatLine.Scripts
{
    public partial class GameManager : Node
    {
        [Export] private SpacecraftHud spacecraftHud;
        [Export] private Starfield starfield;
        private Spacecraft spacecraft;
        public void Load(string filePath)
        {
            var packedScene = ResourceLoader.Load<PackedScene>(filePath);
            spacecraft = packedScene.Instantiate() as Spacecraft;

            starfield.Intialise(spacecraft);
            spacecraftHud.Intialise(spacecraft);

            AddChild(spacecraft);
        }
    }
}