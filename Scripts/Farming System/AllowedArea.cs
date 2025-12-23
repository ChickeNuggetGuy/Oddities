using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class AllowedArea : Node3D
{
    [Export] protected Area3D area;
    [Export] protected CollisionShape3D collisionShape;
    [Export] protected float gridSpacing = 1.0f;
    [Export] protected float raycastHeightOffset = 5.0f;
    [Export(PropertyHint.Layers3DPhysics)] protected uint groundLayerMask = 1;

    [ExportGroup("Debug")]
    [Export] protected bool debugMode = false;
    [Export] protected float debugSphereRadius = 0.15f;
	
    
    protected Dictionary<Vector3, Node3D> occupiedSpots = new Dictionary<Vector3, Node3D>();

    protected List<Vector3> validPoints = new List<Vector3>();

    public override void _Ready()
    {
        GenerateGrid();
        area.BodyEntered += AreaOnBodyEntered;
        area.BodyExited += AreaOnBodyExited;
    }

    private void AreaOnBodyExited(Node3D body)
    {
	    if (body is Player player)
	    {
		    player.CurrentAreas.Remove(this);
	    }
    }

    private void AreaOnBodyEntered(Node3D body)
    {
	    if (body is Player player)
	    {
		    player.CurrentAreas.Add(this);
	    }
    }

    public override void _Process(double delta)
    {
        if (debugMode)
        {
            DrawDebug();
        }
    }

    protected virtual void DrawDebug()
    {
        foreach (var point in validPoints)
        {
            // Default color for base AllowedArea is White
            DebugDraw3D.DrawSphere(point, debugSphereRadius, occupiedSpots.ContainsKey(point) ? Colors.Red : Colors.White);
        }
    }

    public void GenerateGrid()
    {
        if (collisionShape?.Shape is not BoxShape3D box)
        {
            GD.PrintErr("AllowedArea: Requires a BoxShape3D to generate a grid.");
            return;
        }

        validPoints.Clear();
        Vector3 size = box.Size;
        Transform3D globalTrans = collisionShape.GlobalTransform;

        float halfX = size.X / 2;
        float halfZ = size.Z / 2;

        for (float x = -halfX; x <= halfX; x += gridSpacing)
        {
            for (float z = -halfZ; z <= halfZ; z += gridSpacing)
            {
                Vector3 localPos = new Vector3(x, size.Y / 2, z);
                Vector3 globalPos = globalTrans.Origin + globalTrans.Basis * localPos;

                if (TryGetGroundPoint(globalPos, out Vector3 hitPoint))
                {
                    validPoints.Add(hitPoint);
                }
            }
        }
    }

    private bool TryGetGroundPoint(Vector3 startPos, out Vector3 hitPoint)
    {
        hitPoint = Vector3.Zero;
        var spaceState = GetWorld3D().DirectSpaceState;

        Vector3 endPos =
            startPos +
            Vector3.Down * (collisionShape.Shape as BoxShape3D).Size.Y * 2;

        var query = PhysicsRayQueryParameters3D.Create(
            startPos,
            endPos,
            groundLayerMask
        );
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            hitPoint = (Vector3)result["position"];
            return true;
        }

        return false;
    }

    public List<Vector3> GetValidPoints() => validPoints;
    
    
    public Vector3? GetNearestAvailableSpot(Vector3 globalPosition, float maxDistance = 1.5f)
    {
	    var nearest = validPoints
		    .Where(p => !occupiedSpots.ContainsKey(p))
		    .OrderBy(p => p.DistanceTo(globalPosition))
		    .Cast<Vector3?>()
		    .FirstOrDefault();

	    if (nearest.HasValue && nearest.Value.DistanceTo(globalPosition) > maxDistance)
		    return null;

	    return nearest;
    }
    
    public Vector3? GetNearestSpot(Vector3 globalPosition, float maxDistance = 1.5f)
    {
	    var nearest = validPoints
		    .OrderBy(p => p.DistanceTo(globalPosition))
		    .Cast<Vector3?>()
		    .FirstOrDefault();

	    if (nearest.HasValue && nearest.Value.DistanceTo(globalPosition) > maxDistance)
		    return null;

	    return nearest;
    }
    
    public Dictionary<Vector3, Node3D> GetOccupiedSpots() => occupiedSpots;
}