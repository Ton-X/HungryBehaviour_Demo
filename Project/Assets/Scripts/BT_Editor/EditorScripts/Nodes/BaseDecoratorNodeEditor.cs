using System;
using UnityEngine;
using System.Collections.Generic;

public abstract class BaseDecoratorNodeEditor : BaseNodeEditor
{
	public BaseNodeEditor child;

#if UNITY_EDITOR
	public override void Draw ()
	{
		base.Draw ();
	}

	public override void DrawConnections ()
	{
		if (child) {
			Rect childRect = child.nodeRect;
			NodeEditorWindow.DrawNodeLines (nodeRect, childRect);
		}
	}
#endif
	public override void AddChild (BaseNodeEditor node)
	{
		if (child != null) {
			child.Parent = null;
			UnityEngine.Object.DestroyImmediate (child, true);
		}
		child = node;
		node.Parent = this;
	}



	public override void NodeDeleted (BaseNodeEditor node)
	{
		if (Parent && Parent.Equals (node)) {
			Parent = null;
		}
		if (child && child.Equals(node)) {
			child = null;
		}
	}
}
