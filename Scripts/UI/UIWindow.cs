using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class UIWindow : UIElement
{
	[Export] private Enums.UIType uiType = Enums.UIType.NONE;
    public bool IsShown { get; protected set; }
    [Export] private bool startHidden = true;
    [Export] private Key toggleKey;
    [Export] private Control visual;

    public Array<UIElement> uiChildren = new Array<UIElement>();

    public override void Initilize(UIWindow parent)
    {
        uiChildren.Clear();
        // Start searching through the scene tree nodes under this window
        DiscoverUIElements(this);

        if (startHidden)
        {
            HideCall();
        }
        else
        {
            ShowCall();
        }
        if(!UIManager.Instance.uiWindows.ContainsKey(uiType))
        {
	        UIManager.Instance.uiWindows[uiType] = this;
        }
        else
        {
	        GD.PrintErr($"UI Window already exists: {uiType}");
        }
    }
	
    private void DiscoverUIElements(Node root)
    {
        foreach (Node child in root.GetChildren())
        {
            if (child is UIElement uiElem)
            {
                uiElem.InitilizeCall(this);
                uiChildren.Add(uiElem);
                
                if (uiElem is UIWindow)
                {
                    continue;
                }
            }


            DiscoverUIElements(child);
        }
    }

    public void ShowCall()
    {
        if (IsShown) return;
        IsShown = true;
        visual?.Show();
        Show();
    }

    protected virtual void Show() { }

    public void HideCall()
    {
        if (!IsShown) return;
        IsShown = false;
        visual?.Hide();
        Hide();
    }

    protected virtual void Hide() { }
}