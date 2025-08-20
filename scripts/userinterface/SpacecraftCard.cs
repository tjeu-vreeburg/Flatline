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
            var spacecraftName = fileName.Split('.')[0];
            var spacecraftTexture = GD.Load<Texture2D>("res://sprites/" + spacecraftName + ".png");

            label.Text = spacecraftName.ToUpper();
            textureRect.Texture = spacecraftTexture;

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