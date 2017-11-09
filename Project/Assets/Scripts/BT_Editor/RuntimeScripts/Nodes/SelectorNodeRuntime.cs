using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SelectorNodeRuntime : BaseCompositeNodeRuntime {

	public override void Tick() {
		
		foreach (BaseNodeEditor child in editorNode.children) {
			ResultValue = ComputeResult();
			if (ResultValue.Equals (Result.SUCCESS.ToString ()))
				break;
			child.runtimeNode.Tick ();
		}

		base.Tick ();
	}

	public override string ComputeResult() {
		List<BaseNodeEditor> children = editorNode.children;
		if (children.Exists( child => child.runtimeNode.ResultValue.Equals(Result.SUCCESS.ToString()) ))
			return Result.SUCCESS.ToString();
		else if (children.Exists( child => child.runtimeNode.ResultValue.Equals(Result.RUNNING.ToString()) ))
			return Result.RUNNING.ToString();
		else
			return Result.FAILURE.ToString();
	}
}
