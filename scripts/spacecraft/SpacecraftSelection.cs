using FlatLine.Scripts.UserInterface;
using Godot;

namespace FlatLine.Scripts
{
    public partial class SpacecraftSelection : Control
    {
        [Export] private Control parent;
        [Export] private PackedScene packedSpacecraftCard;
        [Export] private string path;

        public override void _Ready()
        {
            using var dir = DirAccess.Open(path);
            if (dir != null)
            {
                dir.ListDirBegin();
                string fileName = dir.GetNext();
                while (fileName != "")
                {
                    if (!dir.CurrentIsDir())
                    {
                        var card = packedSpacecraftCard.Instantiate() as SpacecraftCard;
                        card.Deserialize(path, fileName);

                        parent.AddChild(card);
                    }
                    fileName = dir.GetNext();
                }
            }
        }
    }
}