using Epilogue.Global.Enums;

namespace Epilogue.actors.hestmor;
/// <summary>
///     Object used to quickly and easily transmit data related to Growls between Actors
/// </summary>
public class GrowlData
{
    /// <summary>
    ///     Type of Growl
    /// </summary>
    public GrowlType Type { get; set; }

    /// <summary>
    ///     The animation used by the player character when growling
    /// </summary>
    public string Animation { get; set; }

    /// <summary>
    ///     Base strength of the Growl. Higher values mean an Actor will react more strongly
    /// </summary>
    public float Strength { get; set; }

    /// <summary>
    ///     The fall-off of the strength of the performed Growl. Higher values mean the Base Strength will drop more sharply based on the distance between the Actors
    /// </summary>
    public float StrengthFallOff { get; set; }

    /// <summary>
    ///     The radius of the Area2D of the performed Growl. Stronger Growls affect a larger area, and this value is used to determine how far away from the Growl origin an Actor is
    /// </summary>
    public float AreaRadius { get; set; }

    /// <summary>
    ///     The minimum area of the performed Growl. If an Actor's distance to the origin is smaller than this value, then no reduction will happen to the Growl's strength, making the Actor react to the Growl's full force
    /// </summary>
    public float MinimumDistance { get; set; }
}
