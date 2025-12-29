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
    [Export] protected bool blockInput = false;
    [Export] public bool showMouse = false;
    public Array<UIElement> uiChildren = new Array<UIElement>();

    protected override void Initilize(UIWindow parent)
    {
        uiChildren.Clear();
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


	    if (showMouse)
	    {
		    InputManager.Instance.ChangeMouseMode(Input.MouseModeEnum.Confined);
	    }
	    
		if (blockInput)
        {
	        UIManager.Instance.TryBlockInput(this);
        }
        visual?.Show();
        Show();
    }

    protected virtual void Show() { }

    public void HideCall()
    {
        IsShown = false;
        visual.Hide();

        if (showMouse)
        {
	        InputManager.Instance.ChangeMouseMode(Input.MouseModeEnum.Captured);
        }
        
        if (blockInput)
        {
	        UIManager.Instance.UnblockInput(this);
        }
        Hide();
    }

    protected virtual void Hide() { }

    public void Toggle()
    {
	    if (IsShown) HideCall();
	    else ShowCall();
    }
    public override void _Input(InputEvent @event)
    {
	    base._Input(@event);
	    if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == toggleKey)
	    {
		    Toggle();
	    }
    }
}