using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCompositeNodeEditor : BaseNodeEditor
{
#if UNITY_EDITOR
	public override void Draw ()
	{
		base.Draw ();
	}

	public override void DrawConnections ()
	{
		foreach (BaseNodeEditor child in children) {
			Rect childRect = child.nodeRect;
			NodeEditorWindow.DrawNodeLines(nodeRect, childRect);
		}
	}
#endif

	public override void AddChild (BaseNodeEditor node)
	{
		children.Add(node);
		node.Parent = this;
	}

	public override void NodeDeleted (BaseNodeEditor node)
	{
		if (Parent && Parent.Equals (node)) {
			Parent = null;
		}
		if (children.Contains(node)) {
			children.Remove(node);
		}
	}
}

