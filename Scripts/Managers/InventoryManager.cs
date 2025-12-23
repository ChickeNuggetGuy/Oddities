using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class InventoryManager : Manager<InventoryManager>
{
    // Define the path where your ItemData resources are stored
    private const string ItemsPath = "res://Data/Items/";
    [Export] private PackedScene itemScene;

    public Dictionary<int, ItemData> ItemDatabase { get; private set; } =
        new Dictionary<int, ItemData>();

    protected override void Setup()
    {
        LoadItemsFromPath(ItemsPath);
    }

    private void LoadItemsFromPath(string path)
    {
        ItemDatabase.Clear();

        using var dir = DirAccess.Open(path);
        if (dir == null)
        {
            GD.PrintErr($"InventoryManager: Could not open path: {path}");
            return;
        }

        dir.ListDirBegin();
        string fileName = dir.GetNext();
        int currentIndex = 0;

        while (fileName != "")
        {
            // Only load resource files, skipping import files
            if (!dir.CurrentIsDir() &&
                (fileName.EndsWith(".tres") || fileName.EndsWith(".res")))
            {
                string fullPath = path.PathJoin(fileName);
                ItemData item = GD.Load<ItemData>(fullPath);

                if (item != null)
                {
                    ItemDatabase.Add(currentIndex, item);
                    currentIndex++;
                }
            }
            fileName = dir.GetNext();
        }

        GD.Print($"InventoryManager: Loaded {ItemDatabase.Count} items.");
    }

    public bool TryGetRandomItem(out ItemData itemData)
    {
        itemData = null;
        if (ItemDatabase == null || ItemDatabase.Count == 0)
        {
            return false;
        }
        
        int itemIndex = GD.RandRange(0, ItemDatabase.Count - 1);
        itemData = (ItemData)ItemDatabase[itemIndex].Duplicate(true);


        return itemData != null;
    }

    public WorldItem InstantiateWorldItem(ItemData itemData, int count)
    {
	    ItemData item = (ItemData)itemData.Duplicate(true);
	    WorldItem retVal = itemScene.Instantiate() as WorldItem;
	    retVal.itemData = item;
        retVal.Initialize();
        return retVal;
    }

    public bool TryGetItemByName(string name, out ItemData itemData)
    {
        itemData = null;
        if (ItemDatabase == null || ItemDatabase.Count == 0)
        {
            return false;
        }

        foreach (ItemData item in ItemDatabase.Values)
        {
            if (item.ItemName == name)
            {
                itemData = (ItemData)item.Duplicate(true);
                return true;
            }
        }

        return false;
    }
}