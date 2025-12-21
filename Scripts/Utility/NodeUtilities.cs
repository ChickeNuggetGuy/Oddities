using Godot;
using System;
using System.Collections.Generic;

public static partial class NodeExtensions
{
	public static bool TryGetComponentsInChildrenRecursive<T>(
		this Node node,
		out List<T> retList,
		string attachedScriptPropertyName = null
	) where T : class
	{
		retList = new List<T>();

		foreach (Node child in node.GetChildren())
		{
			// Check if the child itself is of type T.
			if (child is T component)
			{
				retList.Add(component);
			}
			// If not, and if a property name was provided, try to get that property.
			else if (!string.IsNullOrEmpty(attachedScriptPropertyName))
			{
				var propertyInfo = child.GetType().GetProperty(attachedScriptPropertyName);
				if (propertyInfo != null)
				{
					var propertyValue = propertyInfo.GetValue(child);
					if (propertyValue is T propertyComponent)
					{
						retList.Add(propertyComponent);
					}
				}
			}

			// Recursively check the child's children.
			if (child.TryGetComponentsInChildrenRecursive<T>(out List<T> childComponents, attachedScriptPropertyName))
			{
				retList.AddRange(childComponents);
			}
		}

		return retList.Count > 0;
	}
	
	public static bool TryGetAllComponentsInChildren<T>(
		this Node node,
		out List<T> retList,
		string attachedScriptPropertyName = null
	) where T : class
	{
		retList = new List<T>();

		foreach (Node child in node.GetChildren())
		{
			// Check if the child itself is of type T.
			if (child is T component)
			{
				retList.Add(component);
			}
		}

		return retList.Count > 0;
	}

	public static T GetOrCreateNodeAndAddAsChild<T>(this Node parent, string path) where T : Node, new()
	{
		T node = parent.GetNodeOrNull<T>(path);
		if (node == null)
		{
			node = new T();
			parent.AddChild(node);
		}
		return node;
	}
}
