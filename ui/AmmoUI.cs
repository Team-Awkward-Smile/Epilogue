using Epilogue.global.singletons;
using Godot;

namespace Epilogue.ui;
/// <summary>
///		UI Screen responsible for displaying the ammo information of the current gun (if any)
/// </summary>
public partial class AmmoUI : Control
{
	private int _maxAmmo;
	private int _currentAmmo;
	private GunEvents _gunEvents;
	private Label _ammoUI;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_gunEvents = GetNode<GunEvents>("/root/GunEvents");
		_ammoUI = (Label) GetChild(0);

		_ammoUI.Hide();

		_gunEvents.Connect("PlayerPickedUpGun", Callable.From((int currentAmmo, int maxAmmo) =>
		{
			_currentAmmo = currentAmmo;
			_maxAmmo = maxAmmo;
			_ammoUI.Text = $"{_currentAmmo.ToString().PadLeft(2, '0')} / {_maxAmmo.ToString().PadLeft(2, '0')}";
			_ammoUI.Show();
		}));

		_gunEvents.Connect("GunFired", Callable.From((int currentAmmo) =>
		{
			_currentAmmo = currentAmmo;
			_ammoUI.Text = $"{_currentAmmo.ToString().PadLeft(2, '0')} / {_maxAmmo.ToString().PadLeft(2, '0')}";
		}));

		_gunEvents.Connect("GunWasDropped", Callable.From(() => _ammoUI.Hide()));
	}
}
