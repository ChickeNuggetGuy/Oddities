using Godot.Collections;
using Godot;
public partial class UIManager : Manager<UIManager>
{
	// Remove [Export] to prevent Inspector nulling this out
	public Dictionary<Enums.UIType, UIWindow> uiWindows = new Dictionary<Enums.UIType, UIWindow>();
    
	[Export] public UIWindow rootWindow ;
	public UIWindow blocikingWindow;

	protected override void Setup()
	{
		if (rootWindow == null)
		{
			GD.PrintErr("UIManager: Root Window is not assigned in inspector!");
			return;
		}
        
		rootWindow.InitilizeCall(null);
		GD.Print("UIManager: Setup completed successfully.");
	}

	public bool TryBlockInput(UIWindow window)
	{
		if (InputManager.Instance.InputBlocked) return false;
		
		blocikingWindow = window;
		InputManager.Instance.InputBlocked = true;
		return true;
		
	}

	public void UnblockInput(UIWindow window)
	{
		if(blocikingWindow != null && blocikingWindow != window) return;
		InputManager.Instance.InputBlocked = false;
		blocikingWindow = null;
	}
}