using Godot;

namespace Epilogue.UI.HP;
/// <summary>
///		Object representing 1 Heart shown in the HP Screen
/// </summary>
public partial class HeartSprite : TextureRect
{
    /// <summary>
    ///     Unique ID of this Heart, used to easily access it during runtime
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    ///     Defines if this Heart is one of the 3 Main Hearts.
    ///     Main Hearts will not be destroyed when emptied
    /// </summary>
    public bool Main { get; set; }

    /// <summary>
    ///     Defines if this Heart if full or not
    /// </summary>
    public bool Full 
    {
        get => _full;
        set
        {
            _full = value;

            if(!_full && !Main)
            {
                QueueFree();
            }
            else
            {
                Texture = GD.Load<CompressedTexture2D>(_full ? "res://ui/hp/HeartFull.png" : "res://ui/hp/HeartEmpty.png");
            }
        }
    }

    private bool _full;
}
