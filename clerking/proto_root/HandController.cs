using Godot;

public partial class HandController : Node3D
{
    [Export]
    public Node3D Anchor;

    [Export]
    public float MoveSpeed = 3.0f;

    [Export]
    public float MaxReach = 1.25f;

    [Export]
    public string MoveLeftAction = "";

    [Export]
    public string MoveRightAction = "";

    [Export]
    public string MoveForwardAction = "";

    [Export]
    public string MoveBackAction = "";

    [Export]
    public string GrabAction = "";

    private Godot.Collections.Array<GrabbableItem> _nearbyItems = new();
    private GrabbableItem _heldItem;

    public override void _Ready()
    {
        var handBody = GetNode<Area3D>("HandBody");
        handBody.AreaEntered += OnHandBodyAreaEntered;
        handBody.AreaExited += OnHandBodyAreaExited;
    }

    public override void _Process(double delta)
    {
        float x =
            Input.GetActionStrength(MoveRightAction) - Input.GetActionStrength(MoveLeftAction);
        float z =
            Input.GetActionStrength(MoveBackAction) - Input.GetActionStrength(MoveForwardAction);

        Vector3 input = new Vector3(x, 0, z);

        if (input.LengthSquared() > 1.0f)
        {
            input = input.Normalized();
        }

        GlobalPosition += input * MoveSpeed * (float)delta;

        if (Anchor != null)
        {
            Vector3 offset = GlobalPosition - Anchor.GlobalPosition;

            if (offset.Length() > MaxReach)
            {
                offset = offset.Normalized() * MaxReach;
                GlobalPosition = Anchor.GlobalPosition + offset;
            }
        }

        HandleGrab();
    }

    private void OnHandBodyAreaEntered(Area3D area)
    {
        var item = area.GetParent() as GrabbableItem;
        if (item != null && !_nearbyItems.Contains(item))
        {
            _nearbyItems.Add(item);
            GD.Print($"{Name} entered range of {item.Name}");
        }
    }

    private void OnHandBodyAreaExited(Area3D area)
    {
        var item = area.GetParent() as GrabbableItem;
        if (item != null)
        {
            _nearbyItems.Remove(item);
            GD.Print($"{Name} left range of {item.Name}");
        }
    }

    private void HandleGrab()
    {
        if (!Input.IsActionJustPressed(GrabAction))
            return;

        if (_heldItem != null)
        {
            _heldItem.Detach();
            _heldItem = null;
            GD.Print($"{Name} released item");
            return;
        }

        GrabbableItem nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (var item in _nearbyItems)
        {
            if (item == null || item.IsHeld)
                continue;

            float dist = GlobalPosition.DistanceTo(item.GlobalPosition);
            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearest = item;
            }
        }

        if (nearest != null)
        {
            var holdPoint = GetNode<Node3D>("HoldPoint");
            _heldItem = nearest;
            _heldItem.AttachTo(holdPoint);
            GD.Print($"{Name} grabbed {nearest.Name}");
        }
    }
}
