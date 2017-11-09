using UnityEngine;
using System.Collections;
using System;

public class InverterNodeEditor : BaseDecoratorNodeEditor {

	public InverterNodeEditor() {
		title = "Inverter";
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
		InverterNodeRuntime invRun = new InverterNodeRuntime ();
		invRun.editorNode = this;
		runtimeNode = invRun;
		return invRun;
	}

	public override BaseNodeRuntime InstantiateRuntimeNode (GameObject gameObj, Type type)
	{
		return null;
	}
}
