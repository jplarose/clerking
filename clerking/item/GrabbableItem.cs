using Godot;

public partial class GrabbableItem : Node3D
{
	private Node3D _followTarget;
	public bool IsHeld { get; private set; }

	public override void _Process(double delta)
	{
		if (IsHeld && _followTarget != null)
		{
			GlobalPosition = _followTarget.GlobalPosition;
		}
	}

	public void AttachTo(Node3D target)
	{
		_followTarget = target;
		IsHeld = true;
	}

	public void Detach()
	{
		_followTarget = null;
		IsHeld = false;
	}
}
