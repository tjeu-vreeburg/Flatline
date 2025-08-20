using Godot;

namespace FlatLine.Scripts.UserInterface
{
    public partial class SpacecraftCard : PanelContainer
    {
        [Export] private Button button;
        [Export] private TextureRect textureRect;
        [Export] private Label label;

        public void Deserialize(string path, string fileName)
        {
            label.Text = fileName.Split('.')[0].ToUpper();

            button.Pressed += () =>
            {
                var tree = GetTree();
                var currentScene = tree.CurrentScene;

                currentScene?.QueueFree();


                var packedScene = ResourceLoader.Load<PackedScene>("res://scenes/game.tscn");
                var gameManager = packedScene.Instantiate() as GameManager;
                gameManager.Load(path + fileName);

                tree.Root.AddChild(gameManager);
                tree.CurrentScene = gameManager;
            };
        }
    }
}