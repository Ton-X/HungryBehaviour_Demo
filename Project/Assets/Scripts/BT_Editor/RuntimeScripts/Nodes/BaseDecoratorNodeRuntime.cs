using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class BaseDecoratorNodeRuntime : BaseNodeRuntime {

	public BaseDecoratorNodeEditor editorNode;

	public override void Tick ()
	{
		if (editorNode.child != null) {
			ResultValue = ComputeResult();
		}

		if (editorNode && editorNode.child) {
			while (editorNode.child.runtimeNode.ResultValue.Equals(Result.RUNNING.ToString())) {
				editorNode.child.runtimeNode.Tick ();
			}

		}

		ResultValue = ComputeResult ();
	}

	public abstract string ComputeResult ();

}
