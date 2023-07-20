using Epilogue.global.singletons;
using Godot;

namespace Epilogue.ui;
public partial class AmmoUI : Control
{
	private int _maxAmmo;
	private int _currentAmmo;
	private Events _events;
	private Label _ammoUI;

	public override void _Ready()
	{
		_events = GetNode<Events>("/root/Events");
		_ammoUI = (Label) GetChild(0);

		_ammoUI.Hide();

		_events.PlayerPickedUpGun += (int currentAmmo, int maxAmmo) =>
		{
			_currentAmmo = currentAmmo;
			_maxAmmo = maxAmmo;
			_ammoUI.Text = $"{_currentAmmo.ToString().PadLeft(2, '0')} / {_maxAmmo.ToString().PadLeft(2, '0')}";
			_ammoUI.Show();
		};

		_events.GunFired += (int currentAmmo) =>
		{
			_currentAmmo = currentAmmo;
			_ammoUI.Text = $"{_currentAmmo.ToString().PadLeft(2, '0')} / {_maxAmmo.ToString().PadLeft(2, '0')}";
		};

		_events.GunWasDropped += () => _ammoUI.Hide();
	}
}
