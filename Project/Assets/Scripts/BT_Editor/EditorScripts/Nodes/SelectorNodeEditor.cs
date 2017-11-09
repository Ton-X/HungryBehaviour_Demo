using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SelectorNodeEditor : BaseCompositeNodeEditor {

	public SelectorNodeEditor() {
		title = "Selector";
		children = new List<BaseNodeEditor> ();
	}
#if UNITY_EDITOR
	public override void Draw() {
		base.Draw();
	}

	public override void AddChild(BaseNodeEditor node) {
		base.AddChild (node);
	}

	public override void DrawConnections() {
		base.DrawConnections ();
	}
#endif
	public override void NodeDeleted(BaseNodeEditor node) {
		base.NodeDeleted (node);
	}

	public override BaseNodeRuntime InstantiateRuntimeNode ()
	{
		SelectorNodeRuntime selRun = new SelectorNodeRuntime ();
		selRun.editorNode = this;
		runtimeNode = selRun;
		return selRun;
	}

	public override BaseNodeRuntime InstantiateRuntimeNode (GameObject gameObj, Type type)
	{
		return null;
	}
}
