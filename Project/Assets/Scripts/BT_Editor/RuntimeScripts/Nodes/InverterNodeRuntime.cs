using UnityEngine;
using System.Collections;

[System.Serializable]
public class InverterNodeRuntime : BaseDecoratorNodeRuntime {

	public override void Tick() {
		base.Tick ();
	}

	public override string ComputeResult() {
		BaseNodeEditor child = editorNode.child;
		if (child.runtimeNode.ResultValue.Equals(Result.RUNNING.ToString()))
			return Result.RUNNING.ToString();
		else if (child.runtimeNode.ResultValue.Equals(Result.SUCCESS.ToString()))
			return Result.FAILURE.ToString();
		else
			return Result.SUCCESS.ToString();
	}
}
