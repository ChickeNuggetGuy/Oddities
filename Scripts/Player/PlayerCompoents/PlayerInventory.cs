using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class PlayerInventory : PlayerComponent
{
    public int selectedItemIndex = 0;
	
    [Export] public Dictionary<Enums.InventoryType, Inventory> inventories = new Dictionary<Enums.InventoryType, Inventory>();
    [Export(PropertyHint.Layers3DPhysics)] private uint areaDetectionMask = 4294967295; 

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        if(!inventories.ContainsKey(Enums.InventoryType.HOTBAR)) return;
        
        InventorySlot activeSlot = inventories[Enums.InventoryType.HOTBAR].GetSlot(selectedItemIndex);
        
        if (activeSlot == null || activeSlot.IsEmpty) return;

        ItemData itemData = activeSlot.Item;

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                HandleLeftClick(itemData);
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right)
            {
                HandleRightClick(itemData);
            }
        }
    }

    private void HandleLeftClick(ItemData itemData)
    {
        if (!parentPlayer.TryGetPlayerComponent<PlayerInteraction>(out var playerInteraction)) return;
        
        var result = playerInteraction.PerformRaycast(10);
        if (!result.TryGetValue("position", out var position)) return;
        Vector3 hitPos = (Vector3)position;
        
        if (!itemData.TryGetItemComponent<PlantableComponent>(out var plantableComponent)) return;
        
        FarmingArea targetArea = FindFarmingAreaAtPosition(hitPos);

        if (targetArea != null)
        {

            Vector3? nearestAvailable = targetArea.GetNearestAvailableSpot(hitPos);

            if (nearestAvailable.HasValue)
            {
                targetArea.PlantAtSpot(nearestAvailable.Value, itemData, parentPlayer);
                inventories[Enums.InventoryType.HOTBAR].TryRemoveFromSlot(selectedItemIndex, 1);
            }
        }
    }

    private FarmingArea FindFarmingAreaAtPosition(Vector3 globalPos)
    {
        // Get the space state directly
        var spaceState = parentPlayer.GetWorld3D().DirectSpaceState;

        // Create a query that checks a specific point in 3D space
        var query = new PhysicsPointQueryParameters3D();
        query.Position = globalPos;
        
        // Very important: We are looking for Areas (Triggers), not physics bodies (Walls/Floor)
        query.CollideWithAreas = true;
        query.CollideWithBodies = false;
        query.CollisionMask = areaDetectionMask;

        var results = spaceState.IntersectPoint(query);

        // Iterate through everything touching this point
        foreach (Dictionary res in results)
        {
            // Get the collider (The Area3D node)
            Node colliderNode = res["collider"].As<Node>();

            if (colliderNode is Area3D)
            {
                // In your architecture, AllowedArea is a Node3D that *contains* an Area3D.
                // So if we hit the Area3D, the Script is likely on the Parent.
                if (colliderNode.GetParent() is FarmingArea farmingArea)
                {
                    return farmingArea;
                }
                
                // Fallback: Check if the script is on the Area3D itself (depending on setup)
                if (colliderNode is FarmingArea directFarmingArea)
                {
                    return directFarmingArea;
                }
            }
        }

        return null;
    }

    private void HandleRightClick(ItemData itemData)
    {
        if (!itemData.TryGetItemComponent<InteractalbleComponent>(out var interactableComponent)) return;
        
        interactableComponent.Interact(parentPlayer, itemData);
    }


    protected override void Initialize(Player parent)
    {
	    base.Initialize(parent);
	    foreach (var inventoryKVP in inventories)
	    {
		    inventoryKVP.Value.InitializeInventory();
	    }
    }
}