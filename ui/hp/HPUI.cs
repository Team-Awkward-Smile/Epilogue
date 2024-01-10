using Epilogue.Global.Singletons;
using Godot;
using System.Linq;

namespace Epilogue.UI.hp;
/// <summary>
///		UI for Hestmor's HP sprites
/// </summary>
public partial class HPUI : Control
{
	// TODO: 189 - This class should inherit UI

	private HBoxContainer _heartContainer;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_heartContainer = GetNode<HBoxContainer>("HBoxContainer");

		for(var i = 0; i < 3; i++)
		{
			_heartContainer.AddChild(new HeartSprite()
			{
				ID = i,
				Main = true,
				Full = true
			});
		}

		var playerEvents = GetNode<PlayerEvents>("/root/PlayerEvents");

		playerEvents.Connect("PlayerWasDamaged", Callable.From((float damageTaken, float currentHP) => UpdateSprites(currentHP)));
		playerEvents.Connect("PlayerWasHealed", Callable.From((float healAmount, float currentHP) => UpdateSprites(currentHP)));
	}

	private void UpdateSprites(float currentHP)
	{
		var currentHearts = _heartContainer.GetChildren().OfType<HeartSprite>().OrderBy(h => h.ID).ToList();

		if(currentHearts.Count < currentHP)
		{
			var maxID = currentHearts.Max(h => h.ID) + 1;

			for(var i = 0; i < currentHP - currentHearts.Count; i++)
			{
				_heartContainer.AddChild(new HeartSprite()
				{
					ID = maxID + i,
					Main = false,
					Full = true
				});
			}

			currentHearts = _heartContainer.GetChildren().OfType<HeartSprite>().OrderBy(h => h.ID).ToList();
		}

		currentHearts.ForEach(h =>
		{
			h.Full = h.ID < currentHP;
		});
	}
}
