using Godot;

public partial class HandController : Node3D
{
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			GD.Print($"Key event received: keycode={keyEvent.Keycode}, physical={keyEvent.PhysicalKeycode}");
		}
	}
}
