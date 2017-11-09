using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class BaseCompositeNodeRuntime : BaseNodeRuntime {

	public BaseCompositeNodeEditor editorNode;

	public override void Tick ()
	{

		if (editorNode.children.Count > 0) {
			ResultValue = ComputeResult ();
		}
	}

	public abstract string ComputeResult ();
}
