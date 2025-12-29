using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class PlayerInteraction : PlayerComponent
{
    [ExportGroup("Settings")]
    [Export] private Node3D raycastOrigin;
    [Export] private float interactionDistance = 7.5f;
    [Export] private Key interactionKey = Key.E;
    [Export(PropertyHint.Layers3DPhysics)] private uint collisionMask = 1;

    [ExportGroup("References")]
    [Export] private UIWindow interactionUI;
    [Export] private Node3D holdPosition;

    public WorldItem CurrentlyHeldItem { get; private set; }

    public override void _PhysicsProcess(double delta)
    {
	    if (!parentPlayer.IsSetup) return;
        HandleDropInput();
        ProcessInteractionDetection();
    }

    private void HandleDropInput()
    {
        if (Input.IsPhysicalKeyPressed(Key.Q) && CurrentlyHeldItem != null)
        {
            DropWorldItem();
        }
    }

    private void ProcessInteractionDetection()
    {
        var hitData = PerformRaycast(interactionDistance);

        if (hitData.TryGetValue("collider", out Variant colliderVariant))
        {
            Node3D collider = (Node3D)colliderVariant;
            var interactable = ExtractInteractable(collider);

            if (interactable != null)
            {
                UpdateUI(true);
                HandleInteractionInput(interactable, collider);
                return;
            }
        }

        UpdateUI(false);
    }

    public Godot.Collections.Dictionary PerformRaycast(float length)
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        Vector3 from = raycastOrigin.GlobalPosition;
        Vector3 to = from + (-raycastOrigin.GlobalBasis.Z * length);

        var query = PhysicsRayQueryParameters3D.Create(from, to, collisionMask);
        return spaceState.IntersectRay(query);
    }

    private IInteractable ExtractInteractable(Node3D collider)
    {
	    Node current = collider;
	    
	    while (current != null)
	    {
		    //Does this specific node implement the interface
		    if (current is IInteractable interactable)
		    {
			    return interactable;
		    }

		    //Is this node a WorldItem that contains an InteractableComponent
		    if (current is WorldItem item && 
		        item.TryGetItemComponent<InteractalbleComponent>(out var component))
		    {
			    return (IInteractable)component;
		    }

		    // Move up to the parent and try again
		    current = current.GetParent();
	    }

	    return null;
    }

    private void HandleInteractionInput(IInteractable interactable, Node3D collider)
    {
        if (Input.IsActionJustPressed("interact") || Input.IsPhysicalKeyPressed(interactionKey))
        {
            WorldItem interactionTarget = collider is WorldItem ? collider as WorldItem : null;
            interactable.Interact(parentPlayer, interactionTarget);
        }
    }

    private void UpdateUI(bool canInteract)
    {
        if (canInteract)
            interactionUI.ShowCall();
        else
            interactionUI.HideCall();
    }

    public bool TryHoldWorldItem(WorldItem worldItem)
    {
	    if (holdPosition == null)
	    {
		    GD.Print("holdPosition is null");
		    return false;
	    }
	    if(CurrentlyHeldItem != null)
	    {
		    GD.Print("CurrentlyHeldItem is not null");
		    return false;
	    }

        CurrentlyHeldItem = worldItem;
        CurrentlyHeldItem.Freeze = true;
        
        // Reparent to hold position
        CurrentlyHeldItem.Reparent(holdPosition, false);
        CurrentlyHeldItem.GlobalPosition = holdPosition.GlobalPosition;
        CurrentlyHeldItem.Rotation = Vector3.Zero;
        
        return true;
    }

    public bool DropWorldItem()
    {
        if (CurrentlyHeldItem == null) return false;

        // Reparent back to the scene (Root or Current Level)
        CurrentlyHeldItem.Reparent(GetTree().CurrentScene, true);
        CurrentlyHeldItem.Freeze = false;
        CurrentlyHeldItem = null;

        return true;
    }

    protected override void Initialize(Player parent)
    {
        interactionUI = UIManager.Instance.uiWindows[Enums.UIType.INTERACTUI];
    }
}