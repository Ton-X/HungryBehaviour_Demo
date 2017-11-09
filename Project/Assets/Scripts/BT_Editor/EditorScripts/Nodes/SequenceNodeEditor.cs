using System.Collections.Generic;
using UnityEngine;
using System;

public class SequenceNodeEditor : BaseCompositeNodeEditor {
	
	public SequenceNodeEditor() {
		title = "Sequence";
		children = new List<BaseNodeEditor>();
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
		SequenceNodeRuntime seqRun = new SequenceNodeRuntime ();
		seqRun.editorNode = this;
		runtimeNode = seqRun;
		return seqRun;
	}

	public override BaseNodeRuntime InstantiateRuntimeNode (GameObject gameObj, Type type)
	{
		return null;
	}
}

